using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Shift.Common
{
    public static class StringExtensions
    {
        private static readonly char[] _trimChars;
        private static readonly Dictionary<char, string> _replacememntChars = new Dictionary<char, string>
        {
            {'\xa0', " "},
            {'\xa9', "(c)"},
            {'\xad', "-"},
            {'\xae', "(r)"},
            {'\xb7', "*"},
            {'\u2018', "'"},
            {'\u2019', "'"},
            {'\u201c', "\""},
            {'\u201d', "\""},
            {'\u2026', "..."},
            {'\u2002', " "},
            {'\u2003', " "},
            {'\u2009', " "},
            {'\u2013', "-"},
            {'\u2014', "--"},
            {'\u2122', "(tm)"}
        };
        private static readonly char[] _splitDelimiters = new[] { ',', ';', '|', '\r', '\n' };

        static StringExtensions()
        {
            var tch = new List<char> { ' ', '\n', '\r', '\t' };

            foreach (var kv in _replacememntChars)
            {
                if (kv.Value.Length > 1)
                    continue;

                var ch = kv.Value[0];
                if (tch.Contains(ch))
                    tch.Add(kv.Key);
            }

            _trimChars = tch.ToArray();
        }

        public static string[] CleanSplit(this string text)
        {
            return CleanSplit(text, _splitDelimiters);
        }

        public static string[] CleanSplit(this string text, char[] delimiters)
        {
            var items = text?.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            return items?.Select(CleanTrim).Where(x => !string.IsNullOrEmpty(x)).ToArray();
        }

        public static string CleanTrim(this string text)
        {
            if (text == null)
                return null;

            text = text.Trim(_trimChars);

            var result = new StringBuilder(text.Length);

            for (var i = 0; i < text.Length; i++)
            {
                var ch = text[i];
                if (_replacememntChars.ContainsKey(ch))
                    result.Append(_replacememntChars[ch]);
                else
                    result.Append(ch);
            }

            return result.ToString();
        }

        public static bool Contains(this string str, string substr, StringComparison comparisonType)
        {
            if (substr == null)
                throw new ArgumentNullException(nameof(substr));

            return str.IndexOf(substr, comparisonType) >= 0;
        }

        public static bool HasValue(this string s) =>
            s.IsNotEmpty() && s.Trim(_trimChars).Length > 0;

        public static bool HasNoValue(this string s) =>
            s.IsEmpty() || s.Trim(_trimChars).Length == 0;

        public static bool IsEmpty(this string s) =>
            s == null || s.Length == 0;

        public static bool IsNotEmpty(this string s) =>
            s != null && s.Length > 0;

        public static string IfNullOrEmpty(this string value, string nullValue) =>
            string.IsNullOrEmpty(value) ? nullValue : value;

        public static string IfNullOrEmpty(this string value, Func<string> nullValueFactory) =>
            string.IsNullOrEmpty(value) ? nullValueFactory.Invoke() : value;

        public static string NullIfEmpty(this string value) =>
            string.IsNullOrEmpty(value) ? null : value;

        public static string EmptyIfNull(this string value) =>
            value == null ? string.Empty : value;

        public static string NullIf(this string value, string nullValue, bool ignoreCase = false) =>
            string.IsNullOrEmpty(value) || string.Equals(value, nullValue, ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture) ? null : value;

        public static string NullIfWhiteSpace(this string value) =>
            string.IsNullOrWhiteSpace(value) ? null : value;

        public static string MaxLength(this string value, int maxLength, bool addEllipsis = false) =>
            value == null || value.Length <= maxLength
                ? value
                : addEllipsis && maxLength > 3
                    ? value.Substring(0, maxLength - 3) + "..."
                    : value.Substring(0, maxLength);

        public static T ToEnum<T>(this string value, bool ignoreCase = true) where T : struct, Enum =>
            ToEnum(value, ignoreCase, out T result)
                ? result
                : throw ApplicationError.Create($"Unable to convert a string '{value}' into enum of type {typeof(T).FullName}");

        public static T ToEnum<T>(this string value, T defaultValue, bool ignoreCase = true) where T : struct, Enum =>
            ToEnum(value, ignoreCase, out T result) ? result : defaultValue;

        public static T ToEnum<T>(this string value, Func<string, T> defaultValueFactory, bool ignoreCase = true) where T : struct, Enum =>
            ToEnum(value, ignoreCase, out T result) ? result : defaultValueFactory(value);

        public static T? ToEnumNullable<T>(this string value, bool ignoreCase = true) where T : struct, Enum =>
            ToEnum(value, ignoreCase, out T result) ? result : (T?)null;

        private static bool ToEnum<T>(this string value, bool ignoreCase, out T result) where T : struct, Enum =>
            Enum.TryParse(value, ignoreCase, out result) && Enum.IsDefined(typeof(T), result);

        public static string Format(this string format, object arg0) =>
            string.Format(format, arg0);

        public static string Format(this string format, object arg0, object arg1) =>
            string.Format(format, arg0, arg1);

        public static string Format(this string format, object arg0, object arg1, object arg2) =>
            string.Format(format, arg0, arg1, arg2);

        public static string Format(this string format, params object[] args) =>
            string.Format(format, args);

        /// <summary>
        /// Returns true if a string matches another string.
        /// </summary>
        /// <remarks>
        /// This is not case-sensitive, two null values match, and any two whitespace values match.
        /// </remarks>
        public static bool Matches(this string value, string other)
        {
            if (value == null && other == null)
                return true;

            if ((value == null && other != null) || (value != null && other == null))
                return false;

            return string.Compare(other, value, StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Returns true if a string has one or more matches in an array of other strings.
        /// </summary>
        public static bool MatchesAny(this string value, IEnumerable<string> others)
        {
            if (value.IsEmpty())
                return false;

            foreach (var other in others)
                if (Matches(value, other))
                    return true;

            return false;
        }

        /// <summary>
        /// Returns true if a string has zero matches in an array of other strings.
        /// </summary>
        public static bool MatchesNone(this string value, IEnumerable<string> others)
        {
            return !MatchesAny(value, others);
        }

        /// <summary>
        /// Converts a string value into a non-null collection.
        /// </summary>
        /// <remarks>
        /// Commas and newlines are assumed to be the delimiters. Empty items are removed from the
        /// collection. Leading and trailing whitespace characters are removed from each item in the
        /// collection.
        /// </remarks>
        public static IEnumerable<string> Parse(this string csv)
        {
            var list = new List<string>();

            if (string.IsNullOrWhiteSpace(csv))
                return list;

            list = csv
                .Split(new char[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            return list;
        }

        /// <summary>
        /// Returns true if a string starts with any item in an array of strings.
        /// </summary>
        public static bool StartsWithAny(this string value, IEnumerable<string> others)
        {
            if (value.IsEmpty())
                return false;

            foreach (var other in others)
                if (!other.IsEmpty() && value.StartsWith(other, StringComparison.OrdinalIgnoreCase))
                    return true;

            return false;
        }

        public static Guid ToGuid(this string value) =>
            Guid.Parse(value);

        public static Guid ToGuid(this string value, Guid @default) =>
            Guid.TryParse(value, out var result) ? result : @default;

        public static Guid? ToGuidNullable(this string value) =>
            Guid.TryParse(value, out var result) ? result : (Guid?)null;

        /// <summary>
        /// Converts a string value from PascalCase to kebab-case.
        /// </summary>
        /// <example>
        /// ToKebabCase("ThisIsAnExample") => "this-is-an-example"
        /// </example>
        public static string ToKebabCase(this string input, bool allowForwardSlash = false)
        {
            string output = null;

            if (input.IsEmpty())
                return output;

            if (!allowForwardSlash)
            {
                // Find the boundaries between uppercase letters and other letters.
                output = Regex.Replace(input, "([a-z])([A-Z])", "$1-$2");
            }
            else
            {
                var segments = input.Split(new char[] { '/' });

                foreach (var segment in segments)
                {
                    // Find the boundaries between uppercase letters and other letters.
                    output += Regex.Replace(segment, "([a-z])([A-Z])", "$1-$2");

                    if (segment != segments.Last())
                        output += "/";
                }
            }

            // Convert the entire string to lowercase.
            return output.ToLower();
        }

        /// <summary>
        /// Converts a string value from PascalCase to Title Case.
        /// </summary>
        /// <example>
        /// ToTitleCase("ThisIsAnExample") => "This Is An Example"
        /// </example>
        public static string ToTitleCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var result = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                char currentChar = input[i];

                // Add space before uppercase letters (except for the first character)

                if (i > 0 && char.IsUpper(currentChar))
                    result.Append(' ');

                result.Append(currentChar);
            }

            return result.ToString();
        }
    }
}
