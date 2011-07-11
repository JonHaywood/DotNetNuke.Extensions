using System;
using System.Collections.Generic;

namespace DotNetNuke.Extensions.Linq
{
    /// <summary>
    /// This class allows us to use lambda expression where an IEqualityComparer object
    /// is needed. This particular class will compare two objects of the same type and
    /// comparer their keys. The key that is compared is extracted using a provided
    /// lambda function.
    /// </summary>
    /// <example><![CDATA[
    ///     IEnumberable<Customer> customers1 = GetCustomers1();
    ///     IEnumberable<Customer> customers2 = GetCustomers2();
    ///     
    ///     var customers = customers1.Except(customers2, new KeyEqualityComparer<Customer, int>(c => c.CustomerID));
    /// ]]>
    /// </example>
    /// <see cref="http://stackoverflow.com/questions/98033/wrap-a-delegate-in-an-iequalitycomparer/3719802#3719802"/>
    /// <typeparam name="T">The type being compared.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    public class KeyEqualityComparer<T, TKey> : IEqualityComparer<T>
    {
        protected readonly Func<T, TKey> KeyExtractor;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyEqualityComparer&lt;T, TKey&gt;"/> class.
        /// </summary>
        /// <param name="keyExtractor">The key extractor.</param>
        public KeyEqualityComparer(Func<T, TKey> keyExtractor)
        {
            KeyExtractor = keyExtractor;
        }

        /// <summary>
        /// Determines whether the specified objects are equal. Delegates the
        /// functionality to the function provided in the constructor. 
        /// </summary>
        /// <param name="x">The first object of type T to compare.</param>
        /// <param name="y">The second object of type T to compare.</param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        public virtual bool Equals(T x, T y)
        {
            return KeyExtractor(x).Equals(KeyExtractor(y));
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The type of obj is a reference type and obj is null.</exception>
        public int GetHashCode(T obj)
        {
            return KeyExtractor(obj).GetHashCode();
        }
    }
}
