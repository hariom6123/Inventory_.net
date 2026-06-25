using System;

namespace InventoryManagement.Application.Common
{
    /// <summary>
    /// Thrown when a service-layer validation rule fails.
    /// </summary>
    public class ValidationException : Exception
    {
        /// <summary>
        /// Gets the aggregate validation result describing the failure.
        /// </summary>
        public ValidationResult Result { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        /// <param name="result">The aggregate validation result.</param>
        public ValidationException(ValidationResult result)
            : base("One or more validation errors occurred.")
        {
            Result = result;
        }
    }
}
