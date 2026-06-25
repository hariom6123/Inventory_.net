namespace InventoryManagement.Application.DTOs
{
    /// <summary>
    /// Aggregated metrics shown on the dashboard.
    /// </summary>
    public class DashboardSummaryDto
    {
        /// <summary>
        /// Gets or sets the total number of distinct products.
        /// </summary>
        public int TotalProducts { get; set; }

        /// <summary>
        /// Gets or sets the total number of categories.
        /// </summary>
        public int TotalCategories { get; set; }

        /// <summary>
        /// Gets or sets the number of products below their low-stock threshold.
        /// </summary>
        public int LowStockProducts { get; set; }

        /// <summary>
        /// Gets or sets the number of products with zero quantity.
        /// </summary>
        public int OutOfStockProducts { get; set; }

        /// <summary>
        /// Gets or sets the total value of inventory (Price x Quantity).
        /// </summary>
        public decimal TotalInventoryValue { get; set; }
    }
}
