using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using System.Text;

using Microsoft.Data.Sqlite;

using Hangfire;
using Hangfire.Storage;
using Hangfire.PostgreSql;
using Moq;
using Npgsql;
using StockWatcher.Model.Schemas;
using StockWatcher.Model.Services.Helpers;

namespace StockWatcher.Model.Services.Tests
{
    public class WatchServiceShould
    {
        private MockDbContext context;
        private ITwilioService mockTwilio;
        private AlphaVantage mockAlpha;
        private ILimitCount mockLimitCount;
        private MockWatchService mockService;
        private Stock mockStock;
        private string PHONE;

        public WatchServiceShould()
        {

            PHONE = "NEW_PHONE";
            context = new MockDbContextFactory().CreateDbContext(null);

            context.Requests.Add(new RequestRecord { Id = 0, RequestId = "abc", TwilioBinding = new Guid().ToString(), Price = 10 });

            mockTwilio = new MockTwilioService(context, "", "", "");
            mockAlpha = new MockAlphaVantageService("");
            mockLimitCount = new LimitCountService(context);

            mockService = new MockWatchService(context, mockTwilio, mockAlpha, mockLimitCount);

            mockStock = new Stock
            {
                Symbol = "msft",
                Phone = PHONE,
                Price = 1.99
            };

        }

        [Fact]
        public void ReturnTrueAfterAddingRequest()
        {

            bool status = mockService.AddRequest(mockStock);
            Assert.True(status);
        }

        [Fact]
        public void ReturnTrueAfterRemoveRequest()
        {
            ReturnTrueAfterAddingRequest();
            bool status = mockService.RemoveRequest(mockStock);
            Assert.True(status);
        }

        [Fact]
        public async Task ReturnTrueAfterQueueingRequest()
        {
            var jobId = Guid.NewGuid().ToString();
            ReturnTrueAfterAddingRequest();

            Assert.True(await mockService.ScheduleWatch(mockStock, jobId));
        }

        private class MockWatchService : WatchService
        {
            public MockWatchService(IStockDbContext context, ITwilioService twilio, AlphaVantage alpha, ILimitCount limitCount) : base(context, twilio, alpha, limitCount) { }

            protected override void AddOrUpdate(string jobId, Stock stock)
            {
                // No good way to inject Hangfire, so do nothing!
            }

            protected override void RemoveIfExists(string jobId)
            {
                // No good way to inject Hangfire, so do nothing!
            }
        }
    }
}