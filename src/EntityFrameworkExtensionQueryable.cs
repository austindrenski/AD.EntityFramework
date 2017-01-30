using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace AD.EntityFramework
{
    [UsedImplicitly]
    internal class EntityFrameworkExtensionQueryable<T> : IOrderedQueryable<T>
    {
        private readonly EntityFrameworkExtensionQueryProvider _entityFrameworkExtensionQueryProvider;

        private readonly Expression _expression;

        private readonly Type _elementType;

        Type IQueryable.ElementType
        {
            get
            {
                return _elementType;
            }
        } 

        Expression IQueryable.Expression
        {
            get
            {
                return _expression;
            }
        }

        IQueryProvider IQueryable.Provider
        {
            get
            {
                return _entityFrameworkExtensionQueryProvider;
            }
        }

        internal EntityFrameworkExtensionQueryable(EntityFrameworkExtensionQueryProvider linqEntityFrameworkExtensionProvider, Expression expression)
        {
            _entityFrameworkExtensionQueryProvider = linqEntityFrameworkExtensionProvider;
            _expression = expression;
            _elementType = typeof(T);
        }

        internal EntityFrameworkExtensionQueryable(IQueryable queryable)
        {
            _entityFrameworkExtensionQueryProvider = new EntityFrameworkExtensionQueryProvider(queryable.Provider);
            _expression = queryable.Expression;
            _elementType = typeof(T);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _entityFrameworkExtensionQueryProvider.ExecuteQuery<T>(_expression)
                                                         .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _entityFrameworkExtensionQueryProvider.ExecuteQuery<T>(_expression)
                                                         .GetEnumerator();
        }
    }
}
