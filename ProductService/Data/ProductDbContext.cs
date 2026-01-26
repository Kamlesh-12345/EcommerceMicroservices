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
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Laptop", Description = "High performance laptop"
                , Price = 50000, Stock = 10},
                new Product { Id = 2, Name = "Mouse", Description = "Wireless mouse"
                , Price = 500, Stock = 50},
                new Product { Id = 3, Name = "Keyboard", Description = "Mechanical keyboard"
                , Price = 1500, Stock = 30}
            );
        }
    }
}