using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Npgsql;

using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Notify.V1.Service;
using Twilio.Types;

using StockWatcher.Model.Data;
using StockWatcher.Model.Schemas;

using StockWatcher.Model.Actions;

namespace StockWatcher.Controllers {
    public class AccountController : Controller {
        private string accountSid = Environment.GetEnvironmentVariable("TwilioAcctSid");
        private string authToken = Environment.GetEnvironmentVariable("TwilioAuthToken");
        private string serviceSid = Environment.GetEnvironmentVariable("TwilioServiceSid");

        [HttpPost]
        public ActionResult CreateAccount([FromBody]User user) {
            var responseMsg = new ResponseMessage(){Status=false,Message="Account already in use"};
            if (ModelState.IsValid) {
                if (ManageAccount.AddUser(user)) {
                    responseMsg.Status = true;
                    responseMsg.Message = "Successfully created an account";
                }
            }
            else {
                responseMsg.Message = "Incorrect input. Fix and try again.";
            }
            return Json(responseMsg);
        }
    }
}