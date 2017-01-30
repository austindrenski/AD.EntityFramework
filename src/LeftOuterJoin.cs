using System;
using System.Linq;
using System.Linq.Expressions;

namespace AD.EntityFramework
{
    public static class LeftOuterJoinExtensions
    {
        [EntityFrameworkExtension]
        public static IQueryable<T> LeftOuterJoin<T>(this IQueryable<T> queryable, Expression<Func<T, bool>> predicate) where T : new()
        {
            return !queryable.Provider
                             .ToString()
                             .Equals("System.Data.Entity.Internal.Linq.DbQueryProvider")
                ? queryable.Where(predicate)
                           .DefaultIfEmpty(new T()) 
                : queryable.Where(predicate)
                           .DefaultIfEmpty();
        }
    }
}
