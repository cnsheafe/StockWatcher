using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

using StockWatcher.Model.Schemas;


namespace StockWatcher.Model.Services
{
    public class AlphaVantageService : IAlphaVantageService
    {
        public async Task<IEnumerable<DataPoint>> RequestStockPrice(string stockSymbol)
        {
            using (var client = new HttpClient())
            {
                string responseBody = "";
                string AV_KEY = Environment.GetEnvironmentVariable("AV_KEY");

                // Describes URI query terms for Alphavantage API
                var queryTerms = new StringBuilder();
                queryTerms.Append("function=time_series_daily&");
                queryTerms.Append($"symbol={stockSymbol.ToLower()}&");
                queryTerms.Append("interval=1min&");
                queryTerms.Append($"apikey={AV_KEY}");

                var avUri = new UriBuilder();
                avUri.Scheme = "https";
                avUri.Host = "www.alphavantage.co";
                avUri.Path = "query";
                avUri.Query = queryTerms.ToString();

                // Asynchronous HTTP call
                try
                {
                    responseBody = await client.GetStringAsync(avUri.Uri);
                    Console.WriteLine("Success!");
                }
                catch (HttpRequestException err)
                {
                    Console.WriteLine("\n Exception Caught!");
                    Console.WriteLine($"Message: {err.Message}");
                    return null;
                }

                var parsedResponse = new JObject();
                try
                {
                    parsedResponse = JObject.Parse(responseBody)["Time Series (Daily)"]
                        .Value<JObject>();
                }
                catch (System.Exception e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
                var timestamps = parsedResponse.Properties()
                    .Select(p => p.Name)
                    .ToArray();

                var priceHistory = parsedResponse.Children()
                    .Select(timeSeries => timeSeries.Children().First())
                    .Select(price => (double)price["1. open"])
                    .ToArray();

                var history = new DataPoint[priceHistory.Count()];
                for (int i = 0; i < timestamps.Length; i++)
                {
                    history[i] = (new DataPoint()
                    {
                        TimeStamp = timestamps[i],
                        Price = priceHistory[i]
                    });
                }

                return history;
            }
        }
    }
    public interface IAlphaVantageService
    {
        Task<IEnumerable<DataPoint>> RequestStockPrice(string StockSymbol);

    }

}