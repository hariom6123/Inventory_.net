using System.Collections.Generic;
using InventoryManagement.Application.DTOs;

namespace InventoryManagement.Web.ViewModels
{
    /// <summary>
    /// Backing model for the Dashboard page.
    /// </summary>
    public class DashboardViewModel
    {
        /// <summary>
        /// Gets or sets the dashboard summary.
        /// </summary>
        public DashboardSummaryDto Summary { get; set; } = new();

        /// <summary>
        /// Gets or sets the recent transactions shown on the dashboard.
        /// </summary>
        public IEnumerable<InventoryTransactionViewModel> RecentTransactions { get; set; } = new List<InventoryTransactionViewModel>();
    }
}
