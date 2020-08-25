using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Hangfire.WebUI.Models
{
    public class CroneJob
    {
        [Display(Name = "Job ID")]
        public int Id { get; set; }
        [Display(Name = "Job Name")]
        [MaxLength(255)]
        [Required]
        public string JobName { get; set; }
        [Display(Name = "Job URL Request")]
        [Required]
        public string JobUrl { get; set; }
        [Display(Name = "Request Type")]
        [MaxLength(15)]
        [Required]
        public string RequestType { get; set; } = "GET";
        [Display(Name = "Repeat Every")]
        [MaxLength(15)]
        [Required]
        public string RepeatEvery { get; set; }
        public DateTime? DateAdded { get; set; } = DateTime.Now;

        //User
        public string ApplicationUserId { get; set; } = "N/A";
    }
}