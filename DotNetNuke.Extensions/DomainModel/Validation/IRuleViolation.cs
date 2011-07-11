using System;

namespace DotNetNuke.Extensions.DomainModel.Validation
{
    /// <summary>
    /// Encapsulates data resulting from the violation of a business rule.
    /// </summary>
    public interface IRuleViolation
    {
        /// <summary>
        /// This is the class type that the validation result is applicable to. For instance,
        /// if the validation result concerns a duplicate record found for an employee, then
        /// this property would hold the typeof(Employee). It should be expected that this
        /// property will never be null.
        /// </summary>
        Type ClassContext { get; }

        /// <summary>
        /// If the validation result is applicable to a specific property, then this
        /// <see cref="PropertyInfo" /> would be set to a property name.
        /// </summary>
        string PropertyName { get; }

        /// <summary>
        /// Value of the property that violated a rule.
        /// </summary>
        object PropertyValue { get; }

        /// <summary>
        /// Holds the message describing the business rule violation.
        /// </summary>
        string ErrorMessage { get; }
    }
}
