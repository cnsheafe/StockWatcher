using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace StockWatcher.Model
{
    public class StockDbContextFactory : IDesignTimeDbContextFactory<StockDbContext>
    {
        public StockDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<StockDbContext>();
            Console.WriteLine($"connstring {Environment.GetEnvironmentVariable("SWConnString")}");
            optionsBuilder.UseNpgsql(Environment.GetEnvironmentVariable("SWConnString"));
            return new StockDbContext(optionsBuilder.Options);
        }
    }
}