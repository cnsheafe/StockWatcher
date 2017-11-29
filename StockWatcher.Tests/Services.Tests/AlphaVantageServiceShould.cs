using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;



using StockWatcher.Model.Schemas;

namespace StockWatcher.Model.Services.Tests
{
    public class AlphaVantageServiceShould
    {
        [Fact]
        public async Task FetchStockHistory()
        {
            var service = new MockAlphaVantageService("SOME_KEY");
            Dictionary<string, DataPoint[]> data = await service.RequestStockPrices(new string[] { "msft" }, TimeSeries.Intraday, IntervalTypes.OneMinute);

            Assert.True(data.ContainsKey("msft"));
            Assert.NotNull(data["msft"]);
            Assert.NotEmpty(data["msft"]);
        }
    }
    public class MockAlphaVantageService : AlphaVantageService
    {
        public MockAlphaVantageService(string mockKey) : base(mockKey)
        {
        }
        protected override async Task<string> FetchStockHistory(Uri uri)
        {
            return await File.ReadAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), "../assets/sample-intraday.json"));
        }
    }

}
