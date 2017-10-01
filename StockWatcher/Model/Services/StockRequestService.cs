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
    public class StockRequestService : IStockRequestService
    {
        private readonly IStockDbContext context;
        private StockRequestHelper helper;
        private LimitCountChecker checker;
        public StockRequestService(IStockDbContext _context)
        {
            context = _context;
            helper = new StockRequestHelper(_context);
            checker = new LimitCountChecker(_context);
        }
        
        public bool AddRequest(Stock stock)
        {
            bool success = true;

            try
            {

                if(checker.IsOverLimit(stock.Phone))
                {
                    return false;
                }

                var request = new RequestRecord
                {
                    RequestId = stock.RequestId,
                    Price = stock.Price,
                    TwilioBinding = Guid.NewGuid().ToString()
                };

                helper.BindUser(request.TwilioBinding, stock.Phone);

                context.Requests.Add(request);
                checker.Increment(stock.Phone);
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

        public async Task<Boolean> QueryStock(Stock stock, string jobId)
        {
            if (String.Compare(jobId,"no_id") == 0)
            {
                jobId = Guid.NewGuid().ToString();
            }
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
                    return true;
                }
                else
                {
                    RecurringJob.AddOrUpdate(jobId, () => QueryStock(stock, jobId), Cron.Minutely);
                }
                return false;
            }
        }
        private static bool IsOpenHours()
        {
            DateTime now = DateTime.Now;
            DateTime endOfDay = DateTime.Today.AddHours(18);
            return DateTime.Compare(now, endOfDay) < 0 ? true : false;
        }
    }

    public interface IStockRequestService
    {
        bool AddRequest(Stock stock);
        bool RemoveRequest(Stock stock);
        Task<Boolean> QueryStock(Stock stock, string jobId);
    }
}