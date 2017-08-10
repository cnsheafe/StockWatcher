using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Hangfire;

using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

using StockWatcher.Model.Schemas;

namespace StockWatcher.Model.Actions {
    public class PollStock {
        public PollStock() { }
        // TODO: Set up a limit on polls that can be taken (e.g. 3-5)
        // TODO: Allow user to edit, delete requests

        public async Task Poll(Stock stock, string jobId) {
            var client = new HttpClient();
            string responseBody = "";
            string AV_KEY = Environment.GetEnvironmentVariable("AV_KEY");

            var queryTerms = new StringBuilder();
            queryTerms.Append("function=time_series_intraday&");
            queryTerms.Append($"symbol={stock.Equity}&");
            queryTerms.Append("interval=1min&");
            queryTerms.Append($"apikey={AV_KEY}");

            var avUri = new UriBuilder();
            avUri.Scheme = "https";
            avUri.Host = "www.alphavantage.co";
            avUri.Path = "query";
            avUri.Query = queryTerms.ToString();

            try {
                responseBody = await client.GetStringAsync(avUri.Uri);
                Console.WriteLine("Success!");
            } catch(HttpRequestException err) {
                Console.WriteLine("\n Exception Caught!");
                Console.WriteLine($"Message: {err.Message}");
            }
            client.Dispose();
            
            JObject parsedPriceHistory = JObject.Parse(responseBody);
            JToken latestPrices = 
                (JToken)parsedPriceHistory["Time Series (1min)"]
                .First
                .First;
            double openPrice = (double)latestPrices["1. open"];
            openPrice = Math.Round(openPrice, 2, MidpointRounding.ToEven);

            Console.WriteLine(latestPrices);
            Console.WriteLine(openPrice);

            // TODO: Expire when end of trading day (e.g. 6:00pm est)
            if (openPrice > stock.Price) {
                Console.WriteLine("Price reached!");
                var sms = new SmsAction();
                sms.NotifyUser(stock, openPrice);
                RecurringJob.RemoveIfExists(jobId);
                Console.WriteLine("Finished sending notification");
            }
        }
    }
}