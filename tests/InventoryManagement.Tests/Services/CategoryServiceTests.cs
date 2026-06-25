using InventoryManagement.Application.Common;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Services;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Tests.Helpers;
using Xunit;

namespace InventoryManagement.Tests.Services
{
    /// <summary>
    /// Unit tests for <see cref="CategoryService"/>.
    /// </summary>
    public class CategoryServiceTests
    {
        [Fact]
        public async Task CreateAsync_WithValidCategory_ReturnsCreated()
        {
            // Arrange
            var unitOfWork = TestHelpers.BuildInMemoryUnitOfWork();
            var service = new CategoryService(unitOfWork, TestHelpers.CreateMapper());

            // Act
            var result = await service.CreateAsync(TestHelpers.ValidCategory());

            // Assert
            Assert.NotEqual(0, result.CategoryId);
            Assert.Equal("Test Category", result.CategoryName);
        }

        [Fact]
        public async Task CreateAsync_WithMissingName_ThrowsValidationException()
        {
            // Arrange
            var unitOfWork = TestHelpers.BuildInMemoryUnitOfWork();
            var service = new CategoryService(unitOfWork, TestHelpers.CreateMapper());
            var category = TestHelpers.ValidCategory();
            category.CategoryName = string.Empty;

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(category));
        }

        [Fact]
        public async Task UpdateAsync_WithValidCategory_UpdatesFields()
        {
            // Arrange
            var categories = new List<Category>
            {
                new() { CategoryId = 1, CategoryName = "Old Name", Description = "Old", CreatedDate = System.DateTime.UtcNow }
            };
            var unitOfWork = TestHelpers.BuildInMemoryUnitOfWork(categories: categories);
            var service = new CategoryService(unitOfWork, TestHelpers.CreateMapper());

            var updated = new CategoryDto
            {
                CategoryId = 1,
                CategoryName = "New Name",
                Description = "New desc"
            };

            // Act
            var result = await service.UpdateAsync(updated);

            // Assert
            Assert.Equal("New Name", result.CategoryName);
            Assert.Equal("New desc", result.Description);
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistentCategory_ThrowsValidationException()
        {
            // Arrange
            var unitOfWork = TestHelpers.BuildInMemoryUnitOfWork();
            var service = new CategoryService(unitOfWork, TestHelpers.CreateMapper());

            var updated = new CategoryDto
            {
                CategoryId = 999,
                CategoryName = "Missing"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => service.UpdateAsync(updated));
        }

        [Fact]
        public async Task DeleteAsync_RemovesCategory()
        {
            // Arrange
            var categories = new List<Category>
            {
                new() { CategoryId = 1, CategoryName = "ToDelete", CreatedDate = System.DateTime.UtcNow }
            };
            var unitOfWork = TestHelpers.BuildInMemoryUnitOfWork(categories: categories);
            var service = new CategoryService(unitOfWork, TestHelpers.CreateMapper());

            // Act
            await service.DeleteAsync(1);

            // Assert
            var result = await service.GetByIdAsync(1);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsCategoriesSortedByName()
        {
            // Arrange
            var categories = new List<Category>
            {
                new() { CategoryId = 1, CategoryName = "Zeta", CreatedDate = System.DateTime.UtcNow },
                new() { CategoryId = 2, CategoryName = "Alpha", CreatedDate = System.DateTime.UtcNow },
                new() { CategoryId = 3, CategoryName = "Beta", CreatedDate = System.DateTime.UtcNow }
            };
            var unitOfWork = TestHelpers.BuildInMemoryUnitOfWork(categories: categories);
            var service = new CategoryService(unitOfWork, TestHelpers.CreateMapper());

            // Act
            var result = (await service.GetAllAsync()).ToList();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal("Alpha", result[0].CategoryName);
            Assert.Equal("Beta", result[1].CategoryName);
            Assert.Equal("Zeta", result[2].CategoryName);
        }
    }
}