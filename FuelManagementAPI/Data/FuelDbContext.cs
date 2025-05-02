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
        public DbSet<Price> Prices { get; set; }
        public DbSet<FuelEntry> FuelEntries { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }   

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
            .HasOne(p => p.ProductCategory)
            .WithMany() // or .WithMany(c => c.Products) if you define a collection
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

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
