using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Hangfire;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Twilio;
using Twilio.Rest.Notify.V1.Service;
using Twilio.Types;

// using StockWatcher.Model.Actions;
using StockWatcher.Model.Services;
using StockWatcher.Model.Schemas;

namespace StockWatcher.Controllers {
    public class NotificationsController : Controller {
        private string accountSid = Environment.GetEnvironmentVariable("TwilioAcctSid");
        private string authToken = Environment.GetEnvironmentVariable("TwilioAuthToken");
        private string serviceSid = Environment.GetEnvironmentVariable("TwilioServiceSid");

        private SmsService service;
        public NotificationsController(SmsService _service) 
        {
            service = _service;
        }

        [HttpPost]
        public ActionResult WatchPrice([FromBody]Stock stock) {
            // var responseMsg = new ResponseMessage(){Status=false, Message="Request is already running"};
            string msg = "";
            if (ModelState.IsValid) {
                var notification = service.NotifyUsers(stock,1);
                msg = notification.Body;
                Response.StatusCode = 201;
                // var jobId = Guid.NewGuid().ToString();
                // if (ManageRequest.Add(stock)) {
                //     RecurringJob.AddOrUpdate<PollStock>(
                //         jobId,
                //         pollStock => 
                //         pollStock.Poll(stock, jobId),
                //         Cron.Minutely()
                //     );
                //     responseMsg.Status = true;
                //     responseMsg.Message = "Request successfully queued";
                // }
            }
            return Json(msg);
        }

        [HttpPost]
        public void CancelRequest([FromBody]Stock stock) {
            // Remove user
        }
    }
}
