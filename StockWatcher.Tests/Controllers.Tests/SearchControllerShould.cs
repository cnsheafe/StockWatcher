using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

using StockWatcher.Model.Schemas;
using StockWatcher.Model.Services;

namespace StockWatcher.Controllers.Tests
{
    public class SearchControllerShould
    {
        [Fact]
        public void ReturnEnumberableOfCompaniesInJson()
        {
            var mockService = new Mock<IQueryCompanyService>();
            Query query = new Query
            {
                SearchPhrase="msft",
                IsSymbol=true
            };

            mockService
                .Setup(service => 
                    service.SearchCompanies(It.IsAny<Query>()))
                .Returns(FakeCompanies());

            var result = 
                new SearchController(mockService.Object)
                .GetCompany(query);
            
            Assert.IsType<JsonResult>(result);

        }

        private IEnumerable<Company> FakeCompanies()
        {
            var companies = new List<Company>();
            companies.Add(
                new Company{
                    Name="Microsoft",
                    Symbol="MSFT"
                }
            );
            return companies;
        }
    }
    
}