using System;

using Twilio;
using Twilio.Rest.Notify.V1.Service;
using Twilio.Types;

namespace StockWatcher.Model.Services.Tests
{
    public class TwilioServiceShould
    {
        // Methods require network calls
        // Possibly divide up further in the future
    }
    public class MockTwilioService : TwilioService
    {
        public MockTwilioService(IStockDbContext context, string ACCT_SID, string AUTH_TOKEN, string SRV_SID) : base(context, ACCT_SID, AUTH_TOKEN, SRV_SID)
        {
        }

        public override BindingResource BindUser(string uuid, string phoneNumber)
        {
            return null;
        }

        public override NotificationResource NotifyUsers(Schemas.Stock stock, double latestPrice)
        {
            return null;
        }
    }
}