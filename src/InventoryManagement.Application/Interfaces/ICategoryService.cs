using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagement.Application.DTOs;

namespace InventoryManagement.Application.Interfaces
{
    /// <summary>
    /// Encapsulates the business rules for working with categories.
    /// </summary>
    public interface ICategoryService
    {
        /// <summary>
        /// Returns all categories ordered by name.
        /// </summary>
        Task<IEnumerable<CategoryDto>> GetAllAsync();

        /// <summary>
        /// Returns a category by identifier, or null if not found.
        /// </summary>
        Task<CategoryDto?> GetByIdAsync(int id);

        /// <summary>
        /// Creates a new category. Throws <see cref="Common.ValidationException"/> on invalid data.
        /// </summary>
        Task<CategoryDto> CreateAsync(CategoryDto category);

        /// <summary>
        /// Updates an existing category. Throws <see cref="Common.ValidationException"/> on invalid data.
        /// </summary>
        Task<CategoryDto> UpdateAsync(CategoryDto category);

        /// <summary>
        /// Deletes a category by identifier.
        /// </summary>
        Task DeleteAsync(int id);
    }
}
