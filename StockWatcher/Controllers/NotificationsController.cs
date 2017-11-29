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
    /// <summary>
    /// Handles all requests for watches.
    /// </summary>
    public class NotificationsController : Controller
    {
        private readonly IWatchService watch;
        public NotificationsController(IWatchService watchService)
        {
            watch = watchService;
        }

        /// <summary>
        /// Handles requests for watches on particular stock prices. If successful,
        /// the user's request is tabled and an SMS message is sent when the target price is met.
        /// </summary>
        /// <param name="stock">
        /// Contains info such as the user's phone number and target price.
        /// </param>
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

