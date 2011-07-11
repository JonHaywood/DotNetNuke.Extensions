using System;
using System.Collections.Generic;
using DotNetNuke.Extensions.DomainModel.Validation;
using DotNetNuke.Extensions.ServiceLocation;

namespace DotNetNuke.Extensions.DomainModel
{
    /// <summary>
    /// Inherited classes 
    /// </summary>
    [Serializable]
    public abstract class ValidatableObject : BaseObject, IRuleValidatable
    {
        /// <summary>
        /// Reference to the validator to use for this class.
        /// </summary>
        protected virtual IRuleValidator Validator
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #region IRuleValidatable Members

        /// <summary>
        /// Returns true if the object passes all business rule checks.
        /// </summary>
        /// <returns></returns>
        public virtual bool IsValid()
        {
            return Validator.IsValid(this);
        }

        /// <summary>
        /// Gets a collection of business rule violations, if there are any.
        /// </summary>
        /// <returns>
        /// Collection of business rule violations.
        /// </returns>
        public IEnumerable<IRuleViolation> GetRuleViolations()
        {
            return Validator.ValidationResultsFor(this);
        }

        #endregion
    }
}
