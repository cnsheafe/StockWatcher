using Microsoft.EntityFrameworkCore;

using StockWatcher.Model.Schemas;

namespace StockWatcher.Model
{
    public class StockDbContext : DbContext, IStockDbContext
    {
        public StockDbContext(DbContextOptions<StockDbContext> options) : base(options)
        {
            var db = base.Database;
            db.OpenConnection();
            db.ExecuteSqlCommand(@"CREATE EXTENSION IF NOT EXISTS ""uuid-ossp"" ");
            db.CloseConnection();
        }
        
        // Set of companies to lookup
        public DbSet<Company> Companies { get; set; }
        // Requests to make for SMS and watches
        public DbSet<RequestRecord> Requests { get; set; }
        // Counts the number of requests made by a user
        public DbSet<LimitCount> LimitCounts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.Symbol);
                entity.ToTable("companies");
                entity.Property(e => e.Symbol).ValueGeneratedNever();
                entity.Property(e => e.Name).HasColumnName("Name");
                entity.Property(e => e.Adrtso).HasColumnName("ADRTSO");
                entity.Property(e => e.IPOyear).HasColumnName("IPOyear");
            });

            modelBuilder.Entity<RequestRecord>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("requests");
            });

            modelBuilder.Entity<LimitCount>(entity =>
            {
                entity.ToTable("limit_count");
                entity.HasKey(e => e.PhoneHash);
                entity.Property(e => e.PhoneHash).HasColumnName("phone_hash");
                entity.Property(e => e.Date).HasColumnName("date");
                entity.Property(e => e.Count).HasColumnName("count");

            });
        }
    }

    public interface IStockDbContext
    {
        DbSet<Company> Companies { get; }
        DbSet<RequestRecord> Requests { get; }
        DbSet<LimitCount> LimitCounts { get; }

        int SaveChanges();
       
    }
}