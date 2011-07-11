using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetNuke.Extensions.Data
{
    /// <summary>
    /// Extends the <see cref="IPagedRepositoryWithTypedId{T, IdT}" /> to add paging functionality.
    /// 
    /// Since nearly all of the domain objects you create will have a type of int Id, this
    /// base dao leverages this assumption. If you want an entity with a type
    /// other than int, such as string, then use <see cref="IPagedRepositoryWithTypedId{T, IdT}" />.
    /// 
    /// Note: this class extends IRepository and not IPagedRepositoryWithTypedId. This is so
    /// we can treat a paged repository the same as a regular repository.
    /// </summary>
    /// <typeparam name="T">Type entity this repository operates on.</typeparam>
    public interface IPagedRepository<T> : IRepository<T> 
    {
        /// <summary>
        /// Finds all instances in a paged format.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="sortCriterion">The sort criterion.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        IList<T> FindAllPaged(int pageIndex, int pageSize, IFilter filter, IOrder sortCriterion, out int count);


        /// <summary>
        /// Returns all instances in a paged format.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortCriterion">The sort criterion.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        IList<T> GetAllPaged(int pageIndex, int pageSize, IOrder sortCriterion, out int count);
    }

    /// <summary>
    /// Extends the <see cref="IRepositoryWithTypedId{T, IdT}" /> to add paging functionality.
    /// </summary>
    /// <typeparam name="T">Type entity this repository operates on.</typeparam>
    /// <typeparam name="IdT">Type of the identifier for the entities used in this repository.</typeparam>
    public interface IPagedRepositoryWithTypedId<T, IdT> : IRepositoryWithTypedId<T, IdT>
    {
        /// <summary>
        /// Finds all instances in a paged format.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="sortCriterion">The sort criterion.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        IList<T> FindAllPaged(int pageIndex, int pageSize, IFilter filter, IOrder sortCriterion, out int count);


        /// <summary>
        /// Returns all instances in a paged format.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortCriterion">The sort criterion.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        IList<T> GetAllPaged(int pageIndex, int pageSize, IOrder sortCriterion, out int count);
    }
}
