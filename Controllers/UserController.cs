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

namespace StockWatcher.Controllers
{

    public class UserController : Controller
    {
        private ManageUser manageUser;
        public UserController(ManageUser _manageUser)
        {
            manageUser = _manageUser;
        }
        private string UserSid = Environment.GetEnvironmentVariable("TwilioAcctSid");
        private string authToken = Environment.GetEnvironmentVariable("TwilioAuthToken");
        private string serviceSid = Environment.GetEnvironmentVariable("TwilioServiceSid");

        [HttpPost]
        public void CreateUser([FromForm]User user)
        {
            var responseMsg = "";
            Response.StatusCode = 201;
            if (ModelState.IsValid)
            {
                if (manageUser.AddUser(user))
                    responseMsg = "Successfully created an User";
                else
                {
                    // Response.StatusCode = 409;
                    Response.Redirect("/signup");
                    responseMsg = "User already exists";
                }
            }
            else
            {
                // Response.StatusCode = 400;
                Response.Redirect("/signup");
                responseMsg = "Incorrect input. Fix and try again.";
            }
            Response.Redirect("/");
            // return Json(responseMsg);
        }

        [HttpDelete]
        public void RemoveUser([FromBody]User user)
        {
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