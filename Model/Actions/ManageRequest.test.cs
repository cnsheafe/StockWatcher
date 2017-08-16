using System;

using Hangfire;
using Xunit;

using StockWatcher.Model.Actions;
using StockWatcher.Model.Schemas;

namespace StockWatcher.Tests {
    public class ManageRequestTest {
        public ManageRequestTest() { }
        [Fact]
        public void AddandRemove() {
            // GIVEN a Stock
            // WITH valid entries
            var stock = new Stock() {
                Username="dummy",
                Equity="msft",
                Price=1.1
            };
            // IT should add a request 
            Assert.True(ManageRequest.Add(stock));
            // IT should NOT add a request if one already exists
            Assert.False(ManageRequest.Add(stock));
            // IT should remove the request
            Assert.True(ManageRequest.Remove(stock.RequestId));
        }
    }
}