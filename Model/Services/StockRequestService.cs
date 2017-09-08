using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Hangfire;

using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

using StockWatcher.Model.Schemas;

namespace StockWatcher.Model.Services
{
    public class StockRequestService
    {
        private readonly StockDbContext context;
        public StockRequestService(StockDbContext _context)
        {
            context = _context;
        }
        public bool AddRequest(Stock stock)
        {
            bool success = true;

            try
            {
                context.Add(stock);
                context.SaveChanges();
            }
            catch (DbUpdateException dbException)
            {
                var exception = (Npgsql.PostgresException)dbException.InnerException;
                Console.WriteLine(exception.SqlState);
                success = false;
            }
            return success;
        }

        public bool RemoveRequest(Stock stock)
        {
            bool success = true;

            try
            {
                context.Stocks.RemoveRange(
                    context.Stocks
                        .Where(s => s.Username == stock.Username)
                );
                context.SaveChanges();
            }
            catch (DbUpdateException dbException)
            {
                var exception = (Npgsql.PostgresException)dbException.InnerException;
                Console.WriteLine(exception.SqlState);
                success = false;
            }
            return success;
        }

        public async Task QueryStock(Stock stock, string jobId)
        {
            using (var client = new HttpClient())
            {
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
                try
                {
                    responseBody = await client.GetStringAsync(avUri.Uri);
                    Console.WriteLine("Success!");
                }
                catch (HttpRequestException err)
                {
                    Console.WriteLine("\n Exception Caught!");
                    Console.WriteLine($"Message: {err.Message}");
                }

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
                if (openPrice > stock.Price)
                {
                    new SmsService(context).NotifyUsers(stock, openPrice);
                    RecurringJob.RemoveIfExists(jobId);
                    RemoveRequest(stock);
                    Console.WriteLine("Finished sending notification");
                }
            }
        }
        private static bool IsOpenHours()
        {
            DateTime now = DateTime.Now;
            DateTime endOfDay = DateTime.Today.AddHours(18);
            return DateTime.Compare(now, endOfDay) < 0 ? true : false;
        }

    }
}