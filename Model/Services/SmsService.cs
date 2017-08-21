using System;
using System.Collections.Generic;
using System.Linq;

using Twilio;
using Twilio.Rest.Notify.V1.Service;
using Twilio.Types;

using StockWatcher.Model.Schemas;

namespace StockWatcher.Model.Services
{
    public class SmsService 
    {
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

        private StockDbContext context;
        public SmsService(StockDbContext _context) {
            context = _context;
        }
        
        public NotificationResource NotifyUsers(Stock stock, double openPrice) 
        {
            var userIdentities = new List<string>();
            List<Stock> matchingStocks = context.Stocks
                .Where(s => s.RequestId == stock.RequestId)
                .ToList();
            foreach (var match in matchingStocks) 
                userIdentities.Add(match.Username);

            TwilioClient.Init(accountSid, authToken);
            NotificationResource notification = NotificationResource.Create(
                serviceSid,
                identity: userIdentities,
                body: $"${stock.Equity.ToUpper()} has exceeded target price of {stock.Price} and has reached price ${openPrice}"
            );
            return notification;
        }


    }
}