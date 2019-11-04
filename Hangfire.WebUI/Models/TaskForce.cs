using Hangfire.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Hangfire.WebUI.Models

{
    public static class TaskForce
    {
        public static async Task<string> SendRequest(string Url, string Method, List<KeyValuePair<string, string>> RequestParams)
        {
            try
            {
                //Params
                if (RequestParams == null)
                    RequestParams = new List<KeyValuePair<string, string>>();
                //Add Additional Params
                RequestParams.Add(new KeyValuePair<string, string>("token", System.Guid.NewGuid().ToString()));
                //Check
                var request = new HttpRequestMessage(HttpMethod.Get, Url);
                //Others
                if (Method == "POST")
                    request = new HttpRequestMessage(HttpMethod.Post, Url);
                else if (Method == "PUT")
                    request = new HttpRequestMessage(HttpMethod.Put, Url);
                else if (Method == "DELETE")
                    request = new HttpRequestMessage(HttpMethod.Delete, Url);
                else if (Method == "HEAD")
                    request = new HttpRequestMessage(HttpMethod.Head, Url);
                else
                    return await Request_GET(Url, Method, RequestParams, null);

                request.Content = new FormUrlEncodedContent(RequestParams);
                var client = new HttpClient();
                var response = await client.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }


        }


        public static async Task<string> Request_GET(string Url, string Method, List<KeyValuePair<string, string>> RequestParams, List<KeyValuePair<string, string>> RequestHeaders)
        {
            try
            {


                string dataStr = Url;
                //Check Params
                if (RequestParams != null)
                {
                    //Check if string has ?
                    if (!dataStr.Contains("?"))
                        dataStr += "?";
                    //Loop All
                    foreach (var item in RequestParams)
                    {
                        dataStr += HttpUtility.UrlEncode(item.Key, Encoding.UTF8);
                        dataStr += "=" + HttpUtility.UrlEncode(item.Value, Encoding.UTF8);
                        dataStr += "&";
                    }
                }
                // @PREPARE SEND WEB REQUEST
                HttpWebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(dataStr);
                webRequest.Method = "GET";
                webRequest.Accept = "application/json";
                //  @ADDING HEADERS

                //@Add Headers
                if (RequestHeaders != null)
                {
                    foreach (var item in RequestHeaders)
                        webRequest.Headers.Add(item.Key, item.Value);
                }


                // @SENDING AND CHECKING FOR RESPONSE
                HttpWebResponse httpResponse = (HttpWebResponse)webRequest.GetResponse();
                StreamReader webpageReader = new StreamReader(httpResponse.GetResponseStream());
                string response = await webpageReader.ReadToEndAsync();
                //@ RETURN TRUE IF USER REQUESTED FOR DELIVERY
                return response;

            }
            catch (Exception ex)
            {
                return "Error Request: " + ex.Message;
            }

        }


        public static void RemoveJob(CroneJob croneJob)
        {
            if (croneJob != null)
                RecurringJob.RemoveIfExists(croneJob.Id.ToString());
        }

        public static void InitJob(CroneJob croneJob)
        {
            if (croneJob.RepeatEvery == "Minutely")
                RecurringJob.AddOrUpdate(croneJob.Id.ToString(), () => SendRequest(croneJob.JobUrl, croneJob.RequestType, null), Cron.Minutely);
            if (croneJob.RepeatEvery == "Daily")
                RecurringJob.AddOrUpdate(croneJob.Id.ToString(), () => SendRequest(croneJob.JobUrl, croneJob.RequestType, null), Cron.Daily);
            if (croneJob.RepeatEvery == "Hourly")
                RecurringJob.AddOrUpdate(croneJob.Id.ToString(), () => SendRequest(croneJob.JobUrl, croneJob.RequestType, null), Cron.Hourly);
            if (croneJob.RepeatEvery == "Monthly")
                RecurringJob.AddOrUpdate(croneJob.Id.ToString(), () => SendRequest(croneJob.JobUrl, croneJob.RequestType, null), Cron.Monthly);
            if (croneJob.RepeatEvery == "Yearly")
                RecurringJob.AddOrUpdate(croneJob.Id.ToString(), () => SendRequest(croneJob.JobUrl, croneJob.RequestType, null), Cron.Yearly);
        }


        public static bool InitJobs()
        {
            try
            {
                ApplicationDbContext db = new ApplicationDbContext();
                var storedCroneJobs = db.CroneJobs.ToList();
                foreach (var item in storedCroneJobs)
                {
                    InitJob(item);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }



    }
}