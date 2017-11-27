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
        private WatchService mockService;
        private Stock mockStock;

        private string DB_URL;
        private string PHONE;

        public WatchServiceShould()
        {

            PHONE = "NEW_PHONE";
            context = new MockDbContextFactory().CreateDbContext(null);

            context.Requests.Add(new RequestRecord { Id = 0, RequestId = "abc", TwilioBinding = new Guid().ToString(), Price = 10 });

            mockService = new MockWatchService(context, "", "", "", "");

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
            public MockWatchService(IStockDbContext _context, string AVKEY, string ACCT_SID, string AUTH_TOKEN, string SRV_SID) : base(_context, AVKEY, ACCT_SID, AUTH_TOKEN, SRV_SID)
            {
            }

            protected override bool IsOverLimit(string phone)
            {
                if (String.Compare(phone, "NEW_PHONE") == 0)
                {
                    return false;
                }
                return true;
            }

            protected override void BindUser(string twilioBinding, string phone)
            {
                // Do Nothing!
            }

            protected override void NotifyUsers(Stock stock, double latestPrice)
            {
                //Do Nothing!
            }

            protected override void IncrementCount(string phone)
            {
                // Do NOthing!
            }
            protected override async Task<Dictionary<string, DataPoint[]>> FetchStockPrices(string[] symbols, TimeSeries timeSeries, IntervalTypes interval)
            {
                var mockDict = new Dictionary<string, DataPoint[]>();
                mockDict.Add("msft", new DataPoint[] { new DataPoint { TimeStamp = "10:00", Price = 10 } });
                return mockDict;
            }

            protected override void AddOrUpdate(string jobId, Stock stock)
            {
                // do nothing!
            }

            protected override void RemoveIfExists(string jobId) {
                // do nothing!
            }


        }
    }
}