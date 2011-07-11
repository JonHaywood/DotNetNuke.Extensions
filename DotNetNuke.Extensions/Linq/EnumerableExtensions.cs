using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DotNetNuke.Extensions.Linq
{
    /// <summary>
    /// Extension methods that add that deal with the IEnumerable class.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Converts an ArrayList into an IEnumerable collection.
        /// </summary>
        /// <typeparam name="T">Type of object contained in the array list.</typeparam>
        /// <param name="list">List to convert.</param>
        /// <returns>An IEnumerable collection.</returns>
        public static IEnumerable<T> ToEnumerable<T>(this ArrayList list)
        {
            foreach (T item in list)
                yield return item;
        }

        /// <summary>
        /// Joins list using a comma, serializing each item using ToString().
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>Joined keys.</returns>
        public static string Join<T, TKey>(IEnumerable<T> list)
        {
            return Join(list, ",", i => i.ToString());
        }

        /// <summary>
        /// Joins list using the specified separator, serializing each item using ToString().
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// /// <param name="list">The list.</param>
        /// <param name="separator">The separator.</param>        
        /// <returns>Joined keys.</returns>
        public static string Join<T, TKey>(IEnumerable<T> list, string separator)
        {
            return Join(list, separator, i => i.ToString());
        }

        /// <summary>
        /// Joins list using a comma and the provided function to select what
        /// property of each item in the collection to serialize.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>        
        /// <param name="list">The list.</param>
        /// <param name="keyExtractor">The key extractor.</param>
        /// <returns>Joined keys.</returns>
        public static string Join<T, TKey>(IEnumerable<T> list, Func<T, TKey> keyExtractor)
        {
            return Join(list, ",", keyExtractor);
        }

        /// <summary>
        /// Joins list using the specified separator and the provided function to select what
        /// property of each item in the collection to serialize.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="separator">The separator.</param>
        /// <param name="list">The list.</param>
        /// <param name="keyExtractor">The key extractor.</param>
        /// <returns>Joined keys.</returns>
        public static string Join<T, TKey>(this IEnumerable<T> list, string separator, Func<T, TKey> keyExtractor)
        {
            return string.Join(separator, list.Select(i => keyExtractor(i).ToString()).ToArray());
        }

        /// <summary>
        /// Extension method for Except LINQ extension method that allows us to use lambda 
        /// expression in our LINQ statements instead of custom classes that implement IEqualityComparer.
        /// </summary>
        /// <example><![CDATA[
        ///     List<AuthorInfo> list = GetCurrentAuthorList();
        ///     List<AuthorInfo> newAuthors = GetNewAuthorList();
        ///     
        ///     // we don't want duplicates - use Author ID to see if authors are the same
        ///     var uniqueAuthors = newAuthors.Except(list, a=> a.AuthorID);
        ///     AddAuthors(uniqueAuthors);
        /// ]]></example>
        public static IEnumerable<T> Except<T, TKey>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, TKey> keyExtractor)
        {
            return first.Except(second, new KeyEqualityComparer<T, TKey>(keyExtractor));
        }

        /// <summary>
        /// Extension method for distinct LINQ extension method that allows us to use lambda 
        /// expression in our LINQ statements instead of custom classes that implement IEqualityComparer.
        /// </summary>
        /// <example><![CDATA[
        ///     // list may have duplicates
        ///     List<AuthorInfo> list = GetAllAuthors();
        /// 
        ///     // use the author ID to tell if authors are the same
        ///     var uniqueAuthors = list.Distinct(a => a.AuthorID);
        /// ]]></example>
        public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> first, Func<T, TKey> keyExtractor)
        {
            return first.Distinct(new KeyEqualityComparer<T, TKey>(keyExtractor));
        }

        /// <summary>
        /// Extension method for contains LINQ extension method that allows us to use lambda 
        /// expression in our LINQ statements instead of custom classes that implement IEqualityComparer.
        /// </summary>
        /// <example><![CDATA[  
        ///     List<AuthorInfo> newAuthor = GetAuthor();
        ///     List<AuthorInfo> list = GetAllAuthors();
        /// 
        ///     // use the author ID to tell if authors are the same
        ///     bool hasAuthor = list.Contains(newAuthor, a => a.AuthorID); 
        /// ]]></example>
        public static bool Contains<T, TKey>(this IEnumerable<T> first, T value, Func<T, TKey> keyExtractor)
        {
            return first.Contains(value, new KeyEqualityComparer<T, TKey>(keyExtractor));
        }

        /// <summary>
        /// Extension method for intersect LINQ extension method that allows us to use lambda 
        /// expression in our LINQ statements instead of custom classes that implement IEqualityComparer.
        /// </summary>
        /// <example><![CDATA[  
        ///     List<SeriesInfo> allowedSeries = GetAllowedSeries(UserId);
        ///     List<SeriesInfo> contentSeries = GetContentSeries(ContentId);
        ///     
        ///     // only get series which user has access to, use series ID to tell if they are the same
        ///     var series = contentSeries.Intersect(allowedSeries, s => s.SeriesId);
        /// ]]></example>
        public static IEnumerable<T> Intersect<T, TKey>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, TKey> keyExtractor)
        {
            return first.Intersect(second, new KeyEqualityComparer<T, TKey>(keyExtractor));
        }
    }
}
