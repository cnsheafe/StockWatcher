using System;
using System.Collections.Generic;
using Xunit;

using StockWatcher.Model.Schemas;

namespace StockWatcher.Model.Services.Tests
{
    public class QueryCompanyServiceShould
    {
        private StockDbContext context;

        public QueryCompanyServiceShould()
        {
            string[] args = {Environment.GetEnvironmentVariable("DATABASE_URL")};
            context = new StockDbContextFactory().CreateDbContext(args);
        }

        [Fact]
        public void ReturnEnumberableOfCompanies()
        {
            var service = new QueryCompanyService(context);
            var query = new Query
            {
                SearchPhrase="msf",
                IsSymbol=true
            };
            IEnumerable<Company> companies = service.SearchCompanies(query);

            Assert.NotNull(companies);
            Assert.NotEmpty(companies);
        }
    }
    
}