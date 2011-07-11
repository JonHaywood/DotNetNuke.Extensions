using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DotNetNuke.Extensions.Data
{
    public static class DataExtensions
    {
        /// <summary>
        /// Simple recordset field routines. Returns results as type value.
        /// </summary>
        /// <typeparam name="T">Type to return as.</typeparam>
        /// <param name="rs">Datareader.</param>
        /// <param name="fieldName">Fieldname to retrieve.</param>
        /// <returns>Value of field.</returns>
        public static T Field<T>(this IDataReader rs, string fieldName) where T : struct
        {
            int idx = rs.GetOrdinal(fieldName);
            object returnValue = null;
            if (typeof(T) == typeof(bool))
            {
                if (rs.IsDBNull(idx))
                {
                    returnValue = false;
                }
                else
                {
                    String s = rs[fieldName].ToString();

                    returnValue = (s.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase) ||
                            s.Equals("YES", StringComparison.InvariantCultureIgnoreCase) ||
                            s.Equals("1", StringComparison.InvariantCultureIgnoreCase));
                }
            }
            else if (typeof(T) == typeof(Guid))
            {
                if (rs.IsDBNull(idx))
                    returnValue = Guid.Empty;
                else
                    returnValue = rs.GetGuid(idx);
            }
            else if (typeof(T) == typeof(int))
            {
                if (rs.IsDBNull(idx))
                    returnValue = 0;
                else
                    returnValue = rs.GetInt32(idx);
            }
            else if (typeof(T) == typeof(double))
            {
                if (rs.IsDBNull(idx))
                    returnValue = 0.0F;
                else
                    returnValue = rs.GetDouble(idx);
            }
            else if (typeof(T) == typeof(decimal))
            {
                if (rs.IsDBNull(idx))
                    returnValue = System.Decimal.Zero;
                else
                    returnValue = rs.GetDecimal(idx);
            }
            else if (typeof(T) == typeof(DateTime))
            {
                if (rs.IsDBNull(idx))
                    returnValue = System.DateTime.MinValue;
                else
                    returnValue = Convert.ToDateTime(rs[idx], System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                throw new ArgumentOutOfRangeException(string.Format("Provided type '{0}' is not implemented.", typeof(T).Name));
            }

            return (T)Convert.ChangeType(returnValue, typeof(T));
        }
    }
}
