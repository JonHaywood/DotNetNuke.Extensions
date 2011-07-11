using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetNuke.Extensions.DomainModel;

namespace DotNetNuke.Extensions.Data
{
    /// <summary>
    /// Class that serves as a base class for the repository design pattern. Uses
    /// Massive to interface with the database.
    /// </summary>
    /// <typeparam name="T">Type that this repository will operate on.</typeparam>
    public class BaseRepository<T> : IPagedRepository<T> where T : Entity, new()
    {
        /// <summary>
        /// Reference to Massive which will search as our data access technology.
        /// </summary>
        protected readonly DynamicModel Model;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRepository&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="primaryKeyField">The primary key field for the table.</param>
        public BaseRepository(string tableName, string primaryKeyField = "")
        {
            Model = new DynamicModel(tableName, primaryKeyField);
        }

        /// <summary>
        /// Returns null if a row is not found matching the provided Id.
        /// </summary>
        public T Get(int id)
        {
            return Model.Single(id).Convert<T>();
        }

        /// <summary>
        /// Returns all of the items of a given type.
        /// </summary>
        public IList<T> GetAll()
        {
            var results = Model.All();
            return results.Convert<T>().ToList();
        }

        /// <summary>
        /// Returns all instances in a paged format.
        /// </summary>
        public IList<T> GetAllPaged(int pageIndex, int pageSize, IOrder sortCriterion, out int count)
        {
            var results = Model.Paged(
                orderBy: sortCriterion.GetExpressionString(),
                pageSize: pageSize,
                currentPage: pageIndex);

            // get paged variables
            IEnumerable<object> items = results.Items;
            int totalPages = results.TotalPages;
            int totalRecords = results.TotalRecords;

            // set count
            count = totalRecords;

            return items.Convert<T>().ToList();
        }

        /// <summary>
        /// Looks for zero or more instances using the filter provided.
        /// </summary>
        public IList<T> FindAll(IFilter filter)
        {
            var results = Model.All(where: filter.GetExpressionString());
            return results.Convert<T>().ToList();
        }

        /// <summary>
        /// Finds all instances in a paged format.
        /// </summary>
        public IList<T> FindAllPaged(int pageIndex, int pageSize, IFilter filter, IOrder sortCriterion, out int count)
        {
            var results = Model.Paged(
                where: filter.GetExpressionString(),
                orderBy: sortCriterion.GetExpressionString(), 
                pageSize: pageSize, 
                currentPage: pageIndex);

            // get paged variables
            IEnumerable<object> items = results.Items;
            int totalPages = results.TotalPages;
            int totalRecords = results.TotalRecords;

            // set count
            count = totalRecords;

            return items.Convert<T>().ToList();
        }

        /// <summary>
        /// Looks for a single instance using the filter provided.
        /// </summary>        
        public T FindOne(IFilter filter)
        {
            return FindAll(filter).FirstOrDefault();
        }

        /// <summary>
        /// SaveOrUpdate may be called when saving or updating an entity.
        /// </summary>
        public T SaveOrUpdate(T entity)
        {
            Model.Save(entity);
            return entity;
        }

        /// <summary>
        /// I'll let you guess what this does.
        /// </summary>
        public void Delete(T entity)
        {
            Model.Delete(entity.Id);
        }        
    }
}
