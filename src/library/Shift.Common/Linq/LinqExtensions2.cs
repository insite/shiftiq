using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Shift.Common.Linq
{
    /// <summary>Refer to http://www.albahari.com/nutshell/linqkit.html and
    /// http://tomasp.net/blog/linq-expand.aspx for more information.</summary>
    public static class LinqExtensions2
    {
        public static IQueryable<T> AsExpandable<T>(this IQueryable<T> query)
        {
            if (query is ExpandableQuery<T>) return (ExpandableQuery<T>)query;
            return new ExpandableQuery<T>(query);
        }

        public static Expression<TDelegate> Expand<TDelegate>(this Expression<TDelegate> expr)
        {
            return (Expression<TDelegate>)new ExpressionExpander().Visit(expr);
        }

        public static Expression Expand(this Expression expr)
        {
            return new ExpressionExpander().Visit(expr);
        }

        public static TResult Invoke<TResult>(this Expression<Func<TResult>> expr)
        {
            return expr.Compile().Invoke();
        }

        public static TResult Invoke<T1, TResult>(this Expression<Func<T1, TResult>> expr, T1 arg1)
        {
            return expr.Compile().Invoke(arg1);
        }

        public static TResult Invoke<T1, T2, TResult>(this Expression<Func<T1, T2, TResult>> expr, T1 arg1, T2 arg2)
        {
            return expr.Compile().Invoke(arg1, arg2);
        }

        public static TResult Invoke<T1, T2, T3, TResult>(
            this Expression<Func<T1, T2, T3, TResult>> expr, T1 arg1, T2 arg2, T3 arg3)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3);
        }

        public static TResult Invoke<T1, T2, T3, T4, TResult>(
            this Expression<Func<T1, T2, T3, T4, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4);
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T element in source)
            {
                action(element);

                yield return element;
            }
        }

        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> getChildren, bool childrenFirst = false)
        {
            var stack = new Stack<IEnumerator<T>>();
            var enumerator = source.GetEnumerator();

            stack.Push(enumerator);

            try
            {
                while (true)
                {
                    if (!enumerator.MoveNext())
                    {
                        stack.Pop().Dispose();

                        if (stack.Count == 0)
                            break;

                        enumerator = stack.Peek();

                        if (childrenFirst)
                            yield return enumerator.Current;
                    }
                    else
                    {
                        var current = enumerator.Current;
                        var children = getChildren(current);

                        if (!childrenFirst)
                            yield return current;

                        if (children != null)
                        {
                            enumerator = children.GetEnumerator();
                            stack.Push(enumerator);
                        }
                    }
                }
            }
            finally
            {
                while (stack.Count > 0)
                    stack.Pop().Dispose();
            }
        }

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> value) => value == null ? Enumerable.Empty<T>() : value;
    }
}
