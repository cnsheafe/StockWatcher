using System;
using System.Threading.Tasks;
using Xunit;

using Hangfire;
using Hangfire.PostgreSql;
using Npgsql;
using StockWatcher.Model.Schemas;
using StockWatcher.Model.Services.Helpers;

namespace StockWatcher.Model.Services.Tests
{
    public class StockRequestServiceShould
    {
        private StockDbContext context;
        private Stock mockStock;

        private string DB_URL;

        public StockRequestServiceShould()
        {
            DB_URL = Environment.GetEnvironmentVariable("DATABASE_URL");
            string[] args = {DB_URL};
            context = new StockDbContextFactory().CreateDbContext(args);
            mockStock = new Stock
            {
                Symbol="msft",
                Phone="+17193607639",
                Price=1.99
            };
        }

        [Fact]
        public void ReturnTrueAfterAddingRequest()
        {
            var service = new StockRequestService(context);
            bool status = service.AddRequest(mockStock);
            Assert.True(status);
        }

        [Fact]
        public void ReturnTrueAfterRemoveRequest()
        {
            ReturnTrueAfterAddingRequest();
            var service = new StockRequestService(context);
            bool status = service.RemoveRequest(mockStock);
            Assert.True(status);
        }

        [Fact]
        public async Task ReturnTrueAfterQueueingRequest()
        {
            var jobId = Guid.NewGuid().ToString();
            ReturnTrueAfterAddingRequest();
            var storage = new PostgreSqlStorage(
                ConnectionStringParser.Parse(DB_URL)
            );
            JobStorage.Current = storage;

            var service = new StockRequestService(context);
            Assert.True(await service.QueryStock(mockStock, jobId));
        }

    }
}