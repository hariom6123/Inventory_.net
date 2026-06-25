using InventoryManagement.Application.Common;
using InventoryManagement.Application.Services;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Enums;
using InventoryManagement.Tests.Helpers;
using Xunit;

namespace InventoryManagement.Tests.Services
{
    /// <summary>
    /// Unit tests for <see cref="InventoryService"/>.
    /// </summary>
    public class InventoryServiceTests
    {
        [Fact]
        public async Task StockInAsync_IncreasesQuantity()
        {
            // Arrange
            var products = new List<Product>
            {
                new() { ProductId = 1, ProductName = "Test", SKU = "T-1", Price = 1, Quantity = 10, CategoryId = 1 }
            };
            var unitOfWork = TestHelpers.BuildInMemoryUnitOfWork(products: products);
            var service = new InventoryService(unitOfWork, TestHelpers.CreateMapper());

            // Act
            var tx = await service.StockInAsync(1, 5, "restock");

            // Assert
            Assert.Equal(5, tx.QuantityChanged);
            var p = await unitOfWork.Products.GetByIdAsync(1);
            Assert.Equal(15, p!.Quantity);
        }

        [Fact]
        public async Task StockOutAsync_DecreasesQuantity()
        {
            // Arrange
            var products = new List<Product>
            {
                new() { ProductId = 1, ProductName = "Test", SKU = "T-1", Price = 1, Quantity = 10, CategoryId = 1 }
            };
            var unitOfWork = TestHelpers.BuildInMemoryUnitOfWork(products: products);
            var service = new InventoryService(unitOfWork, TestHelpers.CreateMapper());

            // Act
            var tx = await service.StockOutAsync(1, 3, "sold");

            // Assert
            Assert.Equal(-3, tx.QuantityChanged);
            var p = await unitOfWork.Products.GetByIdAsync(1);
            Assert.Equal(7, p!.Quantity);
        }

        [Fact]
        public async Task StockOutAsync_PreventsNegativeInventory()
        {
            // Arrange
            var products = new List<Product>
            {
                new() { ProductId = 1, ProductName = "Test", SKU = "T-1", Price = 1, Quantity = 5, CategoryId = 1 }
            };
            var unitOfWork = TestHelpers.BuildInMemoryUnitOfWork(products: products);
            var service = new InventoryService(unitOfWork, TestHelpers.CreateMapper());

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => service.StockOutAsync(1, 100, "too many"));
        }

        [Fact]
        public async Task StockOutAsync_WithZeroQuantity_Throws()
        {
            // Arrange
            var unitOfWork = TestHelpers.BuildInMemoryUnitOfWork();
            var service = new InventoryService(unitOfWork, TestHelpers.CreateMapper());

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => service.StockOutAsync(1, 0, null));
        }

        [Fact]
        public async Task StockInAsync_WithZeroQuantity_Throws()
        {
            // Arrange
            var unitOfWork = TestHelpers.BuildInMemoryUnitOfWork();
            var service = new InventoryService(unitOfWork, TestHelpers.CreateMapper());

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => service.StockInAsync(1, 0, null));
        }

        [Fact]
        public async Task AdjustAsync_SetsToNewQuantity()
        {
            // Arrange
            var products = new List<Product>
            {
                new() { ProductId = 1, ProductName = "Test", SKU = "T-1", Price = 1, Quantity = 10, CategoryId = 1 }
            };
            var unitOfWork = TestHelpers.BuildInMemoryUnitOfWork(products: products);
            var service = new InventoryService(unitOfWork, TestHelpers.CreateMapper());

            // Act
            await service.AdjustAsync(1, 25, "audit");

            // Assert
            var p = await unitOfWork.Products.GetByIdAsync(1);
            Assert.Equal(25, p!.Quantity);
        }

        [Fact]
        public async Task AdjustAsync_WithNegativeValue_Throws()
        {
            // Arrange
            var unitOfWork = TestHelpers.BuildInMemoryUnitOfWork();
            var service = new InventoryService(unitOfWork, TestHelpers.CreateMapper());

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => service.AdjustAsync(1, -1, null));
        }

        [Fact]
        public async Task StockInAsync_WithNonExistentProduct_Throws()
        {
            // Arrange
            var unitOfWork = TestHelpers.BuildInMemoryUnitOfWork();
            var service = new InventoryService(unitOfWork, TestHelpers.CreateMapper());

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => service.StockInAsync(999, 1, null));
        }

        [Fact]
        public void GetStockStatus_ReturnsCorrectEnum()
        {
            // Arrange
            var service = new InventoryService(TestHelpers.BuildInMemoryUnitOfWork(), TestHelpers.CreateMapper());

            // Act & Assert
            Assert.Equal(StockStatus.OutOfStock, service.GetStockStatus(0, 10));
            Assert.Equal(StockStatus.LowStock, service.GetStockStatus(5, 10));
            Assert.Equal(StockStatus.InStock, service.GetStockStatus(50, 10));
        }
    }
}