using System.Collections.Generic;
using Xunit;

using StockWatcher.Model.Schemas;


namespace StockWatcher.Model.Services.Tests
{
    public class LimitCountServiceShould
    {
        [Fact]
        public void ReturnTrueForAddingNewEntry()
        {
            var mockLimitCount = createNewServiceInstance();
            Assert.True(mockLimitCount.AddEntry("abc"));
        }

        [Fact]
        public void ReturnFalseForTryingToAddAnExistingEntry()
        {
            var mockLimitCount = createNewServiceInstance();
            mockLimitCount.AddEntry("abc");
            Assert.False(mockLimitCount.IsNewEntry("abc"));
        }

        [Fact]
        public void ReturnFalseIfNotOverLimit()
        {
            var mockLimitCount = createNewServiceInstance();
            mockLimitCount.AddEntry("abc");
            Assert.False(mockLimitCount.IsOverLimit("abc"));
        }

        [Fact]
        public void ReturnTrueOnSuccessfulIncrement()
        {
            var mockLimitCount = createNewServiceInstance();
            mockLimitCount.AddEntry("abc");
            Assert.True(mockLimitCount.Increment("abc"));
        }

        [Fact]
        public void ReturnFalseIfNotExpired()
        {
            var mockLimitCount = createNewServiceInstance();
            mockLimitCount.AddEntry("abc");
            Assert.False(mockLimitCount.IsExpired("abc"));
        }

        private ILimitCount createNewServiceInstance()
        {
            var dbContext = new MockDbContextFactory().CreateDbContext(null);

            return new LimitCountService(dbContext);

        }

    }
}