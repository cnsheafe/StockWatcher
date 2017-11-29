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
    /// <summary>
    /// Manages Watches set by users.
    /// </summary>
    public class WatchService : IWatchService
    {
        private readonly IStockDbContext context;
        private readonly ITwilioService twilio;
        private readonly AlphaVantage alphaVantage;
        private ILimitCount limit;

        /// <summary>
        /// Construct service with dependencies from database and other services.
        /// </summary>
        /// <param name="_context">
        /// Database context.
        /// </param>
        /// <param name="twilioService">
        /// Service for accessing SMS.
        /// </param>
        /// <param name="alpha">
        /// Service for getting prices.
        /// </param>
        /// <param name="limitCount">
        /// Service for limiting requests made by users.
        /// </param>
        public WatchService(IStockDbContext _context, ITwilioService twilioService, AlphaVantage alpha, ILimitCount limitCount)
        {
            context = _context;
            twilio = twilioService;
            alphaVantage = alpha;
            limit = limitCount;
        }

        /// <summary>
        /// Adds a request for a particular stock to the database.
        /// </summary>
        /// <param name="stock">
        /// Stock info such as price and phone number to message.
        /// </param>
        /// <returns>
        /// True if adding the request was successful, otherwise false.
        /// </returns>
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

        /// <summary>
        /// Removes a request from the database
        /// </summary>
        /// <param name="stock">
        /// Stock used to find the particular request.
        /// </param>
        /// <returns>
        /// True is removal was successful, otherwise false.
        /// </returns>
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

        /// <summary>
        /// Checks if the price has exceeded the targeted price. If not, the task is added to the Hangfire scheduler and executed every minute.
        /// </summary>
        /// <param name="stock">
        /// Stock to watch.
        /// </param>
        /// <param name="jobId">
        /// Hangfire task id. Used to find the correct task when it needs to be removed.
        /// </param>
        /// <returns>
        /// True if the target price is met, otherwise false.
        /// </returns>
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

        /// <summary>
        /// Removes the Hangfire task if it exists.
        /// </summary>
        /// <param name="jobId">
        /// Hangfire task id.
        /// </param>
        protected virtual void RemoveIfExists(string jobId)
        {
            RecurringJob.RemoveIfExists(jobId);
        }

        /// <summary>
        /// Adds a Hangfire task, if it doesn't already exist.
        /// </summary>
        /// <param name="jobId">
        /// Hangfire task id.
        /// </param>
        /// <param name="stock">
        /// Stock info
        /// </param>
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