using Customer.Portal.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Customer.Portal.Web.Context {
    public class CustomerPortalContext : DbContext {
        public CustomerPortalContext(DbContextOptions<CustomerPortalContext> options) : base(options) {
        }

        public DbSet<BankCustomer> Customers { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });
        }
    }
}