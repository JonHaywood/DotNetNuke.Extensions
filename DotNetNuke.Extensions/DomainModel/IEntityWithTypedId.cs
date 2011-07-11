using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace DotNetNuke.Extensions.DomainModel
{
    /// <summary>
    /// This serves as a base interface for <see cref="EntityWithTypedId"/> and
    /// <see cref="Entity"/>. Also provides a simple means to develop your own base entity.
    /// </summary>
    public interface IEntityWithTypedId<IdT>
    {
        /// <summary>
        /// The object serving as the main identifier for this domain object.
        /// </summary>
        IdT Id { get; }

        /// <summary>
        /// Transient objects are not associated with an item already in storage. For instance,
        /// a Customer is transient if its Id is 0. It's virtual to allow NHibernate-backed
        /// objects to be lazily loaded.
        /// </summary>
        /// <returns>True if the object is transient, otherwise false.</returns>
        bool IsTransient();

        /// <summary>
        /// The property getter for SignatureProperties should ONLY compare the properties which make up
        /// the "domain signature" of the object.
        /// </summary>
        /// <returns>Properties which make up the "domain signature" of the object.</returns>
        IEnumerable<PropertyInfo> GetSignatureProperties();
    }
}
