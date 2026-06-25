using System;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Data
{
    /// <summary>
    /// Seeds the database with sample categories and products on first run.
    /// </summary>
    public static class SeedData
    {
        /// <summary>
        /// Applies seed data to the supplied <see cref="ApplicationDbContext"/>.
        /// </summary>
        /// <param name="context">The EF Core context.</param>
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            await context.Database.EnsureCreatedAsync();

            if (context.Categories.Any() || context.Products.Any())
            {
                return;
            }

            var electronics = new Category
            {
                CategoryName = "Electronics",
                Description = "Electronic devices, gadgets, and accessories.",
                CreatedDate = DateTime.UtcNow
            };

            var furniture = new Category
            {
                CategoryName = "Furniture",
                Description = "Office and home furniture.",
                CreatedDate = DateTime.UtcNow
            };

            var accessories = new Category
            {
                CategoryName = "Accessories",
                Description = "Peripherals and miscellaneous accessories.",
                CreatedDate = DateTime.UtcNow
            };

            await context.Categories.AddRangeAsync(electronics, furniture, accessories);
            await context.SaveChangesAsync();

            // Reload to get CategoryId values.
            electronics = await context.Categories.FirstAsync(c => c.CategoryName == "Electronics");
            furniture = await context.Categories.FirstAsync(c => c.CategoryName == "Furniture");
            accessories = await context.Categories.FirstAsync(c => c.CategoryName == "Accessories");

            var products = new[]
            {
                new Product
                {
                    ProductName = "Laptop",
                    SKU = "ELEC-LAP-001",
                    Description = "High performance business laptop with 16GB RAM.",
                    Price = 1299.99m,
                    Quantity = 25,
                    LowStockThreshold = 5,
                    CategoryId = electronics.CategoryId,
                    CreatedDate = DateTime.UtcNow
                },
                new Product
                {
                    ProductName = "Monitor",
                    SKU = "ELEC-MON-001",
                    Description = "27-inch 4K Ultra HD monitor.",
                    Price = 399.99m,
                    Quantity = 40,
                    LowStockThreshold = 10,
                    CategoryId = electronics.CategoryId,
                    CreatedDate = DateTime.UtcNow
                },
                new Product
                {
                    ProductName = "Keyboard",
                    SKU = "ACC-KBD-001",
                    Description = "Mechanical keyboard with RGB backlight.",
                    Price = 89.99m,
                    Quantity = 60,
                    LowStockThreshold = 15,
                    CategoryId = accessories.CategoryId,
                    CreatedDate = DateTime.UtcNow
                },
                new Product
                {
                    ProductName = "Mouse",
                    SKU = "ACC-MOU-001",
                    Description = "Wireless optical mouse.",
                    Price = 29.99m,
                    Quantity = 8,
                    LowStockThreshold = 10,
                    CategoryId = accessories.CategoryId,
                    CreatedDate = DateTime.UtcNow
                },
                new Product
                {
                    ProductName = "Office Chair",
                    SKU = "FUR-CHR-001",
                    Description = "Ergonomic office chair with lumbar support.",
                    Price = 249.99m,
                    Quantity = 0,
                    LowStockThreshold = 5,
                    CategoryId = furniture.CategoryId,
                    CreatedDate = DateTime.UtcNow
                }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
    }
}