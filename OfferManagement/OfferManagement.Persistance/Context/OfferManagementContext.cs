using Microsoft.EntityFrameworkCore;
using OfferManagement.Domain.Entities;
using OfferManagement.Persistance.Configurations;
using static System.Net.Mime.MediaTypeNames;

namespace OfferManagement.Persistance.Context
{
    public class OfferManagementContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Domain.Entities.Image> Images { get; set; }

        public OfferManagementContext(DbContextOptions<OfferManagementContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AdministratorConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyConfiguration());
            modelBuilder.ApplyConfiguration(new OfferConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new SubscriptionConfiguration());
            modelBuilder.ApplyConfiguration(new PurchaseConfiguration());
            modelBuilder.ApplyConfiguration(new ImageConfiguration());
        }
    }
}
