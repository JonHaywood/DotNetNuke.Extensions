using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetNuke.Extensions.Data
{
    /// <summary>
    /// Provides a standard interface for DAOs which are data-access mechanism agnostic.
    ///
    /// Since nearly all of the domain objects you create will have a type of int Id, this
    /// base dao leverages this assumption. If you want an entity with a type
    /// other than int, such as string, then use <see cref="IRepositoryWithTypedId{T, IdT}" />.
    /// </summary>
    /// <typeparam name="T">Type entity this repository operates on.</typeparam>
    public interface IRepository<T> : IRepositoryWithTypedId<T, int> { }

    /// <summary>
    /// Provides a standard interface for DAOs which are data-access mechanism agnostic.
    /// </summary>
    /// <typeparam name="T">Type entity this repository operates on.</typeparam>
    /// <typeparam name="IdT">Type of the identifier for the entities used in this repository.</typeparam>
    public interface IRepositoryWithTypedId<T, IdT>
    {
        /// <summary>
        /// Returns null if a row is not found matching the provided Id.
        /// </summary>
        T Get(IdT id);

        /// <summary>
        /// Returns all of the items of a given type.
        /// </summary>
        IList<T> GetAll();

        /// <summary>
        /// Looks for zero or more instances using the filter provided.
        /// </summary>
        IList<T> FindAll(IFilter filter);

        /// <summary>
        /// Looks for a single instance using the filter provided.
        /// </summary>        
        T FindOne(IFilter filter);

        /// <summary>
        /// SaveOrUpdate may be called when saving or updating an entity.
        /// </summary>
        T SaveOrUpdate(T entity);

        /// <summary>
        /// I'll let you guess what this does.
        /// </summary>
        void Delete(T entity);
    }    
}
