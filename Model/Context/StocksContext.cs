using Microsoft.EntityFrameworkCore;

using StockWatcher.Model.Schemas;

namespace StockWatcher.Model.Context {
    public class StocksContext: DbContext {
        public StocksContext(DbContextOptions<StocksContext> options): base(options) { }

        public DbSet<Stock> Stocks {get; set;}
    }
}