using System.Collections.Generic;

namespace DotNetNuke.Extensions.DomainModel.Validation
{
    /// <summary>
    /// Interface for classes that can validate an object to see if it meets it's
    /// particular business rules.
    /// </summary>
    public interface IRuleValidator
    {
        /// <summary>
        /// Returns true if the object passes all business rule checks.
        /// </summary> 
        bool IsValid(object value);

        /// <summary>
        /// Gets a collection of business rule violations for the provided object, 
        /// if there are any.
        /// </summary>
        /// <returns>Collection of business rule violations.</returns>
        IEnumerable<IRuleViolation> ValidationResultsFor(object value);
    }
}
