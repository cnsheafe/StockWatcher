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

        public void NotifyUser(Stock stock, double openPrice) {
            var surveyDb = new StockSurveyDb(); 
            string uuid = surveyDb.GetUuid(stock.Username);
            Console.WriteLine(uuid);

            TwilioClient.Init(accountSid, authToken);
            NotificationResource notification = NotificationResource.Create(
                serviceSid,
                identity: new List<string> {uuid},
                body: $"${stock.Equity.ToUpper()} has exceeded target price of {stock.Price} and has reached price ${openPrice}"
            );
            Console.WriteLine(notification.Identities.ToString());
        }
        /// <summary>
        /// Registers user phone number to a UUID on Twilio
        /// </summary>
        /// <param name="user">
        /// User credentials for binding
        /// </param>
        public void MakeBinding(User user) {
            TwilioClient.Init(accountSid, authToken);
            var binding = BindingResource.Create(
                serviceSid,
                identity: user.Uuid,
                bindingType: BindingResource.BindingTypeEnum.Sms,
                address: user.Phone
            );
            Console.WriteLine(binding.Sid);
        }
    }
}