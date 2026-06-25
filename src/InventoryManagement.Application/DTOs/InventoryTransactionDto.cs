using System;
using System.ComponentModel.DataAnnotations;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Domain.Enums;

namespace InventoryManagement.Application.DTOs
{
    /// <summary>
    /// Data transfer object for an inventory transaction.
    /// </summary>
    public class InventoryTransactionDto
    {
        /// <summary>
        /// Gets or sets the transaction identifier.
        /// </summary>
        public int TransactionId { get; set; }

        /// <summary>
        /// Gets or sets the product identifier.
        /// </summary>
        [Required]
        [Display(Name = "Product")]
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the product name (read-only).
        /// </summary>
        public string? ProductName { get; set; }

        /// <summary>
        /// Gets or sets the SKU of the product (read-only).
        /// </summary>
        public string? ProductSKU { get; set; }

        /// <summary>
        /// Gets or sets the quantity change (positive for in, negative for out).
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the signed change to apply (positive for in, negative for out).
        /// </summary>
        public int QuantityChanged { get; set; }

        /// <summary>
        /// Gets or sets the transaction type.
        /// </summary>
        [Required]
        [Display(Name = "Transaction Type")]
        public TransactionType TransactionType { get; set; }

        /// <summary>
        /// Gets or sets the date of the transaction.
        /// </summary>
        [Display(Name = "Transaction Date")]
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets optional notes.
        /// </summary>
        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
