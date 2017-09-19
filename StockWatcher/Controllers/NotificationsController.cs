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
        private readonly IStockRequestService requestService;
        public NotificationsController(IStockRequestService _requestService)
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
                    RecurringJob.AddOrUpdate<IStockRequestService>(
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
    }
}

