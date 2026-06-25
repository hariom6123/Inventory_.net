using System;
using System.ComponentModel.DataAnnotations;
using InventoryManagement.Domain.Enums;

namespace InventoryManagement.Domain.Entities
{
    /// <summary>
    /// Represents a record of inventory movement for a product.
    /// </summary>
    public class InventoryTransaction
    {
        /// <summary>
        /// Gets or sets the unique identifier for the transaction.
        /// </summary>
        public int TransactionId { get; set; }

        /// <summary>
        /// Gets or sets the foreign key of the related product.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the amount the quantity changed (positive for stock in, negative for stock out).
        /// </summary>
        public int QuantityChanged { get; set; }

        /// <summary>
        /// Gets or sets the type of transaction (StockIn, StockOut, Adjustment).
        /// </summary>
        public TransactionType TransactionType { get; set; }

        /// <summary>
        /// Gets or sets the date the transaction occurred.
        /// </summary>
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets optional notes regarding the transaction.
        /// </summary>
        [StringLength(500)]
        public string? Notes { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for the related product.
        /// </summary>
        public Product? Product { get; set; }
    }
}
