using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Hangfire;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Twilio;
// using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Notify.V1.Service;
using Twilio.Types;

using StockWatcher.Data;
using StockWatcher.Model;

namespace StockWatcher.Controllers {
    public class NotificationsController : Controller {
        private string accountSid = Environment.GetEnvironmentVariable("TwilioAcctSid");
        private string authToken = Environment.GetEnvironmentVariable("TwilioAuthToken");
        private string serviceSid = Environment.GetEnvironmentVariable("TwilioServiceSid");
       [HttpGet]
        public void Test() {
        }

        [HttpPost]
        public void WatchPrice([FromBody]JObject stockJson) {
            var stock = stockJson.ToObject<Stock>();
            BackgroundJob.Enqueue<PollStock>(pollStock => 
                pollStock.Poll(stock)
            ); 
        }
    }
}
