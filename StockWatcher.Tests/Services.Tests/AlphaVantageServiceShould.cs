using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

using StockWatcher.Model.Schemas;

namespace StockWatcher.Model.Services.Tests
{
    public class AlphaVantageServiceShould
    {
        [Fact]
        public async Task ReturnTimeSeriesData()
        {
            var service = new AlphaVantageService();
            IEnumerable<DataPoint> data = await service.RequestStockPrice("msft");
            Assert.NotNull(data);
            Assert.NotEmpty(data);
        }
    }
}
