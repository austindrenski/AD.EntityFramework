using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace AD.EntityFramework
{
    /// <summary>
    /// Extension methods to assist in the local execuation of LINQ methods prior to database access.
    /// </summary>
    [PublicAPI]
    public static class ElementAtOrDefaultExtensions
    {
        /// <summary>
        /// Returns the element at the specified index or the default value. This method executes locally, prior to the SQL translation.
        /// </summary>
        [EntityFrameworkExtension]
        public static T ElementAtOrDefaultLocal<T>(this IEnumerable<T> source, int index)
        {
            if (source == null)
            {
                throw new ArgumentNullException();
            }
            if (index < 0)
            {
                return default(T);
            }
            T[] array = source as T[] ?? source.ToArray();
            return index < array.Length ? array[index] : default(T);
        }
    }
}