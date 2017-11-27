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
    public class WatchService : AlphaVantageService, IWatchService
    {
        private readonly IStockDbContext context;
        private ITwilioService twilio;
        private LimitCountChecker checker;
        public WatchService(IStockDbContext _context, string AVKEY, string ACCT_SID, string AUTH_TOKEN, string SRV_SID) : base(AVKEY)
        {
            context = _context;
            twilio = new TwilioService(_context, ACCT_SID, AUTH_TOKEN, SRV_SID);
            checker = new LimitCountChecker(_context);
        }

        public bool AddRequest(Stock stock)
        {
            bool success = true;

            try
            {

                if (IsOverLimit(stock.Phone))
                {
                    return false;
                }

                var request = new RequestRecord
                {
                    RequestId = stock.RequestId,
                    Price = stock.Price,
                    TwilioBinding = Guid.NewGuid().ToString()
                };

                BindUser(request.TwilioBinding, stock.Phone);

                context.Requests.Add(request);
                IncrementCount(stock.Phone);
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
            var data = await FetchStockPrices(new string[] { stock.Symbol }, TimeSeries.Intraday, IntervalTypes.OneMinute);

            DataPoint[] priceHistory;
            if (!data.TryGetValue(stock.Symbol, out priceHistory))
            {
                return false;
            }

            double latestPrice = priceHistory[0].Price;
            if (latestPrice > stock.Price)
            {
                RemoveIfExists(jobId);
                NotifyUsers(stock, latestPrice);
                RemoveRequest(stock);
            }
            else
            {
                // RecurringJob.AddOrUpdate(jobId, () => ScheduleWatch(stock, jobId), Cron.Minutely);
                AddOrUpdate(jobId, stock);
            }
            return true;
        }
        protected virtual bool IsOverLimit(string phone)
        {
            return checker.IsOverLimit(phone);
        }

        protected virtual void BindUser(string twilioBinding, string phone)
        {
            twilio.BindUser(twilioBinding, phone);
        }

        protected virtual void IncrementCount(string phone)
        {
            checker.Increment(phone);
        }

        protected virtual void NotifyUsers(Stock stock, double latestPrice)
        {
            twilio.NotifyUsers(stock, latestPrice);
        }

        protected virtual async Task<Dictionary<string, DataPoint[]>> FetchStockPrices(string[] symbols, TimeSeries timeSeries, IntervalTypes interval
        )
        {
            return await RequestStockPrices(symbols, timeSeries, interval);

        }

        protected virtual void RemoveIfExists(string jobId) {
            RecurringJob.RemoveIfExists(jobId);
        }

        protected virtual void AddOrUpdate(string jobId, Stock stock) {
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