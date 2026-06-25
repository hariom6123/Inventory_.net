using System.Threading.Tasks;

namespace InventoryManagement.Domain.Interfaces
{
    /// <summary>
    /// Coordinates work across multiple repositories within a single transaction.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Gets the product repository.
        /// </summary>
        IRepository<Entities.Product> Products { get; }

        /// <summary>
        /// Gets the category repository.
        /// </summary>
        IRepository<Entities.Category> Categories { get; }

        /// <summary>
        /// Gets the inventory transaction repository.
        /// </summary>
        IRepository<Entities.InventoryTransaction> InventoryTransactions { get; }

        /// <summary>
        /// Persists all pending changes as a single transaction.
        /// </summary>
        Task<int> CompleteAsync();
    }
}
