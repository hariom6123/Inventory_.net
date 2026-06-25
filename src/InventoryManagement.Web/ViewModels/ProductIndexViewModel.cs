using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using InventoryManagement.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.Web.ViewModels
{
    /// <summary>
    /// Backing model for the Product index page (search, filter, pagination).
    /// </summary>
    public class ProductIndexViewModel
    {
        /// <summary>
        /// Gets or sets the paginated list of products.
        /// </summary>
        public PaginatedResultDto<ProductDto> Result { get; set; } = new();

        /// <summary>
        /// Gets or sets the search term.
        /// </summary>
        [Display(Name = "Search")]
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Gets or sets the optional category filter.
        /// </summary>
        [Display(Name = "Category")]
        public int? CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the optional stock status filter.
        /// </summary>
        [Display(Name = "Stock")]
        public string? StockStatus { get; set; }

        /// <summary>
        /// Gets or sets the categories used for the filter dropdown.
        /// </summary>
        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    }
}