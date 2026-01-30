using Microsoft.EntityFrameworkCore;
using ProductService.Models;

namespace ProductService.Data
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) 
                                : base(options) {}

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var seedUtc = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Laptop", Description = "High performance laptop"
                , Price = 50000, Stock = 10, CreatedAt = seedUtc, UpdatedAt = seedUtc},
                new Product { Id = 2, Name = "Mouse", Description = "Wireless mouse"
                , Price = 500, Stock = 50, CreatedAt = seedUtc, UpdatedAt = seedUtc},
                new Product { Id = 3, Name = "Keyboard", Description = "Mechanical keyboard"
                , Price = 1500, Stock = 30, CreatedAt = seedUtc, UpdatedAt = seedUtc}
            );
        }
    }
}