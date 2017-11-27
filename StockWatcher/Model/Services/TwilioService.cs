using System;
using System.Collections.Generic;
using System.Linq;

using Twilio;
using Twilio.Rest.Notify.V1.Service;
using Twilio.Types;

using StockWatcher.Model.Schemas;

namespace StockWatcher.Model.Services
{
    public class TwilioService : ITwilioService
    {
        private readonly string accountSid;
        private readonly string authToken;
        private readonly string serviceSid;


        private readonly IStockDbContext context;

        public TwilioService(IStockDbContext _context, string ACCT_SID, string AUTH_TOKEN, string SRV_SID)
        {
            context = _context;
            accountSid = ACCT_SID;
            authToken = AUTH_TOKEN;
            serviceSid = SRV_SID;
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
        public NotificationResource NotifyUsers(Stock stock, double latestPrice)
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
                body: $"${stock.Symbol.ToUpper()} has exceeded target price of {stock.Price} and has reached price ${latestPrice}"
            );

            return notification;
        }
        public BindingResource BindUser(string uuid, string phoneNumber)
        {
            TwilioClient.Init(
                accountSid,
                authToken
            );
            var binding = BindingResource.Create(
                serviceSid,
                identity: uuid,
                bindingType: BindingResource.BindingTypeEnum.Sms,
                address: phoneNumber
            );
            return binding;
        }
    }

    public interface ITwilioService {
        NotificationResource NotifyUsers(Stock stock, double latestPrice);
        BindingResource BindUser(string uuid, string phoneNumber);
    }
}