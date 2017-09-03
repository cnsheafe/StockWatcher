using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using StockWatcher.Model;
using StockWatcher.Model.Schemas;

namespace StockWatcher.Model.Services
{
    public class QueryCompanyService
    {
        private readonly StockDbContext context;
        public QueryCompanyService(StockDbContext _context)
        {
            context = _context;
            // context.Database.
        }

        public IEnumerable<Company> SearchCompanies(Query query)
        {
            var table = context.Companies;
            IEnumerable<Company> companies;

            if (query.IsEquity.CompareTo("true") == 0)
            {
                companies = table
                    .Where(c => c.Symbol.Contains(query.SearchPhrase.ToUpper()))
                    .AsEnumerable();
            }
            else
            {
                companies = table
                    .Where(c => c.Name.Contains(query.SearchPhrase))
                    .AsEnumerable();
            }
            context.SaveChanges();

            return companies;
        }
    }
}