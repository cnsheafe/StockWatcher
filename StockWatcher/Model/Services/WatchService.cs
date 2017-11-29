using System;
using System.Collections.Generic;
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
    public class WatchService : IWatchService
    {
        private readonly IStockDbContext context;
        private readonly ITwilioService twilio;
        private readonly AlphaVantage alphaVantage;
        private ILimitCount limit;
        public WatchService(IStockDbContext _context, ITwilioService twilioService, AlphaVantage alpha, ILimitCount limitCount)
        {
            context = _context;
            twilio = twilioService;
            alphaVantage = alpha;
            limit = limitCount;
        }

        public bool AddRequest(Stock stock)
        {
            bool success = true;

            try
            {

                if (limit.IsOverLimit(stock.Phone))
                {
                    return false;
                }

                var request = new RequestRecord
                {
                    RequestId = stock.RequestId,
                    Price = stock.Price,
                    TwilioBinding = Guid.NewGuid().ToString()
                };

                twilio.BindUser(request.TwilioBinding, stock.Phone);

                context.Requests.Add(request);
                limit.Increment(stock.Phone);
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

        public async Task<bool> ScheduleWatch(Stock stock, string jobId)
        {
            var data = await alphaVantage.RequestStockPrices(new string[] { stock.Symbol }, TimeSeries.Intraday, IntervalTypes.OneMinute);

            DataPoint[] priceHistory;
            if (!data.TryGetValue(stock.Symbol, out priceHistory))
            {
                return false;
            }

            double latestPrice = priceHistory[0].Price;
            if (latestPrice > stock.Price)
            {
                RemoveIfExists(jobId);
                twilio.NotifyUsers(stock, latestPrice);
                RemoveRequest(stock);
            }
            else
            {
                AddOrUpdate(jobId, stock);
            }
            return true;
        }

        protected virtual void RemoveIfExists(string jobId)
        {
            RecurringJob.RemoveIfExists(jobId);
        }

        protected virtual void AddOrUpdate(string jobId, Stock stock)
        {
            RecurringJob.AddOrUpdate(jobId, () => ScheduleWatch(stock, jobId), Cron.Minutely);
        }

    }

    public interface IWatchService
    {
        bool AddRequest(Stock stock);
        bool RemoveRequest(Stock stock);
        Task<bool> ScheduleWatch(Stock stock, string jobId);
    }
}