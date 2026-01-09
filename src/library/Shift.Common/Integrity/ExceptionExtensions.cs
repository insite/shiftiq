using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shift.Common
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Traverses a hierarchy of objects using a selector function until a condition is met.
        /// </summary>
        public static IEnumerable<TSource> FromHierarchy<TSource>(
            this TSource source,
            Func<TSource, TSource> nextItem,
            Func<TSource, bool> canContinue)
        {
            for (var current = source; canContinue(current); current = nextItem(current))
            {
                yield return current;
            }
        }

        /// <summary>
        /// Traverses a hierarchy of reference types until null is encountered.
        /// </summary>
        public static IEnumerable<TSource> FromHierarchy<TSource>(
            this TSource source,
            Func<TSource, TSource> nextItem)
            where TSource : class
        {
            return FromHierarchy(source, nextItem, s => s != null);
        }

        /// <summary>
        /// Gets all exception messages concatenated with newlines.
        /// </summary>
        public static string GetAllMessages(this Exception exception)
        {
            if (exception == null) return string.Empty;

            var messages = exception
                .GetAllExceptions()
                .Select(ex => ex.Message);

            return string.Join(Environment.NewLine, messages);
        }

        /// <summary>
        /// Gets all exception messages formatted as Markdown with hierarchical bullet points.
        /// </summary>
        public static string GetFormattedMessages(this Exception exception)
        {
            if (exception == null) return string.Empty;
            var result = new StringBuilder();
            var exceptions = exception.GetAllExceptions().ToList();
            for (int i = 0; i < exceptions.Count; i++)
            {
                if (i > 0) result.AppendLine();
                // Use markdown bullet points with proper indentation
                result.Append(new string(' ', i * 2));
                result.Append("- ");
                if (i > 0)
                    result.Append($"**Level {i}**: ");

                // Add method name if available
                var methodName = GetMethodName(exceptions[i]);
                if (!string.IsNullOrEmpty(methodName))
                    result.Append($"`{methodName}`: ");

                result.Append(exceptions[i].Message);
            }
            return result.ToString();
        }

        private static string GetMethodName(Exception exception)
        {
            if (exception.TargetSite == null) return null;

            var method = exception.TargetSite;
            var declaringType = method.DeclaringType?.FullName ?? method.DeclaringType?.Name;

            if (string.IsNullOrEmpty(declaringType))
                return method.Name;

            return $"{declaringType}.{method.Name}";
        }

        /// <summary>
        /// Gets all exceptions in the hierarchy, including AggregateException inner exceptions.
        /// </summary>
        public static IEnumerable<Exception> GetAllExceptions(this Exception exception)
        {
            if (exception == null) yield break;

            yield return exception;

            // Handle AggregateException specially
            if (exception is AggregateException aggregateEx && aggregateEx.InnerExceptions.Any())
            {
                foreach (var innerEx in aggregateEx.InnerExceptions)
                {
                    foreach (var nestedEx in innerEx.GetAllExceptions())
                    {
                        yield return nestedEx;
                    }
                }
            }
            else if (exception.InnerException != null)
            {
                foreach (var innerEx in exception.InnerException.GetAllExceptions())
                {
                    yield return innerEx;
                }
            }
        }

        /// <summary>
        /// Gets all exception messages as an enumerable sequence.
        /// </summary>
        public static IEnumerable<string> GetMessages(this Exception exception)
        {
            return exception?.GetAllExceptions().Select(ex => ex.Message) ?? Enumerable.Empty<string>();
        }
    }
}
