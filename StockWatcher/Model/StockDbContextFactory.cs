using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

using Npgsql;

using StockWatcher.Model.Services.Helpers;

namespace StockWatcher.Model
{
    public class StockDbContextFactory : IDesignTimeDbContextFactory<StockDbContext>
    {

        public StockDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<StockDbContext>();

            optionsBuilder.UseNpgsql(
                ConnectionStringParser.Parse(args[0])
            );
            return new StockDbContext(optionsBuilder.Options);
        }
    }
}