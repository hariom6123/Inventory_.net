using InventoryManagement.Application.DTOs;

namespace InventoryManagement.Application.Common
{
    /// <summary>
    /// Performs business-rule validation on product DTOs.
    /// </summary>
    public static class ProductValidator
    {
        /// <summary>
        /// Validates the supplied product DTO against the system's business rules.
        /// </summary>
        /// <param name="product">The product DTO to validate.</param>
        public static ValidationResult Validate(ProductDto product)
        {
            var result = new ValidationResult();

            if (string.IsNullOrWhiteSpace(product.ProductName))
            {
                result.AddError(nameof(product.ProductName), "Product name is required.");
            }
            else if (product.ProductName.Length > 200)
            {
                result.AddError(nameof(product.ProductName), "Product name cannot exceed 200 characters.");
            }

            if (string.IsNullOrWhiteSpace(product.SKU))
            {
                result.AddError(nameof(product.SKU), "SKU is required.");
            }
            else if (product.SKU.Length > 50)
            {
                result.AddError(nameof(product.SKU), "SKU cannot exceed 50 characters.");
            }

            if (product.Price <= 0)
            {
                result.AddError(nameof(product.Price), "Price must be greater than zero.");
            }

            if (product.Quantity < 0)
            {
                result.AddError(nameof(product.Quantity), "Quantity cannot be negative.");
            }

            if (product.LowStockThreshold < 0)
            {
                result.AddError(nameof(product.LowStockThreshold), "Low-stock threshold cannot be negative.");
            }

            if (product.CategoryId <= 0)
            {
                result.AddError(nameof(product.CategoryId), "A valid category is required.");
            }

            return result;
        }
    }
}
