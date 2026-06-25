using System;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Domain.Entities
{
    /// <summary>
    /// Represents a product in the inventory system.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Gets or sets the unique identifier for the product.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        [Required]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Stock Keeping Unit (unique).
        /// </summary>
        [Required]
        [StringLength(50)]
        public string SKU { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product description.
        /// </summary>
        [StringLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the unit price of the product.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the available quantity in stock.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the stock threshold for low-stock alerts.
        /// </summary>
        public int LowStockThreshold { get; set; } = 10;

        /// <summary>
        /// Gets or sets the foreign key of the related category.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the date the product was created.
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the date the product was last updated.
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for the product's category.
        /// </summary>
        public Category? Category { get; set; }

        /// <summary>
        /// Gets or sets the collection of inventory transactions for this product.
        /// </summary>
        public ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
    }
}
