using AutoMapper;
using InventoryManagement.Application.Common;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Application.Mappings;
using InventoryManagement.Application.Services;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Enums;
using InventoryManagement.Domain.Interfaces;
using Moq;

namespace InventoryManagement.Tests.Helpers
{
    /// <summary>
    /// Shared helpers used by test classes.
    /// </summary>
    internal static class TestHelpers
    {
        /// <summary>
        /// Creates an in-memory IMapper using the production mapping profile.
        /// </summary>
        public static IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            config.AssertConfigurationIsValid();
            return config.CreateMapper();
        }

        /// <summary>
        /// Builds a stub UnitOfWork backed by in-memory dictionaries.
        /// </summary>
        public static IUnitOfWork BuildInMemoryUnitOfWork(
            List<Product>? products = null,
            List<Category>? categories = null,
            List<InventoryTransaction>? transactions = null)
        {
            products ??= new List<Product>();
            categories ??= new List<Category>();
            transactions ??= new List<InventoryTransaction>();

            var mock = new Mock<IUnitOfWork>();
            mock.Setup(u => u.Products).Returns(new InMemoryRepository<Product>(products));
            mock.Setup(u => u.Categories).Returns(new InMemoryRepository<Category>(categories));
            mock.Setup(u => u.InventoryTransactions).Returns(new InMemoryRepository<InventoryTransaction>(transactions));
            mock.Setup(u => u.CompleteAsync()).Returns(async () =>
            {
                await Task.CompletedTask;
                return 1;
            });
            return mock.Object;
        }

        /// <summary>
        /// Builds a valid ProductDto for testing.
        /// </summary>
        public static ProductDto ValidProduct(int categoryId = 1)
        {
            return new ProductDto
            {
                ProductName = "Test Product",
                SKU = "TEST-001",
                Description = "Test description",
                Price = 99.99m,
                Quantity = 10,
                LowStockThreshold = 5,
                CategoryId = categoryId,
                CreatedDate = System.DateTime.UtcNow
            };
        }

        /// <summary>
        /// Builds a valid CategoryDto for testing.
        /// </summary>
        public static CategoryDto ValidCategory()
        {
            return new CategoryDto
            {
                CategoryName = "Test Category",
                Description = "Test description",
                CreatedDate = System.DateTime.UtcNow
            };
        }
    }

    /// <summary>
    /// In-memory IRepository implementation used by tests.
    /// </summary>
    internal class InMemoryRepository<T> : IRepository<T> where T : class
    {
        private readonly List<T> _items;
        private int _nextId;

        public InMemoryRepository(List<T> items, int seed = 1)
        {
            _items = items;
            _nextId = seed + items.Count;
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<T>>(_items.ToList());
        }

        public Task<IEnumerable<T>> FindAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            var compiled = predicate.Compile();
            return Task.FromResult<IEnumerable<T>>(_items.Where(compiled).ToList());
        }

        public Task<T?> GetByIdAsync(int id)
        {
            // We assume entities have an Id-like property through reflection.
            T? match = default;
            foreach (var item in _items)
            {
                var prop = item.GetType().GetProperty("ProductId")
                          ?? item.GetType().GetProperty("CategoryId")
                          ?? item.GetType().GetProperty("TransactionId");
                if (prop != null && prop.GetValue(item) is int val && val == id)
                {
                    match = item;
                    break;
                }
            }
            return Task.FromResult(match);
        }

        public Task AddAsync(T entity)
        {
            var prop = entity.GetType().GetProperty("ProductId")
                      ?? entity.GetType().GetProperty("CategoryId")
                      ?? entity.GetType().GetProperty("TransactionId");
            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(entity, _nextId++);
            }
            _items.Add(entity);
            return Task.CompletedTask;
        }

        public void Update(T entity) { /* no-op */ }

        public void Remove(T entity)
        {
            _items.Remove(entity);
        }

        public Task SaveChangesAsync() => Task.CompletedTask;
    }
}