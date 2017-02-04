using System;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace AD.EntityFramework
{
    /// <summary>
    /// Extension methods to perform left outer joins between entities.
    /// </summary>
    [PublicAPI]
    public static class LeftOuterJoinExtensions
    {
        /// <summary>
        /// Performs a left outer join.
        /// </summary>
        /// <typeparam name="T">The type of the source <see cref="IQueryable"/>.</typeparam>
        /// <param name="queryable">The source <see cref="IQueryable"/>.</param>
        /// <param name="predicate">The predicate joining the source <see cref="IQueryable"/> with another entity.</param>
        /// <returns>The result of the left outer join.</returns>
        [EntityFrameworkExtension]
        public static IQueryable<T> LeftOuterJoin<T>(this IQueryable<T> queryable, Expression<Func<T, bool>> predicate) where T : new()
        {
            return !queryable.Provider
                             .ToString()
                             .Equals("System.Data.Entity.Internal.Linq.DbQueryProvider", StringComparison.OrdinalIgnoreCase)
                   ?
                   queryable.Where(predicate)
                            .DefaultIfEmpty(new T()) 
                   :
                   queryable.Where(predicate)
                            .DefaultIfEmpty();
        }
    }
}
