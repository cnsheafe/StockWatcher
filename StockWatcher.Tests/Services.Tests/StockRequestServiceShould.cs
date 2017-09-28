using System;
using System.Threading.Tasks;
using Xunit;

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
    public class StockRequestServiceShould
    {
        private MockDbContext context;
        private IStockRequestService mockService;
        private Stock mockStock;

        private string DB_URL;
        private string PHONE;

        public StockRequestServiceShould()
        {

            DB_URL = Environment.GetEnvironmentVariable("DATABASE_URL");
            PHONE = Environment.GetEnvironmentVariable("Phone");
            context = new MockDbContextFactory().CreateDbContext(null);
            mockService = new StockRequestService(context);

            mockStock = new Stock
            {
                Symbol="msft",
                Phone= PHONE,
                Price=1.99
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

            // Use offsite db for hangfire
            var storage = new PostgreSqlStorage(
                ConnectionStringParser.Parse(DB_URL)
            );
            JobStorage.Current = storage;

            Assert.True(await mockService.QueryStock(mockStock, jobId));
        }

    }
}