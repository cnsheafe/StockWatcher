using System;
using System.Collections.Generic;
using System.Linq;

using Twilio;
using Twilio.Rest.Notify.V1.Service;
using Twilio.Types;

using StockWatcher.Model.Schemas;

namespace StockWatcher.Model.Services
{
    /// <summary>
    /// Manages SMS Messaging via the Twilio Notifications API.
    /// </summary>
    public class TwilioService : ITwilioService
    {
        private readonly string accountSid;
        private readonly string authToken;
        private readonly string serviceSid;


        private readonly IStockDbContext context;

        /// <summary>
        /// Initialize the service with database and Twilio credentials
        /// </summary>
        /// <param name="_context">
        /// Database context for checking user records.
        /// </param>
        /// <param name="ACCT_SID">
        /// Account Service ID for API.
        /// </param>
        /// <param name="AUTH_TOKEN">
        /// Authentication Token for API.
        /// </param>
        /// <param name="SRV_SID">
        /// Service ID for Notifications.
        /// </param>
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
        /// <param name="latestPrice">
        /// The latest stock price.
        /// </param>
        /// <returns>
        /// Returns a NotificationResource with info on the request. Not interesting.
        /// </returns>
        public virtual NotificationResource NotifyUsers(Stock stock, double latestPrice)
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

        /// <summary>
        /// Creates a binding between a phone number and uuid that is then stored on Twilio's servers for later messaging.
        /// </summary>
        /// <param name="uuid">
        /// Random generated user id.
        /// </param>
        /// <param name="phoneNumber">
        /// U.S. phone number(e.g. +15551234567)
        /// </param>
        /// <returns>
        /// BindingResource with info. Not particularly interesting.
        /// </returns>
        public virtual BindingResource BindUser(string uuid, string phoneNumber)
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

    public interface ITwilioService
    {
        NotificationResource NotifyUsers(Stock stock, double latestPrice);
        BindingResource BindUser(string uuid, string phoneNumber);
    }
}