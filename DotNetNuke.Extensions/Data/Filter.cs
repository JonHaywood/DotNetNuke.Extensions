using System;
using System.Collections;
using System.Text;

namespace DotNetNuke.Extensions.Data
{
    /// <summary>
    /// Interface for filter which will create a where sql clause.
    /// </summary>
    public interface IFilter
    {
        string GetExpressionString();
    }

    /// <summary>
    /// Represents a filter over a set of relational data.  This class assists in building SQL
    /// queries in the form of 'Where' clauses.
    /// </summary>
    /// <see cref="http://drewnoakes.com/code/util/Filter.cs.txt"/>
    [Serializable]
    public class Filter : IFilter
    {
        #region Fields
        /// <summary>
        /// The default date format string used for SQL commands.
        /// </summary>
        const string DateFormat = "dd MMM yyyy HH:mm:ss";

        ConditionNode _topNode;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructs a new, empty, query.
        /// </summary>
        public Filter()
        {
            _topNode = null;
        }
        #endregion

        #region CreateEmpty

        /// <summary>
        /// Gets an empty query.
        /// </summary>
        public static Filter CreateEmpty()
        {
            return new Filter();
        }


        #endregion

        #region Equals

        /// <summary>
        /// Extends the Query (using the AND operator) such that only rows with values equal to
        /// that specified in the named column should be included.
        /// </summary>
        /// <param name="columnName">The column to filter within.</param>
        /// <param name="lessThan">The value that remaining rows must be equal to.</param>
        /// <returns>This Query instance, such that multiple methods may be chained together for convenience.</returns>
        public Filter EqualTo(string columnName, object equalTo)
        {
            return And(CreateEqualToCondition(columnName, equalTo));
        }

        public Filter OrEqualTo(string columnName, object equalTo)
        {
            return Or(CreateEqualToCondition(columnName, equalTo));
        }

        private ConditionNode CreateEqualToCondition(string columnName, object equalTo)
        {
            ConditionNode condition
                = new OperatorNode(new ColumnNameNode(columnName), new ValueNode(equalTo), "=");
            return condition;
        }


        #endregion

        #region NotEqualTo

        /// <summary>
        /// Extends the Query (using the AND operator) such that only rows with values not equal to
        /// that specified in the named column should be included.
        /// </summary>
        /// <param name="columnName">The column to filter within.</param>
        /// <param name="lessThan">The value that remaining rows must not be equal to.</param>
        /// <returns>This Query instance, such that multiple methods may be chained together for convenience.</returns>
        public Filter NotEqualTo(string columnName, object notEqualTo)
        {
            return And(CreateNotEqualToCondition(columnName, notEqualTo));

        }

        public Filter OrNotEqualTo(string columnName, object notEqualTo)
        {
            return Or(CreateNotEqualToCondition(columnName, notEqualTo));
        }

        private ConditionNode CreateNotEqualToCondition(string columnName, object notEqualTo)
        {
            ConditionNode condition
                = new OperatorNode(new ColumnNameNode(columnName), new ValueNode(notEqualTo), "<>");
            return condition;
        }

        #endregion

        #region GreaterThan

        /// <summary>
        /// Extends the Query (using the AND operator) such that only rows with values greater than
        /// that specified in the named column should be included.
        /// </summary>
        /// <param name="columnName">The column to filter within.</param>
        /// <param name="lessThan">The value that remaining rows must be greater than.</param>
        /// <returns>This Query instance, such that multiple methods may be chained together for convenience.</returns>
        public Filter GreaterThan(string columnName, object greaterThan)
        {
            return And(CreateGreaterThanCondition(columnName, greaterThan));
        }

        public Filter OrGreaterThan(string columnName, object greaterThan)
        {
            return Or(CreateGreaterThanCondition(columnName, greaterThan));
        }

        private ConditionNode CreateGreaterThanCondition(string columnName, object greaterThan)
        {
            ConditionNode condition
                = new OperatorNode(new ColumnNameNode(columnName), new ValueNode(greaterThan), ">");
            return condition;
        }

        #endregion

        #region GreaterThanOrEqualTo

        /// <summary>
        /// Extends the Query (using the AND operator) such that only rows with values greater than or equal to
        /// that specified in the named column should be included.
        /// </summary>
        /// <param name="columnName">The column to filter within.</param>
        /// <param name="lessThan">The value that remaining rows must be greater than or equal to.</param>
        /// <returns>This Query instance, such that multiple methods may be chained together for convenience.</returns>
        public Filter GreaterThanOrEqualTo(string columnName, object greaterThanOrEqualTo)
        {
            return And(CreateGreaterThanOrEqualToCondition(columnName, greaterThanOrEqualTo));
        }

        public Filter OrGreaterThanOrEqualTo(string columnName, object greaterThanOrEqualTo)
        {
            return Or(CreateGreaterThanOrEqualToCondition(columnName, greaterThanOrEqualTo));
        }

        private ConditionNode CreateGreaterThanOrEqualToCondition(string columnName, object greaterThanOrEqualTo)
        {
            ConditionNode condition
                = new OperatorNode(new ColumnNameNode(columnName), new ValueNode(greaterThanOrEqualTo), ">=");
            return condition;
        }


        #endregion

        #region LessThan

        /// <summary>
        /// Extends the Query (using the AND operator) such that only rows with values less than
        /// that specified in the named column should be included.
        /// </summary>
        /// <param name="columnName">The column to filter within.</param>
        /// <param name="lessThan">The value that remaining rows must be less than.</param>
        /// <returns>This Query instance, such that multiple methods may be chained together for convenience.</returns>
        public Filter LessThan(string columnName, object lessThan)
        {
            return And(CreateLessThanCondition(columnName, lessThan));
        }

        public Filter OrLessThan(string columnName, object lessThan)
        {
            return Or(CreateLessThanCondition(columnName, lessThan));
        }

        private ConditionNode CreateLessThanCondition(string columnName, object lessThan)
        {
            ConditionNode condition
                = new OperatorNode(new ColumnNameNode(columnName), new ValueNode(lessThan), "<");
            return condition;
        }

        #endregion

        #region LessThanOrEqualTo

        /// <summary>
        /// Extends the Query (using the AND operator) such that only rows with values less than or equal to
        /// that specified in the named column should be included.
        /// </summary>
        /// <param name="columnName">The column to filter within.</param>
        /// <param name="lessThanOrEqualTo">The value that remaining rows must be less than or equal to.</param>
        /// <returns>This Query instance, such that multiple methods may be chained together for convenience.</returns>
        public Filter LessThanOrEqualTo(string columnName, object lessThanOrEqualTo)
        {
            return And(CreateLessThanOrEqualToCondition(columnName, lessThanOrEqualTo));
        }

        public Filter OrLessThanOrEqualTo(string columnName, object lessThanOrEqualTo)
        {
            return Or(CreateLessThanOrEqualToCondition(columnName, lessThanOrEqualTo));
        }

        private ConditionNode CreateLessThanOrEqualToCondition(string columnName, object lessThanOrEqualTo)
        {
            ConditionNode condition
                = new OperatorNode(new ColumnNameNode(columnName), new ValueNode(lessThanOrEqualTo), "<=");
            return condition;
        }


        #endregion

        #region Like

        /// <summary>
        /// Performs wildcard string matching upon the specified column.
        /// </summary>
        /// <param name="columnName">The column to match within.</param>
        /// <param name="likePattern">The wildcard expression.  Patterns commonly use the % symbol as a wildcard.</param>
        /// <returns>This Query instance, such that multiple methods may be chained together for convenience.</returns>
        public Filter Like(string columnName, string likePattern)
        {
            return And(CreateLikeCondition(columnName, likePattern));
        }

        public Filter OrLike(string columnName, string likePattern)
        {
            return Or(CreateLikeCondition(columnName, likePattern));
        }

        private ConditionNode CreateLikeCondition(string columnName, string likePattern)
        {
            ConditionNode condition
                = new OperatorNode(new ColumnNameNode(columnName), new ValueNode(likePattern), " Like ");
            return condition;
        }

        #endregion

        #region ContainsString

        public Filter ContainsString(string columnName, string subString)
        {
            return And(CreateContainsStringCondition(subString, columnName));
        }

        public Filter OrContainsString(string columnName, string subString)
        {
            return Or(CreateContainsStringCondition(subString, columnName));
        }

        private ConditionNode CreateContainsStringCondition(string subString, string columnName)
        {
            string likePattern = string.Format("%{0}%", subString);
            ConditionNode condition
                = new OperatorNode(new ColumnNameNode(columnName), new ValueNode(likePattern), " Like ");
            return condition;
        }

        #endregion

        #region StartsWithString

        public Filter StartsWithString(string columnName, string prefix)
        {
            return And(CreateStartsWithStringCondition(prefix, columnName));
        }

        public Filter OrStartsWithString(string columnName, string prefix)
        {
            return Or(CreateStartsWithStringCondition(prefix, columnName));
        }

        private ConditionNode CreateStartsWithStringCondition(string prefix, string columnName)
        {
            string likePattern = string.Format("{0}%", prefix);
            ConditionNode condition
                = new OperatorNode(new ColumnNameNode(columnName), new ValueNode(likePattern), " Like ");
            return condition;
        }

        #endregion

        #region ContainsStringWithinCommaSeparatedValues

        public Filter ContainsStringWithinCommaSeparatedValues(string columnName, string csvValue)
        {
            return And(CreateContainsStringWithinCommaSeparatedValuesCondition(columnName, csvValue));
        }

        public Filter OrContainsStringWithinCommaSeparatedValues(string columnName, string csvValue)
        {
            return Or(CreateContainsStringWithinCommaSeparatedValuesCondition(columnName, csvValue));
        }

        private ConditionNode CreateContainsStringWithinCommaSeparatedValuesCondition(string columnName, string csvValue)
        {
            Filter likeFilter = new Filter();
            likeFilter.EqualTo(columnName, csvValue);
            likeFilter.OrLike(columnName, string.Format("{0},%", csvValue));
            likeFilter.OrLike(columnName, string.Format("%,{0}", csvValue));
            likeFilter.OrLike(columnName, string.Format("%,{0},%", csvValue));
            ConditionNode condition = likeFilter._topNode;
            return condition;
        }

        #endregion

        #region IsNotNull

        public Filter IsNotNull(string columnName)
        {
            return And(CreateIsNotNullCondition(columnName));
        }

        public Filter OrIsNotNull(string columnName)
        {
            return Or(CreateIsNotNullCondition(columnName));
        }

        private ConditionNode CreateIsNotNullCondition(string columnName)
        {
            ConditionNode condition
                = new NullCheckingNode(new ColumnNameNode(columnName), false);
            return condition;
        }

        #endregion

        #region IsNull

        public Filter IsNull(string columnName)
        {
            return And(CreateIsNullCondition(columnName));
        }

        public Filter OrIsNull(string columnName)
        {
            return Or(CreateIsNullCondition(columnName));
        }

        private ConditionNode CreateIsNullCondition(string columnName)
        {
            ConditionNode condition
                = new NullCheckingNode(new ColumnNameNode(columnName), true);
            return condition;
        }

        #endregion

        #region ToString and GetExpressionString

        /// <summary>
        /// Returns the query's expression as a SQL string, prefixed with the 'Where' command.
        /// For example, <code>Where Column1='Value1'</code>.
        /// </summary>
        public override string ToString()
        {
            if (!HasExpression)
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            sb.Append("Where ");
            sb.Append(GetExpressionString());
            return sb.ToString();
        }


        /// <summary>
        /// Returns the where clause without the leading 'Where' command.  To access the statement
        /// with a where command, use <see cref="ToString()"/>.
        /// </summary>
        /// <returns>The where clause without the leading 'Where' command.</returns>
        public string GetExpressionString()
        {
            if (!HasExpression)
                return string.Empty;

            return _topNode.GetExpressionString();
        }


        #endregion

        #region And & Or operator support

        /// <summary>
        /// Adds the specified sub-query using the OR logical operator.
        /// </summary>
        /// <param name="innerQuery">The sub-query to be added via the OR operator.</param>
        /// <returns>This Query instance, such that multiple methods may be chained together for convenience.</returns>
        public Filter Or(params Filter[] innerQueries)
        {
            foreach (Filter innerQuery in innerQueries)
            {
                if (innerQuery._topNode != null)
                    Or(innerQuery._topNode);
            }

            return this;
        }

        Filter Or(ConditionNode newCondition)
        {
            if (_topNode == null)
            {
                _topNode = newCondition;
            }
            else
            {
                MergeConditionIntoTree(LogicalOperator.OR, newCondition);
            }
            return this;
        }

        public Filter And(params Filter[] innerQueries)
        {
            foreach (Filter innerQuery in innerQueries)
            {
                if (innerQuery._topNode != null)
                    And(innerQuery._topNode);
            }

            return this;
        }

        Filter And(ConditionNode newCondition)
        {
            if (newCondition == null)
                throw new ArgumentNullException("newCondition", "condition may not be null");

            if (_topNode == null)
            {
                _topNode = newCondition;
            }
            else
            {
                MergeConditionIntoTree(LogicalOperator.AND, newCondition);
            }
            return this;
        }

        void MergeConditionIntoTree(LogicalOperator desiredLogicalOperator, ConditionNode newCondition)
        {
            if (newCondition == null)
                throw new ArgumentNullException("newCondition", "condition may not be null");

            if (_topNode is LogicalNode && ((LogicalNode)_topNode).Operator == desiredLogicalOperator)
            {
                // top node is already of the desired type, so just add into it
                LogicalNode logicalNode = _topNode as LogicalNode;
                logicalNode.AddCondition(newCondition);
            }
            else
            {
                // push existing top node down to be beneath this node...
                LogicalNode newTopNode = new LogicalNode(desiredLogicalOperator);
                newTopNode.AddCondition(_topNode);
                newTopNode.AddCondition(newCondition);
                _topNode = newTopNode;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether the Query instance has any conditions set, and hence whether
        /// <see cref="GetExpression()"/> would return a non-zero length string.
        /// </summary>
        public bool HasExpression
        {
            get
            {
                return _topNode != null;
            }
        }

        #endregion

        #region Node classes

        /// <summary>
        /// Private enumeration of all supported logical operators for combination of
        /// condition nodes.
        /// </summary>
        enum LogicalOperator
        {
            AND,
            OR
        }

        abstract class ConditionNode
        {
            public abstract string GetExpressionString();
        }

        class LogicalNode : ConditionNode
        {
            ArrayList _childNodes; // may contain only ConditionNode entries...
            LogicalOperator _operator; // eg. AND or OR

            public LogicalNode(LogicalOperator logicalOperator)
            {
                _operator = logicalOperator;
                _childNodes = new ArrayList();
            }

            public override string GetExpressionString()
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < _childNodes.Count; i++)
                {
                    if (i > 0)
                    {
                        sb.AppendFormat(" {0} ", OperatorName);
                    }
                    ConditionNode child = (ConditionNode)_childNodes[i];

                    string formatString;
                    if (child is LogicalNode)
                        formatString = "({0})";
                    else
                        formatString = "{0}";

                    sb.AppendFormat(formatString, child.GetExpressionString());
                }
                return sb.ToString();
            }

            public void AddCondition(ConditionNode condition)
            {
                if (condition == null)
                    throw new ArgumentNullException("condition", "Cannot add a null condition.");

                _childNodes.Add(condition);
            }

            public LogicalOperator Operator
            {
                get
                {
                    return _operator;
                }
            }

            string OperatorName
            {
                get
                {
                    if (_operator == LogicalOperator.AND)
                        return "And";
                    else if (_operator == LogicalOperator.OR)
                        return "Or";
                    else
                        throw new Exception("Unsupported logical operator: " + _operator);
                }
            }
        }

        class OperatorNode : ConditionNode
        {
            string _operator;
            LiteralNode _left;
            LiteralNode _right;

            public OperatorNode(LiteralNode left, LiteralNode right, string operatorString)
            {
                _left = left;
                _right = right;
                _operator = operatorString;
            }

            public override string GetExpressionString()
            {
                return string.Format("{0}{1}{2}", _left.ToString(), _operator, _right.ToString());
            }
        }

        class NullCheckingNode : ConditionNode
        {
            bool _mustBeNull;
            ColumnNameNode _column;
            public NullCheckingNode(ColumnNameNode column, bool desireNull)
            {
                _column = column;
                _mustBeNull = desireNull;
            }
            public override string GetExpressionString()
            {
                if (_mustBeNull)
                    return string.Format("{0} Is Null", _column.ToString());
                else
                    return string.Format("{0} Is Not Null", _column.ToString());
            }
        }


        abstract class LiteralNode
        {
        }

        class ValueNode : LiteralNode
        {
            /// <summary>
            /// The default date format string used for SQL commands.
            /// </summary>
            const string DateFormat = "dd MMM yyyy HH:mm:ss";

            object _value;
            public ValueNode(object val)
            {
                _value = val;
            }
            public override string ToString()
            {
                return FormatValue(_value);
            }
            string FormatValue(object val)
            {
                if (val is DateTime)
                {
                    return string.Format("'{0}'", ((DateTime)val).ToString(DateFormat));
                }
                else if (val is int)
                {
                    return string.Format("{0}", val);
                }
                else
                {
                    EscapeString(ref val);
                    return string.Format("'{0}'", val);
                }
            }

            void EscapeString(ref object val)
            {
                val = val.ToString().Replace("'", "''");
            }
        }

        class ColumnNameNode : LiteralNode
        {
            string _columnName;
            public ColumnNameNode(string columnName)
            {
                _columnName = columnName;
            }
            public override string ToString()
            {
                return string.Format("[{0}]", _columnName);
            }
        }


        #endregion

        #region Equals & GetHashCode

        public override bool Equals(object obj)
        {
            Filter that = obj as Filter;

            if (that == null)
                return false;

            return this.GetExpressionString().Equals(that.GetExpressionString());
        }

        public override int GetHashCode()
        {
            return GetExpressionString().GetHashCode();
        }



        #endregion
    }
}
