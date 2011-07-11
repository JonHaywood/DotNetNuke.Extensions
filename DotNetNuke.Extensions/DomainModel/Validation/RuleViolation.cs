using System;

namespace DotNetNuke.Extensions.DomainModel.Validation
{
    /// <summary>
    /// Basic class that encapsulates the violation of a business rule.
    /// </summary>
    [Serializable]
    public sealed class RuleViolation : IRuleViolation
    {
        #region Properties

        /// <summary>
        /// This is the class type that the validation result is applicable to. For instance,
        /// if the validation result concerns a duplicate record found for an employee, then
        /// this property would hold the typeof(Employee). It should be expected that this
        /// property will never be null.
        /// </summary>
        public Type ClassContext { get; private set; }

        /// <summary>
        /// If the validation result is applicable to a specific property, then this
        /// <see cref="PropertyInfo" /> would be set to a property name.
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Value of the property that violated a rule.
        /// </summary>
        public object PropertyValue { get; private set; }

        /// <summary>
        /// Holds the message describing the business rule violation.
        /// </summary>
        public string ErrorMessage { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleViolation"></see> class.
        /// </summary>
        public RuleViolation(Type classContext, string propertyName, string errorMessage)
            : this(classContext, propertyName, null, errorMessage)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleViolation"></see> class.
        /// </summary>
        public RuleViolation(Type classContext, string propertyName, object propertyValue, string errorMessage)
        {
            ClassContext = classContext;
            PropertyName = propertyName;
            PropertyValue = propertyValue;
            ErrorMessage = errorMessage;
        }

        #endregion
    }

}
