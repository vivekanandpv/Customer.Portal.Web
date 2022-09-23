using Customer.Portal.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Customer.Portal.Web.Context {
    public class CustomerPortalContext : DbContext {
        public CustomerPortalContext(DbContextOptions<CustomerPortalContext> options) : base(options) {
        }

        public DbSet<BankCustomer> Customers { get; set; }
        public DbSet<Account> Type { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserRole>()
                .HasOne<User>()
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne<Role>()
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);
            

            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<Account>()
                .HasOne<BankCustomer>()
                .WithMany(c => c.Accounts)
                .HasForeignKey(a => a.CustomerId);
        }
    }
}