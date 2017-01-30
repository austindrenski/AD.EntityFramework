using System.Linq;
using JetBrains.Annotations;

namespace AD.EntityFramework
{
    /// <summary>
    /// Extension methods to translate extension methods into expression trees for consumption by Entity Framework.
    /// </summary>
    [PublicAPI]
    public static class ReduceExpressionExtensions  
    {
        /// <summary>
        /// Reduces expression complexity by inlining method calls marked with the EntityFrameworkExtensionAttribute.
        /// </summary>
        /// <typeparam name="T">The type of element in the IQueryable.</typeparam>
        /// <param name="queryable">The IQueryable to reduce.</param>
        /// <returns>An IQueryable with calls to methods marked with the EntityFrameworkExtensionAttribute removed from the internal expression.</returns>
        public static IQueryable<T> ReduceExpression<T>(this IQueryable<T> queryable)
        {
            return queryable as EntityFrameworkExtensionQueryable<T> ?? new EntityFrameworkExtensionQueryable<T>(queryable);
        }
    }
}
