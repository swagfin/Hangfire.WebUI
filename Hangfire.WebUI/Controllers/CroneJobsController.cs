using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Hangfire.WebUI.Models;

namespace Hangfire.WebUI.Controllers
{
    [Authorize]
    public class CroneJobsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CroneJobs
        public async Task<ActionResult> Index()
        {
            //Get Identit
            string user_id = User.Identity.GetUserId();
            return View(await db.CroneJobs.Where(x => x.ApplicationUserId == user_id).ToListAsync());
        }

        // GET: CroneJobs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string user_id = User.Identity.GetUserId();
            CroneJob croneJob = await db.CroneJobs.FirstOrDefaultAsync(x => x.Id == id && x.ApplicationUserId == user_id);
            if (croneJob == null)
            {
                return HttpNotFound();
            }
            return View(croneJob);
        }

        // GET: CroneJobs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CroneJobs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,JobName,JobUrl,RequestType,RepeatEvery")] CroneJob croneJob)
        {
            if (ModelState.IsValid)
            {
                //Set User Id
                string user_id = User.Identity.GetUserId();
                croneJob.ApplicationUserId = user_id;
                db.CroneJobs.Add(croneJob);
                await db.SaveChangesAsync();
                //Crone Jobs
                TaskForce.InitJob(croneJob);
                return RedirectToAction("Index");

            }

            return View(croneJob);
        }

        // GET: CroneJobs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string user_id = User.Identity.GetUserId();
            CroneJob croneJob = await db.CroneJobs.FirstOrDefaultAsync(x => x.Id == id && x.ApplicationUserId == user_id);
            if (croneJob == null)
            {
                return HttpNotFound();
            }
            return View(croneJob);
        }

        // POST: CroneJobs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,JobName,JobUrl,RequestType,RepeatEvery")] CroneJob croneJob)
        {
            if (ModelState.IsValid)
            {
                db.Entry(croneJob).State = EntityState.Modified;
                string user_id = User.Identity.GetUserId();
                croneJob.ApplicationUserId = user_id;
                await db.SaveChangesAsync();
                TaskForce.InitJob(croneJob);
                return RedirectToAction("Index");
            }
            return View(croneJob);
        }

        // GET: CroneJobs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string user_id = User.Identity.GetUserId();
            CroneJob croneJob = await db.CroneJobs.FirstOrDefaultAsync(x => x.Id == id && x.ApplicationUserId == user_id);
            if (croneJob == null)
            {
                return HttpNotFound();
            }
            return View(croneJob);
        }

        // POST: CroneJobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            CroneJob croneJob = await db.CroneJobs.FindAsync(id);
            db.CroneJobs.Remove(croneJob);
            await db.SaveChangesAsync();
            TaskForce.RemoveJob(croneJob);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
