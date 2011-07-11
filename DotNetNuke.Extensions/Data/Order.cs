using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetNuke.Extensions.Data
{
    /// <summary>
    /// This class assists in building SQL queries in the form of 'Order By' clauses.
    /// </summary>
    public interface IOrder
    {
        string GetExpressionString();
    }

    /// <summary>
    /// This class assists in building SQL queries in the form of 'Order By' clauses.
    /// </summary>
    [Serializable]
    public class Order
    {
        protected StringBuilder OrderSql = new StringBuilder();

        /// <summary>
        /// Gets whether the Query instance has any order criteria.
        /// </summary>
        public bool HasExpression
        {
            get
            {
                return OrderSql.Length > 0;
            }
        }

        protected void AddCriteria(string propertyName, bool isAscending)
        {
            if (OrderSql.Length > 0)
                OrderSql.Append(", ");
            OrderSql.AppendFormat("{0} {1}", propertyName, (isAscending ? "asc" : "desc"));
        }

        public Order Asc(string propertyName)
        {
            AddCriteria(propertyName, true);
            return this;
        }

        public Order Desc(string propertyName)
        {
            AddCriteria(propertyName, false);
            return this;
        }

        /// <summary>
        /// Returns the query's expression as a SQL string, prefixed with the 'Order By' command.
        /// For example, <code>OrderBy Column1</code>.
        /// </summary>
        public override string ToString()
        {
            if (!HasExpression)
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            sb.Append("Order By ");
            sb.Append(GetExpressionString());
            return sb.ToString();
        }

        /// <summary>
        /// Returns the order by clause without the leading 'Order By' command.  To access the statement
        /// with a order by command, use <see cref="ToString()"/>.
        /// </summary>
        /// <returns>The order by clause without the leading 'Order By' command.</returns>
        public string GetExpressionString()
        {
            if (!HasExpression)
                return string.Empty;

            return OrderSql.ToString();
        }
    }
}
