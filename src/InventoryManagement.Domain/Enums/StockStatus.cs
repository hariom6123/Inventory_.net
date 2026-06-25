namespace InventoryManagement.Domain.Enums
{
    /// <summary>
    /// Identifies the stock health of a product.
    /// </summary>
    public enum StockStatus
    {
        /// <summary>
        /// Product has zero units available.
        /// </summary>
        OutOfStock = 0,

        /// <summary>
        /// Product quantity is below the configured low-stock threshold.
        /// </summary>
        LowStock = 1,

        /// <summary>
        /// Product quantity is above the configured low-stock threshold.
        /// </summary>
        InStock = 2
    }
}
