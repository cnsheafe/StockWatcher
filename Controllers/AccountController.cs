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

using StockWatcher.Model;
using StockWatcher.Model.Schemas;
using StockWatcher.Model.Services;

namespace StockWatcher.Controllers {

    public class AccountController : Controller 
    {
        private ManageUser manageUser;
        public AccountController(ManageUser _manageUser) {
            manageUser = _manageUser;
        }
        private string accountSid = Environment.GetEnvironmentVariable("TwilioAcctSid");
        private string authToken = Environment.GetEnvironmentVariable("TwilioAuthToken");
        private string serviceSid = Environment.GetEnvironmentVariable("TwilioServiceSid");

        [HttpPost]
        public ActionResult CreateAccount([FromBody]User user) 
        {
            var responseMsg = "";
            Response.StatusCode = 201;
            if (ModelState.IsValid) 
            {
                if (manageUser.AddUser(user)) 
                    responseMsg = "Successfully created an account";
                else
                {
                    Response.StatusCode = 409;
                    responseMsg = "User account already exists";
                }
            }
            else 
            {
                Response.StatusCode = 400;
                responseMsg = "Incorrect input. Fix and try again.";
            }
            return Json(responseMsg);
        }

        [HttpDelete]
        public void RemoveAccount([FromBody]User user) {
            Response.StatusCode = 204;
            if (ModelState.IsValid)
            {
                if (!manageUser.RemoveUser(user))
                    Response.StatusCode = 409;
            }
            else
            {
                Response.StatusCode = 400;
            }
        }
    }
}