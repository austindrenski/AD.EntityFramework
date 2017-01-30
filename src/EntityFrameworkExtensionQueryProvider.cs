using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace AD.EntityFramework
{
    [UsedImplicitly]
    internal class EntityFrameworkExtensionQueryProvider : IQueryProvider
    {
        private static readonly Type GenericType = typeof(EntityFrameworkExtensionQueryable<>);

        private readonly IQueryProvider _innerQueryProvider;

        internal EntityFrameworkExtensionQueryProvider(IQueryProvider innerQueryProvider)
        {
            _innerQueryProvider = innerQueryProvider;
        }

        IQueryable<T> IQueryProvider.CreateQuery<T>(Expression expression)
        {
            return new EntityFrameworkExtensionQueryable<T>(this, expression);
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            Type elementType = expression.Type.GetElementType();
            Type genericType = GenericType.MakeGenericType(elementType);
            object[] args = new object[] { this, expression };
            IQueryable queryable = (IQueryable)Activator.CreateInstance(genericType, args);
            return queryable;
        }

        T IQueryProvider.Execute<T>(Expression expression)
        {
            Expression visitedExpression = Visit(expression);
            return _innerQueryProvider.Execute<T>(visitedExpression);
        }

        object IQueryProvider.Execute(Expression expression)
        {
            Expression visitedExpression = Visit(expression);
            return _innerQueryProvider.Execute(visitedExpression);
        }

        internal IEnumerable<T> ExecuteQuery<T>(Expression expression)
        {
            Expression visitedExpression = Visit(expression);
            return _innerQueryProvider.CreateQuery<T>(visitedExpression).AsEnumerable();
        }

        private Expression Visit(Expression expression)
        {
            EntityFrameworkExtensionExpressionVisitor visitor = new EntityFrameworkExtensionExpressionVisitor(_innerQueryProvider);
            Expression visitedExpression = visitor.Visit(expression);
            return visitedExpression;
        }
    }
}
