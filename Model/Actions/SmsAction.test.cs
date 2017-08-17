using System;
using System.Collections.Generic;

using Twilio;
using Twilio.Rest.Notify.V1.Service;
using Twilio.Types;
using Xunit;

using StockWatcher.Model.Actions;
using StockWatcher.Model.Schemas;
using StockWatcher.Model.Data;

namespace StockWatcher.Tests {
    public class SmsActionTest: IDisposable {
        private User user;
        private string accountSid = 
            Environment.GetEnvironmentVariable(
                "TwilioAcctSid"
            );
        private string authToken = 
            Environment.GetEnvironmentVariable(
                "TwilioAuthToken"
            );
        private string serviceSid = 
            Environment.GetEnvironmentVariable(
                "TwilioServiceSid"
            );

        public SmsActionTest() {
            // GIVEN a User
            // WITH valid fields
            user = new User() {
                Username = "dummy",
                Password = "password",
                Phone = Environment.GetEnvironmentVariable("PhoneNumber"),
                Uuid = Guid.NewGuid().ToString()
            };
            new AccountsDb().Add(user);
       }

       public void Dispose() {
           new AccountsDb().Remove(user);
       }

        [Fact]
        public void CreateBinding() {

            //WHEN a binding is made with the User data
            var binding = new SmsAction().MakeBinding(user);

            // IT should be of type BindingResource
            Assert.IsType(typeof(BindingResource),binding);

            // IT should have registered uuid as the indentity
            Assert.Equal(user.Uuid,binding.Identity);
        }

        [Theory]
        [InlineData("dummy", "msft", 1.1, 2.2)]
        public void NotifyUsers(
            string name, 
            string equity, 
            double price, 
            double openPrice
        ) {
            // GIVEN a list of usernames
            List<string> usernames = new List<string>() {name};
            // AND a Stock
            Stock stock = new Stock() {
                Username = name,
                Equity = equity,
                Price = price
            };
            // AND a binded identity 
            CreateBinding();
            TwilioClient.Init(accountSid, authToken);
            var bindings = BindingResource.Read(serviceSid);
            var bindingIds = new List<string>();

            foreach (var binding in bindings) {
                bindingIds.Add(binding.Identity);
            }
            // AND an openPrice
            // WHEN a notification is created
            var notification = new SmsAction().NotifyUsers(
                usernames,
                stock,
                openPrice
            );
            // IT should have the same identities as the ones
            // stored in the accounts database
            Assert.Equal(
                notification.Identities.ToArray()[0],
                user.Uuid
            );
        }
    }
}