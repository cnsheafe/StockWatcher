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

namespace StockWatcher.Controllers
{
    public class NotificationsController : Controller
    {
        private string accountSid = Environment.GetEnvironmentVariable("TwilioAcctSid");
        private string authToken = Environment.GetEnvironmentVariable("TwilioAuthToken");
        private string serviceSid = Environment.GetEnvironmentVariable("TwilioServiceSid");

        private readonly StockRequestService requestService;
        public NotificationsController(StockRequestService _requestService)
        {
            requestService = _requestService;
        }

        [HttpPost]
        public void WatchPrice([FromBody]Stock stock)
        {
            if (ModelState.IsValid)
            {
                if (requestService.AddRequest(stock))
                {
                    var jobId = Guid.NewGuid().ToString();
                    RecurringJob.AddOrUpdate<StockRequestService>(
                        jobId,
                        service =>
                        service.QueryStock(stock, jobId),
                        Cron.Minutely()
                    );
                    Response.StatusCode = 201;
                }
                else
                {
                    Response.StatusCode = 309;
                }
            }
            else
            {
                Response.StatusCode = 400;
            }

        }
        [HttpPost]
        public void CancelRequest([FromBody]Stock stock)
        {
            // Remove user
        }
    }
}

