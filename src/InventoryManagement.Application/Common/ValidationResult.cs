using System.Collections.Generic;

namespace InventoryManagement.Application.Common
{
    /// <summary>
    /// Aggregate validation result returned by service-layer validators.
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Gets the list of validation errors keyed by field name.
        /// </summary>
        public Dictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>();

        /// <summary>
        /// Gets a value indicating whether the validated entity is valid.
        /// </summary>
        public bool IsValid => Errors.Count == 0;

        /// <summary>
        /// Adds a validation error for the specified field.
        /// </summary>
        /// <param name="field">The field name (typically the property name).</param>
        /// <param name="message">A human-readable error message.</param>
        public void AddError(string field, string message)
        {
            if (!Errors.ContainsKey(field))
            {
                Errors[field] = new[] { message };
            }
            else
            {
                var list = new List<string>(Errors[field]) { message };
                Errors[field] = list.ToArray();
            }
        }
    }
}
