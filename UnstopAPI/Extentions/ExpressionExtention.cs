using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace UnstopAPI.Extentions
{
    public static class ExpressionExtention
    {
        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var visitor = new ReplaceParameterVisitor();
            visitor.Add(expr1.Parameters[0], parameter);
            visitor.Add(expr2.Parameters[0], parameter);

            var combined = Expression.AndAlso(
                visitor.Visit(expr1.Body),
                visitor.Visit(expr2.Body)
            );

            return Expression.Lambda<Func<T, bool>>(combined, parameter);
        }

        private class ReplaceParameterVisitor : ExpressionVisitor
        {
            private readonly Dictionary<ParameterExpression, ParameterExpression> _parameterMap = new();

            public void Add(ParameterExpression from, ParameterExpression to)
            {
                _parameterMap[from] = to;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (_parameterMap.TryGetValue(node, out var replacement))
                {
                    node = replacement;
                }
                return base.VisitParameter(node);
            }
        }
    }
}
