using System.Threading.Tasks;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Interfaces;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Repositories
{
    /// <summary>
    /// Coordinates work across multiple repositories within a single transaction.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IRepository<Product>? _products;
        private IRepository<Category>? _categories;
        private IRepository<InventoryTransaction>? _inventoryTransactions;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="context">The EF Core context.</param>
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context ?? throw new System.ArgumentNullException(nameof(context));
        }

        /// <inheritdoc/>
        public IRepository<Product> Products =>
            _products ??= new Repository<Product>(_context);

        /// <inheritdoc/>
        public IRepository<Category> Categories =>
            _categories ??= new Repository<Category>(_context);

        /// <inheritdoc/>
        public IRepository<InventoryTransaction> InventoryTransactions =>
            _inventoryTransactions ??= new Repository<InventoryTransaction>(_context);

        /// <inheritdoc/>
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Disposes the underlying EF Core context.
        /// </summary>
        public void Dispose()
        {
            _context.Dispose();
            System.GC.SuppressFinalize(this);
        }
    }
}