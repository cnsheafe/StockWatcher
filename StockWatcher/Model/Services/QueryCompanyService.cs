using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.EntityFrameworkCore;

using StockWatcher.Model;
using StockWatcher.Model.Schemas;

namespace StockWatcher.Model.Services
{
    public class QueryCompanyService : IQueryCompanyService
    {
        private readonly IStockDbContext context;
        public QueryCompanyService(IStockDbContext _context)
        {
            context = _context;
        }

        public IEnumerable<Company> SearchCompanies(Query query)
        {
            var table = context.Companies;
            IEnumerable<Company> companies = new List<Company>();
            if (query.SearchPhrase.Length == 0)
            {
                return null;
            }

            var searchPhraseBlocks = query.SearchPhrase.Split(" ");
            var newSearchPhrase = new StringBuilder();
            foreach (var block in searchPhraseBlocks)
            {
                var newBlock = block.Substring(0, 1).ToUpper() + block.Substring(1) + " ";
                newSearchPhrase.Append(newBlock);
            }
            companies = table
                .Where(c => c.Symbol.Contains(query.SearchPhrase.ToUpper()) || c.Name.Contains(newSearchPhrase.ToString().TrimEnd()))
                .Take(5)
                .AsEnumerable();
            context.SaveChanges();

            return companies;
        }
    }

    public interface IQueryCompanyService
    {
        IEnumerable<Company> SearchCompanies(Query query);
    }
}