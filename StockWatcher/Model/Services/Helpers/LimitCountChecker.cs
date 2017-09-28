using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using BCrypt.Net;


using StockWatcher.Model.Schemas;

namespace StockWatcher.Model.Services.Helpers
{
    public class LimitCountChecker
    {
        private IStockDbContext context;
        private int limit = 5;
        public LimitCountChecker(IStockDbContext _context)
        {
            context = _context;
        }

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
                // var exception = (Npgsql.PostgresException)dbException.InnerException;
                var exception = dbException.InnerException;
                Console.WriteLine(exception.Message);

                return false;
            }
        }

        public bool IsOverLimit(string phone)
        {
            if(IsNewEntry(phone))
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

        private string MakeHash(string phone)
        {
            return BCrypt.Net.BCrypt.HashPassword(phone);
        }

        private bool VerifyHash(string phone, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(phone, hash);
        }
    }
}