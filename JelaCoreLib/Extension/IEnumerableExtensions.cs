using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JelaCoreLib.Extension
{
    /// <summary>
    /// Define extension methods for IEnumerable.
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Return IEnumerable itself, or define empty if variable is null.
        /// </summary>
        /// <typeparam name="T">Type of the class that IEnumerable is supposed to store.</typeparam>
        /// <param name="source">Original instance of the IEnumerable.</param>
        /// <returns>The original IEnumerable, or empty if original value was null.</returns>
        public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }
    }
}
