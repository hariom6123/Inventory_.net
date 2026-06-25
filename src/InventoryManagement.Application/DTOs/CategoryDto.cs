using System;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Application.DTOs
{
    /// <summary>
    /// Data transfer object for category operations.
    /// </summary>
    public class CategoryDto
    {
        /// <summary>
        /// Gets or sets the category identifier.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the category name.
        /// </summary>
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the category description.
        /// </summary>
        [StringLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the count of products in the category.
        /// </summary>
        public int ProductCount { get; set; }

        /// <summary>
        /// Gets or sets the date the category was created.
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}
