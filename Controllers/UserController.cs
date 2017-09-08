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
        private JWTService jwtService;
        public UserController(ManageUser _manageUser, JWTService _jwtService)
        {
            manageUser = _manageUser;
            jwtService = _jwtService;
        }
        private string UserSid = Environment.GetEnvironmentVariable("TwilioAcctSid");
        private string authToken = Environment.GetEnvironmentVariable("TwilioAuthToken");
        private string serviceSid = Environment.GetEnvironmentVariable("TwilioServiceSid");

        [HttpPost]
        public IActionResult CreateUser([FromForm]User user)
        {
            Console.WriteLine(Request.Headers.Values);
            Response.StatusCode = 201;
            if (ModelState.IsValid)
            {
                if (manageUser.AddUser(user))
                {
                    Response.StatusCode = 201;
                    return Json(jwtService.CreateToken(user.Username));
                }
                else
                {
                    Response.StatusCode = 409;
                }
            }
            else
            {
                Response.StatusCode = 400;
            }

            return Json("");
            
        }

        [HttpPost]
        public IActionResult LoginUser([FromForm]User user)
        {
            return Json("");
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