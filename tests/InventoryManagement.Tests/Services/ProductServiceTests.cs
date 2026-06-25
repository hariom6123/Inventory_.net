using InventoryManagement.Application.Common;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Services;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Enums;
using InventoryManagement.Tests.Helpers;
using Xunit;

namespace InventoryManagement.Tests.Services
{
    /// <summary>
    /// Unit tests for <see cref="ProductService"/>.
    /// </summary>
    public class ProductServiceTests
    {
        [Fact]
        public async Task CreateAsync_WithValidProduct_ReturnsCreatedProduct()
        {
            // Arrange
            var unitOfWork = TestHelpers.BuildInMemoryUnitOfWork(
                categories: new() { new Category { CategoryId = 1, CategoryName = "Electronics" } });
            var service = new ProductService(unitOfWork, TestHelpers.CreateMapper());
            var product = TestHelpers.ValidProduct(categoryId: 1);

            // Act
            var result = await service.CreateAsync(product);

            // Assert
            Assert.NotEqual(0, result.ProductId);
            Assert.Equal("Test Product", result.ProductName);
            Assert.Equal("TEST-001", result.SKU);
        }

        [Fact]
        public async Task CreateAsync_WithDuplicateSku_ThrowsValidationException()
        {
            // Arrange
            var products = new List<Product>
            {
                new() { ProductId = 1, ProductName = "Existing", SKU = "DUP-001", Price = 1, Quantity = 1, CategoryId = 1 }
            };
            var unitOfWork = TestHelpers.BuildInMemoryUnitOfWork(products: products);
            var service = new ProductService(unitOfWork, TestHelpers.CreateMapper());

            var dup = new ProductDto
            {
                ProductName = "Duplicate",
                SKU = "DUP-001",
                Price = 5,
                Quantity = 1,
                CategoryId = 1
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(dup));
        }

        [Fact]
        public async Task CreateAsync_WithInvalidPrice_ThrowsValidationException()
        {
            // Arrange
            var unitOfWork = TestHelpers.BuildInMemoryUnitOfWork();
            var service = new ProductService(unitOfWork, TestHelpers.CreateMapper());
            var product = TestHelpers.ValidProduct(categoryId: 1);
            product.Price = 0;

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(product));
        }

        [Fact]
        public async Task CreateAsync_WithNegativeQuantity_ThrowsValidationException()
        {
            // Arrange
            var unitOfWork = TestHelpers.BuildInMemoryUnitOfWork();
            var service = new ProductService(unitOfWork, TestHelpers.CreateMapper());
            var product = TestHelpers.ValidProduct(categoryId: 1);
            product.Quantity = -1;

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(product));
        }

        [Fact]
        public async Task CreateAsync_WithMissingName_ThrowsValidationException()
        {
            // Arrange
            var unitOfWork = TestHelpers.BuildInMemoryUnitOfWork();
            var service = new ProductService(unitOfWork, TestHelpers.CreateMapper());
            var product = TestHelpers.ValidProduct(categoryId: 1);
            product.ProductName = string.Empty;

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(product));
        }

        [Fact]
        public async Task UpdateAsync_WithValidProduct_UpdatesFields()
        {
            // Arrange
            var products = new List<Product>
            {
                new() { ProductId = 1, ProductName = "Old Name", SKU = "OLD-001", Price = 1, Quantity = 1, CategoryId = 1 }
            };
            var unitOfWork = TestHelpers.BuildInMemoryUnitOfWork(products: products);
            var service = new ProductService(unitOfWork, TestHelpers.CreateMapper());

            var updated = new ProductDto
            {
                ProductId = 1,
                ProductName = "New Name",
                SKU = "OLD-001",
                Price = 2.5m,
                Quantity = 5,
                CategoryId = 1
            };

            // Act
            var result = await service.UpdateAsync(updated);

            // Assert
            Assert.Equal("New Name", result.ProductName);
            Assert.Equal(2.5m, result.Price);
            Assert.Equal(5, result.Quantity);
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistentProduct_ThrowsValidationException()
        {
            // Arrange
            var unitOfWork = TestHelpers.BuildInMemoryUnitOfWork();
            var service = new ProductService(unitOfWork, TestHelpers.CreateMapper());
            var updated = TestHelpers.ValidProduct(categoryId: 1);
            updated.ProductId = 999;

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => service.UpdateAsync(updated));
        }

        [Fact]
        public async Task DeleteAsync_RemovesProduct()
        {
            // Arrange
            var products = new List<Product>
            {
                new() { ProductId = 1, ProductName = "X", SKU = "X-1", Price = 1, Quantity = 1, CategoryId = 1 }
            };
            var unitOfWork = TestHelpers.BuildInMemoryUnitOfWork(products: products);
            var service = new ProductService(unitOfWork, TestHelpers.CreateMapper());

            // Act
            await service.DeleteAsync(1);

            // Assert
            var remaining = await service.GetByIdAsync(1);
            Assert.Null(remaining);
        }

        [Fact]
        public async Task SearchAsync_FindsProductByNameOrSku()
        {
            // Arrange
            var products = new List<Product>
            {
                new() { ProductId = 1, ProductName = "Laptop Pro", SKU = "LAP-001", Price = 1000, Quantity = 5, CategoryId = 1 },
                new() { ProductId = 2, ProductName = "Monitor HD", SKU = "MON-001", Price = 200, Quantity = 10, CategoryId = 1 },
                new() { ProductId = 3, ProductName = "Mouse Wireless", SKU = "MOU-001", Price = 25, Quantity = 50, CategoryId = 1 }
            };
            var unitOfWork = TestHelpers.BuildInMemoryUnitOfWork(products: products);
            var service = new ProductService(unitOfWork, TestHelpers.CreateMapper());

            // Act
            var byName = await service.GetPagedAsync("laptop", null, null, 1, 10);
            var bySku = await service.GetPagedAsync("MON-001", null, null, 1, 10);

            // Assert
            Assert.Single(byName.Items);
            Assert.Equal("Laptop Pro", byName.Items.First().ProductName);
            Assert.Single(bySku.Items);
            Assert.Equal("Monitor HD", bySku.Items.First().ProductName);
        }

        [Fact]
        public async Task GetPagedAsync_FilterByCategory_ReturnsOnlyMatching()
        {
            // Arrange
            var products = new List<Product>
            {
                new() { ProductId = 1, ProductName = "A", SKU = "A-1", Price = 1, Quantity = 1, CategoryId = 1 },
                new() { ProductId = 2, ProductName = "B", SKU = "B-1", Price = 1, Quantity = 1, CategoryId = 2 }
            };
            var unitOfWork = TestHelpers.BuildInMemoryUnitOfWork(products: products);
            var service = new ProductService(unitOfWork, TestHelpers.CreateMapper());

            // Act
            var page = await service.GetPagedAsync(null, categoryId: 2, null, 1, 10);

            // Assert
            Assert.Single(page.Items);
            Assert.Equal("B", page.Items.First().ProductName);
        }

        [Fact]
        public async Task GetPagedAsync_FilterByStockStatus_ReturnsOnlyMatching()
        {
            // Arrange
            var products = new List<Product>
            {
                new() { ProductId = 1, ProductName = "Out", SKU = "O-1", Price = 1, Quantity = 0, CategoryId = 1 },
                new() { ProductId = 2, ProductName = "Low", SKU = "L-1", Price = 1, Quantity = 2, LowStockThreshold = 5, CategoryId = 1 },
                new() { ProductId = 3, ProductName = "In", SKU = "I-1", Price = 1, Quantity = 50, LowStockThreshold = 5, CategoryId = 1 }
            };
            var unitOfWork = TestHelpers.BuildInMemoryUnitOfWork(products: products);
            var service = new ProductService(unitOfWork, TestHelpers.CreateMapper());

            // Act
            var outStock = await service.GetPagedAsync(null, null, StockStatus.OutOfStock, 1, 10);
            var lowStock = await service.GetPagedAsync(null, null, StockStatus.LowStock, 1, 10);
            var inStock = await service.GetPagedAsync(null, null, StockStatus.InStock, 1, 10);

            // Assert
            Assert.Single(outStock.Items);
            Assert.Single(lowStock.Items);
            Assert.Single(inStock.Items);
            Assert.Equal("Out", outStock.Items.First().ProductName);
            Assert.Equal("Low", lowStock.Items.First().ProductName);
            Assert.Equal("In", inStock.Items.First().ProductName);
        }

        [Fact]
        public async Task GetPagedAsync_Pagination_Works()
        {
            // Arrange
            var products = Enumerable.Range(1, 25).Select(i => new Product
            {
                ProductId = i,
                ProductName = $"Product {i}",
                SKU = $"SKU-{i:000}",
                Price = 1,
                Quantity = 1,
                CategoryId = 1
            }).ToList();

            var unitOfWork = TestHelpers.BuildInMemoryUnitOfWork(products: products);
            var service = new ProductService(unitOfWork, TestHelpers.CreateMapper());

            // Act
            var page1 = await service.GetPagedAsync(null, null, null, 1, 10);
            var page2 = await service.GetPagedAsync(null, null, null, 2, 10);
            var page3 = await service.GetPagedAsync(null, null, null, 3, 10);

            // Assert
            Assert.Equal(25, page1.TotalCount);
            Assert.Equal(10, page1.Items.Count());
            Assert.Equal(10, page2.Items.Count());
            Assert.Equal(5, page3.Items.Count());
            Assert.Equal(3, page1.TotalPages);
        }

        [Fact]
        public async Task SkuExistsAsync_ReturnsTrueWhenDuplicate()
        {
            // Arrange
            var products = new List<Product>
            {
                new() { ProductId = 1, ProductName = "X", SKU = "DUPL-001", Price = 1, Quantity = 1, CategoryId = 1 }
            };
            var unitOfWork = TestHelpers.BuildInMemoryUnitOfWork(products: products);
            var service = new ProductService(unitOfWork, TestHelpers.CreateMapper());

            // Act
            var exists = await service.SkuExistsAsync("DUPL-001");
            var existsCase = await service.SkuExistsAsync("dupl-001");
            var notExists = await service.SkuExistsAsync("OTHER-001");

            // Assert
            Assert.True(exists);
            Assert.True(existsCase);
            Assert.False(notExists);
        }
    }
}