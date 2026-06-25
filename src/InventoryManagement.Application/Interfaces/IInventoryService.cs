using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Domain.Enums;

namespace InventoryManagement.Application.Interfaces
{
    /// <summary>
    /// Encapsulates the business rules for inventory stock movements and history.
    /// </summary>
    public interface IInventoryService
    {
        /// <summary>
        /// Records an incoming stock movement for the given product.
        /// </summary>
        Task<InventoryTransactionDto> StockInAsync(int productId, int quantity, string? notes);

        /// <summary>
        /// Records an outgoing stock movement. Throws if it would make stock negative.
        /// </summary>
        Task<InventoryTransactionDto> StockOutAsync(int productId, int quantity, string? notes);

        /// <summary>
        /// Applies a manual adjustment (positive or negative) to a product's quantity.
        /// </summary>
        Task<InventoryTransactionDto> AdjustAsync(int productId, int newQuantity, string? notes);

        /// <summary>
        /// Returns the inventory transaction history, newest first.
        /// </summary>
        Task<IEnumerable<InventoryTransactionDto>> GetTransactionsAsync(int? productId = null);

        /// <summary>
        /// Computes the stock status for a single product.
        /// </summary>
        StockStatus GetStockStatus(int quantity, int lowStockThreshold);
    }
}
