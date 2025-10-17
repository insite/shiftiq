using System;
using System.Linq;
using System.Linq.Expressions;

using Shift.Constant;

namespace Shift.Common.Linq
{
    public static class LinqExtensions3
    {
        private class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression Visit(Expression node) => node == _oldValue ? _newValue : base.Visit(node);
        }

        public static Expression<Func<T, bool>> CustomAnd<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<T, bool>>(
                Expression.And(left, right), parameter);
        }

        public static Expression<Func<T1, TResult>> MergeMemberInit<T1, TResult>(this Expression<Func<T1, TResult>> expr1, Expression<Func<T1, TResult>> expr2)
        {
            if (expr1.Body.NodeType != ExpressionType.MemberInit)
                throw new ApplicationError($"Unexpected expression type (expr1): {expr1.Body.NodeType.GetName()}");

            if (expr2.Body.NodeType != ExpressionType.MemberInit)
                throw new ApplicationError($"Unexpected expression type (expr2): {expr2.Body.NodeType.GetName()}");

            var visitor = new ReplaceExpressionVisitor(expr2.Parameters[0], expr1.Parameters[0]);
            var body2 = (MemberInitExpression)visitor.Visit(expr2.Body);

            var body1 = (MemberInitExpression)expr1.Body;
            var body = body1.Update(body1.NewExpression, body1.Bindings.Union(body2.Bindings));

            return expr1.Update(body, expr1.Parameters);
        }
    }
}
