using InventoryManagement.Domain.Enums;

namespace InventoryManagement.Web.ViewModels
{
    /// <summary>
    /// Display model for inventory transactions in the UI.
    /// </summary>
    public class InventoryTransactionViewModel
    {
        /// <summary>
        /// Gets or sets the transaction identifier.
        /// </summary>
        public int TransactionId { get; set; }

        /// <summary>
        /// Gets or sets the product name.
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product SKU.
        /// </summary>
        public string ProductSKU { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the signed quantity change.
        /// </summary>
        public int QuantityChanged { get; set; }

        /// <summary>
        /// Gets or sets the transaction type.
        /// </summary>
        public TransactionType TransactionType { get; set; }

        /// <summary>
        /// Gets or sets the transaction date.
        /// </summary>
        public System.DateTime TransactionDate { get; set; }

        /// <summary>
        /// Gets or sets optional notes.
        /// </summary>
        public string? Notes { get; set; }
    }
}