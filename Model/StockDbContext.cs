using Microsoft.EntityFrameworkCore;

using StockWatcher.Model.Schemas;

namespace StockWatcher.Model {
    public class StockDbContext: DbContext {
        public StockDbContext(DbContextOptions<StockDbContext> options) : base(options) { }

        public DbSet<User> Users {get; set;}
        public DbSet<Stock> Stocks {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<User>()
                .HasIndex(user => 
                    new {
                        user.Phone,
                        user.Username
                    })
                .IsUnique();
            base.OnModelCreating(modelBuilder);
        }
    }
}