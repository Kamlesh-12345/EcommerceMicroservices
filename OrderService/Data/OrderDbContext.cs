using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) 
                                : base(options) {}

        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().HasData(
                new Order { Id = 1, ProductId = 1, Quantity = 2, CustomerName = "John Doe",
                CustomerEmail = "john@example.com", ShippingAddress = "123 Street", 
                TotalAmount = 100000, Status = "Completed" },
                new Order { Id = 2, ProductId = 2, Quantity = 5, CustomerName = "Jane Smith",
                CustomerEmail = "jane@example.com", ShippingAddress = "456 Avenue", 
                TotalAmount = 2500, Status = "Pending" }
            );
        }
    }
}