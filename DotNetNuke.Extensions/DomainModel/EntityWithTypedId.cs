using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace DotNetNuke.Extensions.DomainModel
{
    /// <summary>
    /// For a discussion of this object, see
    /// http://devlicio.us/blogs/billy_mccafferty/archive/2007/04/25/using-equals-gethashcode-effectively.aspx
    /// </summary>
    [Serializable]
    public abstract class EntityWithTypedId<IdT> : ValidatableObject, IEntityWithTypedId<IdT>, IExtensibleDataObject
    {
        #region IEntityWithTypedId Members

        /// <summary>
        /// Id may be of type string, int, custom type, etc.
        /// Setter is protected to allow unit tests to set this property via reflection and to allow
        /// domain objects more flexibility in setting this for those objects with assigned Ids.
        /// It's virtual to allow NHibernate-backed objects to be lazily loaded.
        ///
        /// This is ignored for XML serialization because it does not have a public setter (which is very much by design).        
        /// </summary>
        [XmlIgnore]
        public virtual IdT Id { get; protected set; }

        /// <summary>
        /// Transient objects are not associated with an item already in storage. For instance,
        /// a Customer is transient if its Id is 0. It's virtual to allow NHibernate-backed
        /// objects to be lazily loaded.
        /// </summary>
        public virtual bool IsTransient()
        {
            return Id == null || Id.Equals(default(IdT));
        }

        #endregion

        #region Entity comparison support

        /// <summary>
        /// The property getter for SignatureProperties should ONLY compare the properties which make up
        /// the "domain signature" of the object.
        ///
        /// If you choose NOT to override this method (which will be the most common scenario),
        /// then you should decorate the appropriate property(s) with [DomainSignature] and they
        /// will be compared automatically. This is the preferred method of managing the domain
        /// signature of entity objects.
        /// </summary>
        /// <remarks>
        /// This ensures that the entity has at least one property decorated with the
        /// [DomainSignature] attribute.
        /// </remarks>
        protected override IEnumerable<PropertyInfo> GetTypeSpecificSignatureProperties()
        {
            return GetType().GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(DomainSignatureAttribute), true));
        }

        public override bool Equals(object obj)
        {
            EntityWithTypedId<IdT> compareTo = obj as EntityWithTypedId<IdT>;

            if (ReferenceEquals(this, compareTo))
                return true;

            if (compareTo == null || !GetType().Equals(compareTo.GetTypeUnproxied()))
                return false;

            if (HasSameNonDefaultIdAs(compareTo))
                return true;

            // Since the Ids aren't the same, both of them must be transient to
            // compare domain signatures; because if one is transient and the
            // other is a persisted entity, then they cannot be the same object.
            return IsTransient() && compareTo.IsTransient() &&
                HasSameObjectSignatureAs(compareTo);
        }

        public override int GetHashCode()
        {
            if (cachedHashcode.HasValue)
                return cachedHashcode.Value;

            if (IsTransient())
            {
                cachedHashcode = base.GetHashCode();
            }
            else
            {
                unchecked
                {
                    // It's possible for two objects to return the same hash code based on
                    // identically valued properties, even if they're of two different types,
                    // so we include the object's type in the hash calculation
                    int hashCode = GetType().GetHashCode();
                    cachedHashcode = (hashCode * HASH_MULTIPLIER) ^ Id.GetHashCode();
                }
            }

            return cachedHashcode.Value;
        }

        /// <summary>
        /// Returns true if self and the provided entity have the same Id values
        /// and the Ids are not of the default Id value
        /// </summary>
        private bool HasSameNonDefaultIdAs(EntityWithTypedId<IdT> compareTo)
        {
            return !IsTransient() &&
                  !compareTo.IsTransient() &&
                  Id.Equals(compareTo.Id);
        }

        private int? cachedHashcode;

        /// <summary>
        /// To help ensure hashcode uniqueness, a carefully selected random number multiplier
        /// is used within the calculation. Goodrich and Tamassia's Data Structures and
        /// Algorithms in Java asserts that 31, 33, 37, 39 and 41 will produce the fewest number
        /// of collissions. See http://computinglife.wordpress.com/2008/11/20/why-do-hash-functions-use-prime-numbers/
        /// for more information.
        /// </summary>
        private const int HASH_MULTIPLIER = 31;

        #endregion

        #region IExtensibleDataObject Members
        /// <summary>
        /// Gets or sets the structure that contains extra data.
        /// </summary>
        /// <returns>An <see cref="T:System.Runtime.Serialization.ExtensionDataObject"/> that contains data that is not recognized as belonging to the data contract.</returns>
        public virtual ExtensionDataObject ExtensionData
        {
            get;
            set;
        }
        #endregion
    }
}
