using Microsoft.EntityFrameworkCore;
using StockWatcher.Model.Schemas;

namespace StockWatcher.Model.Services.Tests
{
    public class MockDbContext: DbContext, IStockDbContext
    {
        public MockDbContext(DbContextOptions<MockDbContext> options) : base(options)
        {
            base.Database.OpenConnection();
            base.Database.EnsureCreated();
            base.Database.Migrate();
            
        }
        public DbSet<Company> Companies { get; set; }
        public DbSet<RequestRecord> Requests { get; set; }
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
}