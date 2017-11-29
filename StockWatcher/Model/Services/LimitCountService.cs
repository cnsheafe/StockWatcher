using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using BCrypt.Net;


using StockWatcher.Model.Schemas;

namespace StockWatcher.Model.Services
{
    /// <summary>
    /// Manages the number of watches a user can set.
    /// </summary>
    public class LimitCountService : ILimitCount
    {
        private IStockDbContext context;
        private int limit = 5;
        public LimitCountService(IStockDbContext _context)
        {
            context = _context;
        }

        /// <summary>
        /// Checks to see if record is available to set.
        /// </summary>
        /// <param name="phone">
        /// U.S. phone number (e.g. +15551234567)
        /// </param>
        /// <returns>True if record is not set, otherwise false.</returns>
        public bool IsNewEntry(string phone)
        {
            try
            {
                context.LimitCounts
                    .Single(lc => VerifyHash(phone, lc.PhoneHash));
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds a record for a user's request.
        /// </summary>
        /// <param name="phone">
        /// U.S. phone number (e.g. +15551234567)
        /// </param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool AddEntry(string phone)
        {
            var newEntry = new LimitCount
            {
                PhoneHash = MakeHash(phone),
                Count = 0,
                Date = DateTime.UtcNow.ToString()
            };

            try
            {
                context.LimitCounts.Add(newEntry);
                context.SaveChanges();
                return true;
            }
            catch (DbUpdateException dbException)
            {
                var exception = (Npgsql.PostgresException)dbException.InnerException;
                Console.WriteLine(exception.Message);
                return false;
            }
        }

        /// <summary>
        /// Checks to see if the user has reached the max number of requests.
        /// </summary>
        /// <param name="phone">
        /// U.S. phone number (e.g. +15551234567)
        /// </param>
        /// <returns>True if limit is reached, otherwise false.</returns>
        public bool IsOverLimit(string phone)
        {
            if (IsNewEntry(phone))
            {
                AddEntry(phone);
                Increment(phone);
                return false;
            }
            // Resets Count if a day has passed
            IsExpired(phone);

            int count = context.LimitCounts
                .Single(lc => VerifyHash(phone, lc.PhoneHash))
                .Count;

            return (count > limit);
        }

        /// <summary>
        /// Increments count on user record.
        /// </summary>
        /// <param name="phone">
        /// U.S. phone number (e.g. +15551234567)
        /// </param>
        /// <returns>True if increment is successful, otherwise false.</returns>
        public bool Increment(string phone)
        {
            try
            {
                context.LimitCounts
                    .Single(lc => VerifyHash(phone, lc.PhoneHash))
                    .Count++;
                context.SaveChanges();

                return true;
            }

            catch (DbUpdateException dbException)
            {
                var exception = (Npgsql.PostgresException)dbException.InnerException;
                Console.WriteLine(exception.SqlState);

                return false;
            }
        }

        /// <summary>
        /// Checks to see if record has expired.
        /// </summary>
        /// <param name="phone">
        /// U.S. phone number (e.g. +15551234567)
        /// </param>
        /// <returns>True if record has expired, otherwise false.</returns>
        public bool IsExpired(string phone)
        {
            try
            {
                var limitCount = context.LimitCounts
                    .Single(lc => VerifyHash(phone, lc.PhoneHash));
                var oldDate = DateTime.Parse(limitCount.Date);
                var currentDate = DateTime.UtcNow;

                if (currentDate.Subtract(oldDate).Days > 0)
                {
                    limitCount.Count = 0;
                    limitCount.Date = currentDate.ToString();
                    context.SaveChanges();
                    return true;
                }
            }
            catch (DbUpdateException dbException)
            {
                Console.WriteLine(dbException.Message);
                return false;
            }
            return false;
        }

        /// <summary>
        /// Hashes the phone number.
        /// </summary>
        /// <param name="phone">
        /// U.S. phone number (e.g. +15551234567)
        /// </param>
        /// <returns>A hashed string.</returns>
        private string MakeHash(string phone)
        {
            return BCrypt.Net.BCrypt.HashPassword(phone);
        }

        /// <summary>
        /// Verifies that the phone number matches the hash.
        /// </summary>
        /// <param name="phone">
        /// U.S. phone number (e.g. +15551234567)
        /// </param>
        /// <param name="hash">
        /// Hash to compare against
        /// </param>
        /// <returns>True if the phone number matches, otherwise false.</returns>
        private bool VerifyHash(string phone, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(phone, hash);
        }
    }
    public interface ILimitCount
    {
        bool IsNewEntry(string phone);
        bool AddEntry(string phone);
        bool IsOverLimit(string phone);
        bool Increment(string phone);
        bool IsExpired(string phone);
    }
}