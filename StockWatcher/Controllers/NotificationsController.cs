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

using StockWatcher.Model;
using StockWatcher.Model.Services;
using StockWatcher.Model.Schemas;

namespace StockWatcher.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly IWatchService watch;
        private readonly StockDbContext context;
        public NotificationsController(IWatchService watchService)
        {
            watch = watchService;
        }

        [HttpPost]
        [Route("/watch")]
        public async Task WatchPrice([FromBody]Stock stock)
        {
            if (ModelState.IsValid)
            {
                if (watch.AddRequest(stock))
                {
                    bool success = await watch.ScheduleWatch(stock, "no_id");
                    if (success)
                    {
                        Response.StatusCode = 201;
                    }
                    else
                    {
                        Response.StatusCode = 500;
                    }
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

