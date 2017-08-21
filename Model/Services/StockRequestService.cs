using System;

using Microsoft.EntityFrameworkCore;

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
                var exception = (Npgsql.PostgresException) dbException.InnerException;
                Console.WriteLine(exception.SqlState);
                success = false;
            }
            return success;
        }
    }
}