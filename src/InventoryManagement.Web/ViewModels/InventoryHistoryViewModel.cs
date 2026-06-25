using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using InventoryManagement.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.Web.ViewModels
{
    /// <summary>
    /// Backing model for the inventory history page with optional product filter.
    /// </summary>
    public class InventoryHistoryViewModel
    {
        /// <summary>
        /// Gets or sets the optional product filter.
        /// </summary>
        [Display(Name = "Product")]
        public int? ProductId { get; set; }

        /// <summary>
        /// Gets or sets the transactions shown.
        /// </summary>
        public IEnumerable<InventoryTransactionViewModel> Transactions { get; set; } = new List<InventoryTransactionViewModel>();

        /// <summary>
        /// Gets or sets the products used for the filter dropdown.
        /// </summary>
        public IEnumerable<SelectListItem> Products { get; set; } = new List<SelectListItem>();
    }
}