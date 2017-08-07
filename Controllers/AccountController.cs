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

using StockWatcher.Data;

namespace StockWatcher.Controllers {
    public class AccountController : Controller {
        private string accountSid = Environment.GetEnvironmentVariable("TwilioAcctSid");
        private string authToken = Environment.GetEnvironmentVariable("TwilioAuthToken");
        private string serviceSid = Environment.GetEnvironmentVariable("TwilioServiceSid");

       [HttpPost]
        public void CreateBinding([FromBody]JObject info) {
            string uuid;
            var db = new StockWatcherDb();
            do {
                uuid = Guid.NewGuid().ToString();
            } while (!db.CheckUuid(uuid));

            TwilioClient.Init(accountSid, authToken);
            Console.WriteLine(info);
            string phoneNumber = info["info"]["phoneNumber"].ToString();

            var binding = BindingResource.Create(
                serviceSid,
                identity: uuid,
                bindingType: BindingResource.BindingTypeEnum.Sms,
                address: phoneNumber);
            Console.WriteLine(binding.ServiceSid);
            Console.WriteLine(binding.Identity);
        }
        [HttpGet]
        public void ListBindings() {
            TwilioClient.Init(accountSid, authToken);
            var bindings = BindingResource.Read(serviceSid);
            foreach (var binding in bindings) {
                Console.WriteLine(binding.Identity);
            }
        }

        public void Remove([FromBody]JObject userInfo) {

        }


    }
}