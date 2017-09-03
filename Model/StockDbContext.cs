using Microsoft.EntityFrameworkCore;

using StockWatcher.Model.Schemas;

namespace StockWatcher.Model 
{
    public class StockDbContext: DbContext 
    {
        public StockDbContext(DbContextOptions<StockDbContext> options) : base(options) 
        {
            var db = base.Database;
            db.OpenConnection();
            db.ExecuteSqlCommand(@"CREATE EXTENSION IF NOT EXISTS ""uuid-ossp"" ");
            db.CloseConnection();
         }

        public DbSet<User> Users {get; set;}
        public DbSet<Stock> Stocks {get; set;}
        public DbSet<Company> Companies {get; set;}
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

            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.Symbol);
                entity.ToTable("companies");
                entity.Property(e => e.Symbol).ValueGeneratedNever();
                entity.Property(e => e.Name).HasColumnName("Name");
                entity.Property(e => e.Adrtso).HasColumnName("ADRTSO");
                entity.Property(e => e.IPOyear).HasColumnName("IPOyear");
            });
        }
    }
}