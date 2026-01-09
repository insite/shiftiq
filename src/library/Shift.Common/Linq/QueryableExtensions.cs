using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Shift.Common.Linq
{
    public static class QueryableExtensions
    {
        #region Construction

        static QueryableExtensions()
        {
            var queryableType = typeof(Queryable);
            var queryableMethods = queryableType.GetMethods();

            _queryableOrderByMethod = GetQueyrableMethod("OrderBy");
            _queryableOrderByDescendingMethod = GetQueyrableMethod("OrderByDescending");
            _queryableThenByMethod = GetQueyrableMethod("ThenBy");
            _queryableThenByDescendingMethod = GetQueyrableMethod("ThenByDescending");

            MethodInfo GetQueyrableMethod(string methodName)
            {
                return queryableMethods
                    .Where(m => m.Name == methodName && m.IsGenericMethodDefinition)
                    .Where(m =>
                    {
                        var parameters = m.GetParameters().ToList();
                        // Put more restriction here to ensure selecting the right overload                
                        return parameters.Count == 2; //overload that has 2 parameters
                    }).Single();
            }
        }

        #endregion

        #region OrderBy

        private static readonly MethodInfo _queryableOrderByMethod;
        private static readonly MethodInfo _queryableOrderByDescendingMethod;
        private static readonly MethodInfo _queryableThenByMethod;
        private static readonly MethodInfo _queryableThenByDescendingMethod;

        private static readonly ConcurrentDictionary<(Type EntityType, string PropertyName), (Type Type, LambdaExpression Body)> _orderBySelectorCache =
            new ConcurrentDictionary<(Type, string), (Type Type, LambdaExpression Body)>();

        public static IQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> query, string sortExpression)
        {
            if (sortExpression.IsEmpty())
                return query;

            var entityType = typeof(TSource);
            IOrderedQueryable<TSource> orderedQuery = null;

            var sortExpressions = sortExpression.Split(new[] { ',' }, StringSplitOptions.None);
            foreach (var expr in sortExpressions)
            {
                var exprInfo = GetOrderByExpressionInfo(expr);

                // Get x => x.PropName

                var selector = _orderBySelectorCache.GetOrAdd((entityType, exprInfo.PropertyName), x => BuildOrderBySelectorBody(x.EntityType, x.PropertyName));

                // Get System.Linq.Queryable.OrderBy() method

                var method = orderedQuery == null
                    ? (exprInfo.IsDescending ? _queryableOrderByDescendingMethod : _queryableOrderByMethod)
                    : (exprInfo.IsDescending ? _queryableThenByDescendingMethod : _queryableThenByMethod);

                // The LINQ's OrderBy<TSource, TKey> has two generic types.

                var genericMethod = method.MakeGenericMethod(entityType, selector.Type);

                // Call query.OrderBy(selector) with query and selector. Pass the selector to the method as Expression
                // and don't compile it. This way EF can extract "order by" columns and generate its own SQL.

                orderedQuery = (IOrderedQueryable<TSource>)genericMethod.Invoke(genericMethod, new object[] { orderedQuery ?? query, selector.Body });
            }

            return orderedQuery;
        }

        private static (string PropertyName, bool IsDescending) GetOrderByExpressionInfo(string value)
        {
            value = value.Trim();

            if (value.EndsWith(" desc", StringComparison.OrdinalIgnoreCase))
                return (value.Substring(0, value.Length - 5).Trim(), true);
            else if (value.EndsWith(" asc", StringComparison.OrdinalIgnoreCase))
                return (value.Substring(0, value.Length - 4).Trim(), false);
            else
                return (value, false);
        }

        private static (Type Type, LambdaExpression Body) BuildOrderBySelectorBody(Type entityType, string propertyName)
        {
            var parameterExpr = Expression.Parameter(entityType, "x");

            Type selectorType = null;
            Expression bodyExpr = null;

            var coalesceParts = propertyName.Split(new[] { "??" }, StringSplitOptions.None);
            if (coalesceParts.Length > 1)
            {
                // Create x => x.PropName1 ?? x.PropName2

                var propertyExpr = BuildPropertyExpression(entityType, parameterExpr, coalesceParts[0].Trim());
                bodyExpr = propertyExpr;
                selectorType = propertyExpr.Type;

                for (var i = 1; i < coalesceParts.Length; i++)
                {
                    propertyExpr = BuildPropertyExpression(entityType, parameterExpr, coalesceParts[i].Trim());
                    bodyExpr = Expression.Coalesce(bodyExpr, propertyExpr);
                    selectorType = propertyExpr.Type;
                }
            }
            else
            {
                // Create x => x.PropName

                var propertyExpr = BuildPropertyExpression(entityType, parameterExpr, propertyName);
                selectorType = propertyExpr.Type;
                bodyExpr = propertyExpr;
            }

            return (selectorType, Expression.Lambda(bodyExpr, parameterExpr));
        }

        private static Expression BuildPropertyExpression(Type entityType, ParameterExpression parameterExpr, string propertyName)
        {
            MemberExpression result = null;

            var parts = propertyName.Split('.');
            for (var i = 0; i < parts.Length; i++)
                result = Expression.PropertyOrField((Expression)result ?? parameterExpr, parts[i]);

            return result;
        }

        public static bool IsOrdered(this IQueryable queryable)
        {
            var expression = queryable.Expression;

            return HasOrderByInExpression(expression);
        }

        private static bool HasOrderByInExpression(Expression expression)
        {
            if (expression is MethodCallExpression methodCall)
            {
                var methodName = methodCall.Method.Name;

                if (methodName == "OrderBy" || methodName == "OrderByDescending" ||
                    methodName == "ThenBy" || methodName == "ThenByDescending")
                {
                    return true;
                }

                return methodCall.Arguments.Any(HasOrderByInExpression);
            }

            switch (expression)
            {
                case LambdaExpression lambda:
                    return HasOrderByInExpression(lambda.Body);

                case UnaryExpression unary:
                    return HasOrderByInExpression(unary.Operand);

                case BinaryExpression binary:
                    return HasOrderByInExpression(binary.Left) || HasOrderByInExpression(binary.Right);

                default:
                    return false;
            }
        }

        #endregion

        #region Paging

        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, Filter filter)
            => SkipTake(query, filter?.Paging);

        public static IEnumerable<T> ApplyPaging<T>(this IEnumerable<T> query, Filter filter)
            => SkipTake(query, filter?.Paging);

        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, Paging paging)
            => SkipTake(query, paging);

        public static IEnumerable<T> ApplyPaging<T>(this IEnumerable<T> query, Paging paging)
            => SkipTake(query, paging);

        private static IQueryable<T> SkipTake<T>(IQueryable<T> query, Paging paging)
        {
            var skip = paging?.Skip;
            var take = paging?.Take;

            if (skip.HasValue)
            {
                if (skip == int.MaxValue)
                    return query.Take(0);
                else if (skip > 0)
                    query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                if (take == 0)
                    return query.Take(0);
                else if (take >= 1 && take < int.MaxValue)
                    query = query.Take(take.Value);
            }

            return query;
        }

        private static IEnumerable<T> SkipTake<T>(IEnumerable<T> query, Paging paging)
        {
            var skip = paging?.Skip;
            var take = paging?.Take;

            if (skip.HasValue)
            {
                if (skip == int.MaxValue)
                    return query.Take(0);
                else if (skip > 0)
                    query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                if (take == 0)
                    return query.Take(0);
                else if (take >= 1 && take < int.MaxValue)
                    query = query.Take(take.Value);
            }

            return query;
        }

        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> queryable, QueryFilter filter)
        {
            var isOrdered = queryable is IOrderedQueryable || queryable.IsOrdered();

            if (!isOrdered)
            {
                var detail = "This queryable object is not ordered. Sorting is required before you apply paging. "
                    + "No default order is specified for this query, ";

                detail += filter.Sort.IsEmpty()
                    ? "and no sort expression is defined in the filter."
                    : $"but a sort expression is defined in the filter ({filter.Sort}).";

                throw new ArgumentException(detail);
            }

            var page = filter.Page;

            var take = filter.PageSize;

            var skip = (page - 1) * take;

            if (page > 0 && take > 0)
                queryable = queryable.Skip(skip).Take(take);

            return queryable;
        }

        #endregion

        #region To{Something}

        public static SearchResultList ToSearchResult<T>(this IList<T> list)
        {
            return new SearchResultList((IList)list);
        }

        public static SearchResultList ToSearchResult<T>(this IQueryable<T> query)
        {
            return query.ToList().ToSearchResult();
        }

        public static DataTable ToDataTable<T>(this IQueryable<T> query)
        {
            var table = new DataTable();
            var props = typeof(T).GetProperties();

            foreach (var prop in props)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

            foreach (var item in query.AsEnumerable())
            {
                var row = table.NewRow();

                foreach (var prop in props)
                    row[prop.Name] = prop.GetValue(item, null) ?? DBNull.Value;

                table.Rows.Add(row);
            }

            return table;
        }

        #endregion
    }
}