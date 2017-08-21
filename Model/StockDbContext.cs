using Microsoft.EntityFrameworkCore;

using StockWatcher.Model.Schemas;

namespace StockWatcher.Model {
    public class StockDbContext: DbContext {
        public StockDbContext(DbContextOptions<StockDbContext> options) : base(options) {
            var db = base.Database;
            db.OpenConnection();
            db.ExecuteSqlCommand(@"CREATE EXTENSION IF NOT EXISTS ""uuid-ossp"" ");
            db.CloseConnection();
         }

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

            modelBuilder.Entity<User>()
                .Property(user => user.Uuid)
                .HasDefaultValueSql("uuid_generate_v4()");
        }
    }
}