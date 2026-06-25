using InventoryManagement.Application.DTOs;
using InventoryManagement.Domain.Enums;

namespace InventoryManagement.Application.Interfaces
{
    /// <summary>
    /// Encapsulates the business rules for working with products.
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Returns a paginated list of products with optional filtering.
        /// </summary>
        /// <param name="searchTerm">Optional text matched against name/SKU.</param>
        /// <param name="categoryId">Optional category filter.</param>
        /// <param name="stockStatus">Optional stock-status filter.</param>
        /// <param name="pageNumber">1-based page number.</param>
        /// <param name="pageSize">Page size.</param>
        Task<PaginatedResultDto<ProductDto>> GetPagedAsync(
            string? searchTerm,
            int? categoryId,
            StockStatus? stockStatus,
            int pageNumber,
            int pageSize);

        /// <summary>
        /// Returns a product by identifier, or null if not found.
        /// </summary>
        Task<ProductDto?> GetByIdAsync(int id);

        /// <summary>
        /// Creates a new product. Throws <see cref="Common.ValidationException"/> when invalid.
        /// </summary>
        Task<ProductDto> CreateAsync(ProductDto product);

        /// <summary>
        /// Updates an existing product. Throws <see cref="Common.ValidationException"/> when invalid.
        /// </summary>
        Task<ProductDto> UpdateAsync(ProductDto product);

        /// <summary>
        /// Deletes a product by identifier.
        /// </summary>
        Task DeleteAsync(int id);

        /// <summary>
        /// Returns whether a SKU is already used by another product.
        /// </summary>
        Task<bool> SkuExistsAsync(string sku, int? excludeProductId = null);
    }
}
