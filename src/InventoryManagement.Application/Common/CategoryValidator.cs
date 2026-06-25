using InventoryManagement.Application.DTOs;

namespace InventoryManagement.Application.Common
{
    /// <summary>
    /// Performs business-rule validation on category DTOs.
    /// </summary>
    public static class CategoryValidator
    {
        /// <summary>
        /// Validates the supplied category DTO against the system's business rules.
        /// </summary>
        /// <param name="category">The category DTO to validate.</param>
        public static ValidationResult Validate(CategoryDto category)
        {
            var result = new ValidationResult();

            if (string.IsNullOrWhiteSpace(category.CategoryName))
            {
                result.AddError(nameof(category.CategoryName), "Category name is required.");
            }
            else if (category.CategoryName.Length > 100)
            {
                result.AddError(nameof(category.CategoryName), "Category name cannot exceed 100 characters.");
            }

            if (category.Description?.Length > 500)
            {
                result.AddError(nameof(category.Description), "Description cannot exceed 500 characters.");
            }

            return result;
        }
    }
}
