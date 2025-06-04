using FuelManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementAPI.Data
{
    public class FuelDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpcontextAccessor;
        public FuelDbContext(DbContextOptions<FuelDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options) 
        {
            _httpcontextAccessor = httpContextAccessor;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<FuelSale> FuelSales { get; set; }
        public DbSet<LubeSale> LubeSales { get; set; }
        public DbSet<LubeEntry> LubeEntries { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountTransaction> AccountTransactions { get; set; }
        public DbSet<Attendant> Attendants { get; set; }
        public DbSet<PriceHistory> PriceHistory { get; set; }
        public DbSet<FuelEntry> FuelEntries { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductCategoryType> ProductCategoriesType { get; set; }
        public DbSet<Purchase> Purchase { get; set; }
        public DbSet<PurchaseEntry> PurchaseEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductCategory>()
            .HasMany(c => c.ProductCategoriesTypes)
            .WithOne(ct => ct.productCategory)
            .HasForeignKey(ct => ct.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductCategoryType>()
          .HasMany(ct => ct.Products)
          .WithOne(p => p.ProductCategoryType)
          .HasForeignKey(p => p.CategoryTypeId)
          .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product>()
            .HasOne(p => p.ProductCategoryType)
            .WithMany(c => c.Products) // or .WithMany(c => c.Products) if you define a collection
            .HasForeignKey(p => p.CategoryTypeId)
            .OnDelete(DeleteBehavior.Restrict);

            
            modelBuilder.Entity<FuelEntry>()
           .HasMany(fe => fe.Sales)
           .WithOne(fs => fs.FuelEntry)
           .HasForeignKey(fs => fs.FuelEntryId);

            modelBuilder.Entity<FuelSale>()
                .HasOne(fs => fs.Product)
                .WithMany(fs => fs.FuelSales)
                .HasForeignKey(fs => fs.ProductId);

            modelBuilder.Entity<Account>()
               .HasMany(a => a.Transactions)
               .WithOne(t => t.Account)
               .HasForeignKey(t => t.AccountId);

            modelBuilder.Entity<LubeEntry>()
               .HasMany(le => le.Sales)
               .WithOne(ls => ls.LubeEntry)
               .HasForeignKey(ls => ls.LubeEntryId);

            modelBuilder.Entity<PriceHistory>()
                .HasOne(p => p.ProductCategoryType)
                .WithMany()
                .HasForeignKey(p => p.CategoryTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PriceHistory>()
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PriceHistory>()
                .HasOne(p => p.Product)
                .WithMany()
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Restrict);


            base.OnModelCreating(modelBuilder);
            modelBuilder.ConfigureUtcDateTime();           
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity &&
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            var userIdClaim = _httpcontextAccessor.HttpContext?.User?.FindFirst("id")?.Value;
            int.TryParse(userIdClaim, out int userId);

            foreach (var entityEntry in entries)
            {
                if (entityEntry.Entity is BaseEntity baseEntity)
                {
                    if (entityEntry.State == EntityState.Added)
                        baseEntity.CreatedDate = DateTime.UtcNow;
                    else if (entityEntry.State == EntityState.Modified)
                        baseEntity.ModifiedDate = DateTime.UtcNow;
                }

                if (entityEntry.Entity is UserEntity userEntity && userId > 0)
                {
                    userEntity.UsersId = userId;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
