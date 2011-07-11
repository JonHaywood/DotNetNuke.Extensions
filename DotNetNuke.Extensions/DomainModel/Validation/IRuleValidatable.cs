using System.Collections.Generic;

namespace DotNetNuke.Extensions.DomainModel.Validation
{
    /// <summary>
    /// Classes that implement this will be able to be validated using a common method.
    /// </summary>
    public interface IRuleValidatable
    {
        /// <summary>
        /// Returns true if the object passes all business rule checks.
        /// </summary>        
        bool IsValid();

        /// <summary>
        /// Gets a collection of business rule violations, if there are any.
        /// </summary>
        /// <returns>Collection of business rule violations.</returns>
        IEnumerable<IRuleViolation> GetRuleViolations();
    }
}
