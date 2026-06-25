namespace InventoryManagement.Domain.Enums
{
    /// <summary>
    /// Defines the supported inventory transaction types.
    /// </summary>
    public enum TransactionType
    {
        /// <summary>
        /// Incoming stock that increases the available quantity.
        /// </summary>
        StockIn = 1,

        /// <summary>
        /// Outgoing stock that decreases the available quantity.
        /// </summary>
        StockOut = 2,

        /// <summary>
        /// Manual adjustment to align physical and system quantities.
        /// </summary>
        Adjustment = 3
    }
}
