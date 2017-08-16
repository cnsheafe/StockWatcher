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
using StockWatcher.Model.Data;

namespace StockWatcher.Model.Actions {
    public class PollStock {
        public PollStock() { }
        // TODO: Set up a limit on polls that can be taken (e.g. 3-5)
        // TODO: Allow user to edit, delete requests
        /// <summary>
        /// Asynchronously queries intraday price and sends SMS notification
        /// if taret price was exceeded.
        /// </summary>
        /// <param name="stock">
        /// Particular stock requested from some username.
        /// </param>
        /// <param name="jobId">
        /// Randomly assigned id for particular hangfire process.
        /// </param>
        /// <returns>
        /// Nothing.
        /// </returns>
        public async Task Poll(Stock stock, string jobId) {
            //TODO: Add twilio notification for expired requests
            if (!IsOpenHours()) {
                RecurringJob.RemoveIfExists(jobId);
                var request = new StockRequestDb();
                request.Remove(stock.RequestId);
                Console.WriteLine("Its the end of the day!");
                return;
            }
            
            var client = new HttpClient();
            string responseBody = "";
            string AV_KEY = Environment.GetEnvironmentVariable("AV_KEY");

            // Describes URI query terms for Alphavantage API
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

            // Asynchronous HTTP call
            try {
                responseBody = await client.GetStringAsync(avUri.Uri);
                Console.WriteLine("Success!");
            } catch(HttpRequestException err) {
                Console.WriteLine("\n Exception Caught!");
                Console.WriteLine($"Message: {err.Message}");
            }
            client.Dispose();
            
            // Find the Open price for the most recent stock price
            JObject parsedPriceHistory = JObject.Parse(responseBody);
            JToken latestPrices = 
                (JToken)parsedPriceHistory["Time Series (1min)"]
                .First
                .First;
            double openPrice = (double)latestPrices["1. open"];
            openPrice = Math.Round(openPrice, 2, MidpointRounding.ToEven);

            Console.WriteLine(openPrice);

            // TODO: Expire when end of trading day (e.g. 6:00pm est)
            // Sends notification to users if stock target price was met
            if (openPrice > stock.Price) {
                var request = new StockRequestDb();
                List<string> users = request.GetUsers(stock.RequestId);
                Console.WriteLine("Price reached!");
                var sms = new SmsAction();
                sms.NotifyUsers(users, stock, openPrice);
                RecurringJob.RemoveIfExists(jobId);
                request.Remove(stock.RequestId);
                Console.WriteLine("Finished sending notification");
            }
        }

        private static bool IsOpenHours() {
            DateTime now = DateTime.Now; 
            DateTime endOfDay = DateTime.Today.AddHours(18);
            return DateTime.Compare(now, endOfDay) < 0 ? true : false;
        }
    }
}