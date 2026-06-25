using System;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Application.DTOs
{
    /// <summary>
    /// Data transfer object for product create/edit operations.
    /// </summary>
    public class ProductDto
    {
        /// <summary>
        /// Gets or sets the product identifier (zero for new products).
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the product name.
        /// </summary>
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200, MinimumLength = 2)]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the SKU (Stock Keeping Unit).
        /// </summary>
        [Required(ErrorMessage = "SKU is required")]
        [StringLength(50, MinimumLength = 3)]
        public string SKU { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product description.
        /// </summary>
        [StringLength(1000)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the unit price.
        /// </summary>
        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 1000000, ErrorMessage = "Price must be greater than zero")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the stock quantity.
        /// </summary>
        [Required(ErrorMessage = "Quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the low-stock alert threshold.
        /// </summary>
        [Range(0, int.MaxValue)]
        [Display(Name = "Low Stock Threshold")]
        public int LowStockThreshold { get; set; } = 10;

        /// <summary>
        /// Gets or sets the related category identifier.
        /// </summary>
        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the related category name (read-only).
        /// </summary>
        public string? CategoryName { get; set; }

        /// <summary>
        /// Gets or sets the date the product was created.
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}
