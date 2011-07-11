using System;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace DotNetNuke.Extensions.DomainModel
{
    /// <summary>
    /// Facilitates indicating which property(s) describe the unique signature of an
    /// entity. See Entity.GetTypeSpecificSignatureProperties() for when this is leveraged.
    /// </summary>
    /// <remarks>
    /// This is intended for use with <see cref="Entity" />. It may NOT be used on a <see cref="ValueObject"/>.
    /// </remarks>
    [Serializable]
    public class DomainSignatureAttribute : Attribute { }

    /// <summary>
    /// Provides a base class for your objects which will be persisted to the database.
    /// Benefits include the addition of an Id property along with a consistent manner for comparing
    /// entities.
    ///
    /// Since nearly all of the entities you create will have a type of int Id, this
    /// base class leverages this assumption. If you want an entity with a type other
    /// than int, such as string, then use <see cref="EntityWithTypedId{IdT}" /> instead.
    /// </summary>
    [Serializable]
    public abstract class Entity : EntityWithTypedId<int> { }
}
