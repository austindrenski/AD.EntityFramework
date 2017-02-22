using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace AD.EntityFramework
{
    [UsedImplicitly]
    internal class EntityFrameworkExtensionExpressionVisitor : ExpressionVisitor
    {
        private readonly IQueryProvider _queryProvider;

        internal EntityFrameworkExtensionExpressionVisitor(IQueryProvider queryProvider)
        {
            _queryProvider = queryProvider;
        }

        protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
        {
            if (!methodCallExpression.Method.IsStatic)
            {
                return base.VisitMethodCall(methodCallExpression);
            }
            if (!methodCallExpression.Method.GetCustomAttributes(typeof(EntityFrameworkExtensionAttribute), false).Any())
            {
                return base.VisitMethodCall(methodCallExpression);
            }
            object[] parameters = ProcessArguments(methodCallExpression.Arguments);
            return ExecuteMethod(methodCallExpression.Method, parameters).Expression;
        }

        private static IQueryable ExecuteMethod(MethodInfo methodInfo, object[] parameters)
        {
            return (IQueryable)methodInfo.Invoke(null, parameters);
        }

        private object[] ProcessArguments(IReadOnlyList<Expression> arguments)
        {
            object[] parameters = new object[arguments.Count];
            parameters[0] = _queryProvider.CreateQuery(arguments[0]);
            for (int i = 1; i < arguments.Count; i++)
            {
                parameters[i] = ProcessArgument(arguments[i]);
            }
            return parameters;
        }

        private static object ProcessArgument(Expression argument)
        {
            return argument.NodeType == ExpressionType.Constant ? ((ConstantExpression)argument).Value :
                   argument.NodeType == ExpressionType.Quote ? ((UnaryExpression)argument).Operand : argument;
        }
    }
}
