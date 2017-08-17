using System;
using System.Collections.Generic;

using Twilio;
using Twilio.Rest.Notify.V1.Service;
using Twilio.Types;

using StockWatcher.Model.Schemas;
using StockWatcher.Model.Data;

namespace StockWatcher.Model.Actions {
    public class SmsAction {
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

        /// <summary>
        /// Notifies users via SMS about the latest stock price.
        /// </summary>
        /// <param name="usernames">
        /// Usernames associated with the stock's requestId
        /// </param>
        /// <param name="stock">
        /// The particular stock whose target price was met.
        /// </param>
        /// <param name="openPrice">
        /// The latest stock price.
        /// </param>
        public NotificationResource NotifyUsers(List<string> usernames, Stock stock, double openPrice) {
            var request = new StockRequestDb(); 
            var account = new AccountsDb();
            var userIdentities = new List<string>();

            foreach (string username in usernames) {
                Console.WriteLine(account.GetUuid(username));
                userIdentities.Add(account.GetUuid(username));
            }

            TwilioClient.Init(accountSid, authToken);
            NotificationResource notification = NotificationResource.Create(
                serviceSid,
                identity: userIdentities,
                body: $"${stock.Equity.ToUpper()} has exceeded target price of {stock.Price} and has reached price ${openPrice}"
            );
            return notification;
        }
        /// <summary>
        /// Registers user phone number to a UUID on Twilio
        /// </summary>
        /// <param name="user">
        /// User credentials for binding
        /// </param>
        public BindingResource MakeBinding(User user) {
            TwilioClient.Init(accountSid, authToken);
            var binding = BindingResource.Create(
                serviceSid,
                identity: user.Uuid,
                bindingType: BindingResource.BindingTypeEnum.Sms,
                address: user.Phone
            );
            return binding;
        }

        public List<string> ListBindings() {
            TwilioClient.Init(accountSid, authToken);
            var bindings = BindingResource.Read(serviceSid);
            var bindingIds = new List<string>();

            foreach (var id in bindings) {
                Console.WriteLine(id.Identity);
                bindingIds.Add(id.Identity);
            }
            return bindingIds;
        }
    }
}