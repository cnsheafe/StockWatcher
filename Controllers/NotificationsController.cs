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

using StockWatcher.Model.Services;
using StockWatcher.Model.Schemas;

namespace StockWatcher.Controllers {
    public class NotificationsController : Controller {
        private string accountSid = Environment.GetEnvironmentVariable("TwilioAcctSid");
        private string authToken = Environment.GetEnvironmentVariable("TwilioAuthToken");
        private string serviceSid = Environment.GetEnvironmentVariable("TwilioServiceSid");

        private readonly SmsService smsService;
        private readonly StockRequestService requestService;
        public NotificationsController(SmsService _smsService, StockRequestService _requestService) 
        {
            smsService = _smsService;
            requestService = _requestService;
        }

        [HttpPost]
        public ActionResult WatchPrice([FromBody]Stock stock) 
        {
            string msg = "";
            if (ModelState.IsValid) {
                var request = requestService.AddRequest(stock);
                var notification = smsService.NotifyUsers(stock,1);
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
