using System;

using Microsoft.EntityFrameworkCore;
using StockWatcher.Model;
using StockWatcher.Model.Schemas;


namespace StockWatcher.Model.Services {
    public class ManageUser {
        private StockDbContext context;
        public ManageUser(StockDbContext _StockDbContext) {
            context = _StockDbContext;
        }
        public bool AddUser(User user) {
            bool success = true;
            try
            {
                context.Users.Add(user);
                context.SaveChanges();
            }
            catch (DbUpdateException dbException)
            {
                var exception = (Npgsql.PostgresException) dbException.InnerException;
                string SqlCode = exception.SqlState;
                Console.WriteLine(SqlCode);
                success = string.CompareOrdinal(SqlCode, 1, "01P01", 1, length: 1) < 0;
            }
            return success;
        }
        
    }
}