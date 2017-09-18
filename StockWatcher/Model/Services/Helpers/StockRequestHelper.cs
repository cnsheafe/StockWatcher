using System;
using System.Collections.Generic;
using System.Linq;

using Twilio;
using Twilio.Rest.Notify.V1.Service;
using Twilio.Types;

using StockWatcher.Model.Schemas;

namespace StockWatcher.Model.Services.Helpers
{
    public class StockRequestHelper
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

        public StockRequestHelper(StockDbContext _context)
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
            List<RequestRecord> matchingStocks = context.Requests
                .Where(s => s.RequestId == stock.RequestId)
                .ToList();

            foreach (var match in matchingStocks)
            {
                userIdentities.Add(match.TwilioBinding);
            }

            TwilioClient.Init(accountSid, authToken);
            NotificationResource notification = NotificationResource.Create(
                serviceSid,
                identity: userIdentities,
                body: $"${stock.Symbol.ToUpper()} has exceeded target price of {stock.Price} and has reached price ${openPrice}"
            );

            return notification;
        }
        public BindingResource BindUser(string uuid, string phoneNumber)
        {
            TwilioClient.Init(
                Environment.GetEnvironmentVariable("TwilioAcctSid"),
                Environment.GetEnvironmentVariable("TwilioAuthToken")
            );
            var binding = BindingResource.Create(
                Environment.GetEnvironmentVariable("TwilioServiceSid"),
                identity: uuid,
                bindingType: BindingResource.BindingTypeEnum.Sms,
                address: phoneNumber
            );
            return binding;
        }

    }
}