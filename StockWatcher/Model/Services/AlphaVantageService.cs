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

        /// <summary>
        /// Sends HTTP Request(s) to AlphaVantage API and returns a dictionary
        /// with stock symbol and its associated price history.
        /// </summary>
        public async Task<Dictionary<string, DataPoint[]>> RequestStockPrices(
            string[] symbols, TimeSeries timeSeries, IntervalTypes interval
        )
        {
            var responseBodies = new List<Task<string>>();
            var stockPrices = new Dictionary<string, DataPoint[]> { };
            foreach (string symbol in symbols)
            {
                Uri uri = BuildUri(timeSeries, symbol);
                responseBodies.Add(FetchStockHistory(uri));
            }

            string[] allResponses = await Task.WhenAll(responseBodies);
            for (int i = 0; i < allResponses.Length; i++)
            {
                DataPoint[] data = ParseTimeSeriesData(allResponses[i], timeSeries);
                stockPrices.Add(symbols[i], data);
            }
            return stockPrices;
        }

        /// <summary>
        /// Makes HTTP Request to provide URI.
        /// </summary>
        protected virtual async Task<string> FetchStockHistory(Uri uri)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    string responseBody = await client.GetStringAsync(uri);
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

        /// <summary>
        /// Custom parser for AlphaVantage API response body.
        /// </summary>
        protected DataPoint[] ParseTimeSeriesData(
            string dataString, TimeSeries timeSeries, IntervalTypes interval = IntervalTypes.OneMinute
        )
        {
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
                    .Parse(dataString)[propertyString]
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
        /// <summary>
        /// Create formatted URI for AlphaVantage API Time-series call.
        /// </summary>
        protected Uri BuildUri(
            TimeSeries timeSeriesType, string symbol,
            IntervalTypes interval = IntervalTypes.OneMinute
        )
        {
            var queryTerms = new StringBuilder();
            switch (timeSeriesType)
            {
                case TimeSeries.Intraday:
                    queryTerms = BuildQuery(
                        "time_series_intraday", symbol, (int)interval
                    );
                    break;

                case TimeSeries.Daily:
                    queryTerms = BuildQuery("time_series_daily", symbol);
                    break;
                case TimeSeries.Weekly:
                    queryTerms = BuildQuery("time_series_weekly", symbol);
                    break;
                case TimeSeries.Monthly:
                    queryTerms = BuildQuery("time_series_monthly", symbol);
                    break;
            }

            var avUri = new UriBuilder();
            avUri.Scheme = "https";
            avUri.Host = "www.alphavantage.co";
            avUri.Path = "query";
            avUri.Query = queryTerms.ToString();
            return avUri.Uri;
        }
        /// <summary>
        /// Constructs the query terms for AlphaVantage API.
        /// </summary>
        protected StringBuilder BuildQuery(
            string functionType, string symbol, int interval = 0
        )
        {
            var queryTerms = new StringBuilder();
            queryTerms.Append($"function={functionType}&");
            queryTerms.Append($"symbol={symbol.ToLower()}&");
            queryTerms.Append($"apikey={APIKEY}");

            if (interval > 0)
            {
                queryTerms.Append($"&interval={interval}min");
            }
            return queryTerms;
        }

    }
}

/// <summary>
/// Represents an interface for using the
/// the AlphaVantage API.
/// </summary>
public interface AlphaVantage
{
    string APIKEY { get; }
    Task<Dictionary<string, DataPoint[]>> RequestStockPrices(
        string[] symbol, TimeSeries type, IntervalTypes interval
    );
}

/// <summary>
/// Represents options available for the AlphaVantage API Time-series.
/// </summary>
public enum TimeSeries { Intraday, Daily, Weekly, Monthly }

/// <summary>
/// Represents interval options when using AlphaVantage API Intraday
/// Time-Series.
/// </summary>
public enum IntervalTypes
{
    OneMinute = 1,
    FiveMinutes = 5,
    FifteenMinute = 15,
    ThirtyMinute = 30,
    SixtyMinute = 60
}
