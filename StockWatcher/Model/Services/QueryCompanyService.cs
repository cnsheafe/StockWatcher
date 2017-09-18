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
        }

        public IEnumerable<Company> SearchCompanies(Query query)
        {
            var table = context.Companies;
            IEnumerable<Company> companies = new List<Company>();
            if (query.SearchPhrase.Length == 0)
            {
                return companies;
            }

            if (query.IsSymbol.CompareTo("true") == 0)
            {
                companies = table
                    .Where(c => c.Symbol.Contains(query.SearchPhrase.ToUpper()))
                    .Take(5)
                    .AsEnumerable();
            }
            else
            {
                companies = table
                    .Where(c => c.Name.Contains(query.SearchPhrase))
                    .Take(5)
                    .AsEnumerable();
            }
            context.SaveChanges();

            return companies;
        }
    }
}