using System;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace StockWatcher.Model.Services.Tests
{
    public class MockDbContextFactory : IDesignTimeDbContextFactory<MockDbContext>
    {

        public MockDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MockDbContext>();

            var connString = new SqliteConnectionStringBuilder(){DataSource = ":memory:"}.ConnectionString;
            optionsBuilder.UseSqlite(connString);
            return new MockDbContext(optionsBuilder.Options);
        }
    }
}