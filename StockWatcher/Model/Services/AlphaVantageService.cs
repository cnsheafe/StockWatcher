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
    public class AlphaVantageService : AlphaVantage
    {

        public string APIKEY { get; }
        public AlphaVantageService(string AVKey)
        {
            APIKEY = AVKey;
        }

        public async Task<string> FetchStockHistory(
            string symbol,
            TimeSeries timeSeries = TimeSeries.Intraday,
            IntervalTypes interval = IntervalTypes.OneMinute
        )
        {
            UriBuilder avUri = new UriBuilder();
            switch (timeSeries)
            {
                case TimeSeries.Intraday:
                    avUri = AssembleUri(
                        "time_series_intraday", symbol, true, interval
                    );
                    break;

                case TimeSeries.Daily:
                    avUri = AssembleUri(
                        "time_series_daily", symbol
                    );
                    break;
                case TimeSeries.Weekly:
                    avUri = AssembleUri("time_series_weekly", symbol);
                    break;
                case TimeSeries.Monthly:
                    avUri = AssembleUri("time_series_monthly", symbol);
                    break;
            }

            using (var client = new HttpClient())
            {
                try
                {
                    string responseBody = await client.GetStringAsync(avUri.Uri);
                    return responseBody;
                }
                catch (HttpRequestException err)
                {
                    Console.WriteLine("\n Exception Caught!");
                    Console.WriteLine($"Message: {err.Message}");
                    return null;
                }
            }
        }

        public async Task<IEnumerable<DataPoint>> RequestStockPrice(
            string symbol, TimeSeries timeSeries, IntervalTypes interval
        )
        {
            string responseBody = await FetchStockHistory(
                symbol, timeSeries, interval
            );
            string propertyString = "";
            switch (timeSeries)
            {
                case TimeSeries.Intraday:
                    propertyString = $"Time Series ({(int)interval}min)";
                    break;
                case TimeSeries.Daily:
                    propertyString = "Time Series (Daily)";
                    break;
                case TimeSeries.Weekly:
                    propertyString = "Weekly Time Series";
                    break;
                case TimeSeries.Monthly:
                    propertyString = "Monthly Time Series";
                    break;
            }

            var parsedResponse = new JObject();
            try
            {
                parsedResponse =
                JObject
                    .Parse(responseBody)[propertyString]
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
                .Select(series => series.Children().First())
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
        protected UriBuilder AssembleUri(
            string functionType, string symbol,
            bool isIntraday = false,
            IntervalTypes interval = IntervalTypes.OneMinute
        )
        {
            var queryTerms = new StringBuilder();
            queryTerms.Append($"function={functionType}&");
            queryTerms.Append($"symbol={symbol.ToLower()}&");
            queryTerms.Append($"apikey={APIKEY}");

            if (isIntraday)
            {
                queryTerms.Append($"&interval={(int)interval}min");
            }

            var avUri = new UriBuilder();
            avUri.Scheme = "https";
            avUri.Host = "www.alphavantage.co";
            avUri.Path = "query";
            avUri.Query = queryTerms.ToString();
            return avUri;
        }

    }
}
public interface AlphaVantage
{
    string APIKEY { get; }
    Task<IEnumerable<DataPoint>> RequestStockPrice(
        string symbol, TimeSeries type, IntervalTypes interval
    );
    Task<string> FetchStockHistory(
        string symbol,
        TimeSeries timeSeries,
        IntervalTypes interval
    );
}
public enum TimeSeries { Intraday, Daily, Weekly, Monthly }

public enum IntervalTypes
{
    OneMinute = 1,
    FiveMinutes = 5,
    FifteenMinute = 15,
    ThirtyMinute = 30,
    SixtyMinute = 60
}
