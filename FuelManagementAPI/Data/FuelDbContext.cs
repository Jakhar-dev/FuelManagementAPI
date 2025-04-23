using FuelManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementAPI.Data
{
    public class FuelDbContext : DbContext
    {
        public FuelDbContext(DbContextOptions<FuelDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<FuelSale> FuelSales { get; set; }
        public DbSet<LubeSale> LubeSales { get; set; }
        public DbSet<LubeEntry> LubeEntries { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountTransaction> AccountTransactions { get; set; }
        public DbSet<Attendant> Attendants { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<FuelEntry> FuelEntries { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }   

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FuelEntry>()
           .HasMany(fe => fe.Sales)
           .WithOne(fs => fs.FuelEntry)
           .HasForeignKey(fs => fs.FuelEntryId);

            modelBuilder.Entity<FuelSale>()
                .HasOne(fs => fs.Product)
                .WithMany()
                .HasForeignKey(fs => fs.ProductId);

            modelBuilder.Entity<Account>()
               .HasMany(a => a.Transactions)
               .WithOne(t => t.Account)
               .HasForeignKey(t => t.AccountId);

            modelBuilder.Entity<LubeEntry>()
               .HasMany(le => le.Sales)
               .WithOne(ls => ls.LubeEntry)
               .HasForeignKey(ls => ls.LubeEntryId);

            base.OnModelCreating(modelBuilder);
            modelBuilder.ConfigureUtcDateTime();           
        }

    }
}
