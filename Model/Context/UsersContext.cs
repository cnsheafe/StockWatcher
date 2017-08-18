using Microsoft.EntityFrameworkCore;

using StockWatcher.Model.Schemas;

namespace StockWatcher.Model.Context {
    public class UsersContext: DbContext {
        public UsersContext(DbContextOptions<UsersContext> options): base(options) { }

        public DbSet<User> Users {get; set;}
    }
}