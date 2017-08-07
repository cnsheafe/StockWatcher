using System;
using System.Collections.Generic;

using Twilio;
using Twilio.Rest.Notify.V1.Service;
using Twilio.Types;

using StockWatcher.Model;

namespace StockWatcher.Data {
    public class TwilioAction {
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
            TwilioClient.Init(accountSid, authToken);
            NotificationResource notification = NotificationResource.Create(
                serviceSid,
                identity: new List<string> {stock.uuid},
                body: $"\n {stock.equity.ToUpper()} has reached price ${openPrice}"
            );
        }
    }
}