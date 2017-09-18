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
using StockWatcher.Model.Services.Helpers;

namespace StockWatcher.Model.Services
{
    public class StockRequestService
    {
        private readonly StockDbContext context;
        private StockRequestHelper helper;
        public StockRequestService(StockDbContext _context)
        {
            context = _context;
            helper = new StockRequestHelper(_context);
        }
        public bool AddRequest(Stock stock)
        {
            bool success = true;

            try
            {
                var request = new RequestRecord();
                request.RequestId = stock.RequestId;
                request.Price = stock.Price;
                request.TwilioBinding = Guid.NewGuid().ToString();
                helper.BindUser(request.TwilioBinding, stock.Phone);

                context.Add(request);
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
                context.Requests.RemoveRange(
                    context.Requests
                        .Where(r => r.RequestId == stock.RequestId)
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
                queryTerms.Append("function=time_series_daily&");
                queryTerms.Append($"symbol={stock.Symbol}&");
                // queryTerms.Append("interval=1min&");
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
                    (JToken)parsedPriceHistory["Time Series (Daily)"]
                    .First
                    .First;
                double openPrice = (double)latestPrices["1. open"];
                openPrice = Math.Round(openPrice, 2, MidpointRounding.ToEven);

                Console.WriteLine(openPrice);

                // TODO: Expire when end of trading day (e.g. 6:00pm est)
                // Sends notification to users if stock target price was met
                if (openPrice > stock.Price)
                {
                    new StockRequestHelper(context).NotifyUsers(stock, openPrice);
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