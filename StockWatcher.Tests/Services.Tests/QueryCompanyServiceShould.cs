using System;
using System.Collections.Generic;
using Xunit;

using StockWatcher.Model.Schemas;

namespace StockWatcher.Model.Services.Tests
{
    public class QueryCompanyServiceShould
    {
        private MockDbContext context;
        private IQueryCompanyService service;
        private List<Company> mockCompanies;

        public QueryCompanyServiceShould()
        {
            context = new MockDbContextFactory().CreateDbContext(null);
            service = new QueryCompanyService(context);
            var baseCompany = new Company{};
            mockCompanies = new List<Company> 
            {
                new Company {Symbol="MSFT", Name="Microsoft"},
                new Company {Symbol="AMSF", Name="Amerisafe"},
                new Company {Symbol="MSFG", Name="Main Source Financial Group"}
            };

            context.Companies.AddRange(mockCompanies);
            context.SaveChanges();
        }

        [Fact]
        public void ReturnEnumberableOfCompanies()
        {
            var query = new Query
            {
                SearchPhrase="msf"
            };
            IEnumerable<Company> companies = service.SearchCompanies(query);

            Assert.NotNull(companies);
            Assert.NotEmpty(companies);
        }
    }
    
}