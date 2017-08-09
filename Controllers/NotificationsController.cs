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

using StockWatcher.Model.Actions;
using StockWatcher.Model.Schemas;

namespace StockWatcher.Controllers {
    public class NotificationsController : Controller {
        private string accountSid = Environment.GetEnvironmentVariable("TwilioAcctSid");
        private string authToken = Environment.GetEnvironmentVariable("TwilioAuthToken");
        private string serviceSid = Environment.GetEnvironmentVariable("TwilioServiceSid");

        [HttpPost]
        public void WatchPrice([FromBody]Stock stock) {
            BackgroundJob.Enqueue<PollStock>(pollStock => 
                pollStock.Poll(stock)
            ); 
        }
    }
}
