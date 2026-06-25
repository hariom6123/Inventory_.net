using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace InventoryManagement.Domain.Interfaces
{
    /// <summary>
    /// Defines a generic repository contract for entity persistence.
    /// </summary>
    /// <typeparam name="T">The entity type managed by this repository.</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Returns all entities of type <typeparamref name="T"/>.
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Returns the entities matching the supplied predicate.
        /// </summary>
        /// <param name="predicate">A filter expression evaluated against each entity.</param>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Returns the entity identified by <paramref name="id"/> or null if not found.
        /// </summary>
        /// <param name="id">The primary key value.</param>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Adds a new entity to the underlying store.
        /// </summary>
        /// <param name="entity">The entity instance to persist.</param>
        Task AddAsync(T entity);

        /// <summary>
        /// Updates an existing entity in the underlying store.
        /// </summary>
        /// <param name="entity">The entity instance to update.</param>
        void Update(T entity);

        /// <summary>
        /// Removes an existing entity from the underlying store.
        /// </summary>
        /// <param name="entity">The entity instance to remove.</param>
        void Remove(T entity);

        /// <summary>
        /// Persists pending changes to the underlying store.
        /// </summary>
        Task SaveChangesAsync();
    }
}
