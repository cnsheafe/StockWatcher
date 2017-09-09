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

        public SmsService(StockDbContext _context)
        {
            context = _context;
        }

        /// <summary>
        /// Notifies users via SMS about the latest stock price.
        /// </summary>
        /// <param name="stock">
        /// The particular stock whose target price was met.
        /// </param>
        /// <param name="openPrice">
        /// The latest stock price.
        /// </param>
        public NotificationResource NotifyUsers(Stock stock, double openPrice)
        {
            var userIdentities = new List<string>();
            List<Stock> matchingStocks = context.Stocks
                .Where(s => s.RequestId == stock.RequestId)
                .ToList();

            foreach (var match in matchingStocks)
            {
                var matchingUser = context.Users
                    .Single(u => u.Username == match.Username);
                userIdentities.Add(matchingUser.Uuid);
            }

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