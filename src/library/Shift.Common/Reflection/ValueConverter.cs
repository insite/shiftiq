using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace Shift.Common
{
    public static class ValueConverter
    {
        public static readonly CultureInfo DefaultCulture = new CultureInfo("en-US");

        public const int NullInt32 = int.MinValue;
        public const int NavInt32 = int.MinValue + 1;
        public const int NapInt32 = int.MinValue + 2;

        public const decimal DecimalNotKnown = decimal.MinValue;
        public const double DoubleNotKnown = double.MinValue;
        public const int Int32NotKnown = int.MinValue;
        public const long Int64NotKnown = long.MinValue;

        public const decimal DecimalNotAvailable = decimal.MinValue + 1;
        public const double DoubleNotAvailable = double.MinValue + 1;
        public const int Int32NotAvailable = int.MinValue + 1;
        public const long Int64NotAvailable = long.MinValue + 1;

        public const decimal DecimalNotApplicable = decimal.MinValue + 2;
        public const double DoubleNotApplicable = double.MinValue + 2;
        public const int Int32NotApplicable = int.MinValue + 2;
        public const long Int64NotApplicable = long.MinValue + 2;

        #region Interrogation (null)

        public static DateTime NullDate => DateTime.MinValue;

        public static bool IsNull(object value)
        {
            if (value == null)
                return true;

            if (value == DBNull.Value)
                return true;

            var typeName = value.GetType().Name;

            if (typeName == typeof(int).Name)
                return (int)value == Int32NotKnown;

            if (typeName == typeof(long).Name)
                return (long)value == Int64NotKnown;

            if (typeName == typeof(decimal).Name)
                return (decimal)value == DecimalNotKnown;

            if (typeName == typeof(double).Name)
                return Math.Abs((double)value - DoubleNotKnown) < double.Epsilon;

            if (typeName == typeof(string).Name)
                return string.IsNullOrEmpty((string)value);

            if (typeName == typeof(Guid).Name)
                return (Guid)value == Guid.Empty;

            return false;
        }

        public static bool IsNotNull(object value)
        {
            return !IsNull(value);
        }

        public static bool IsNull(string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsNotNull(string value)
        {
            return !IsNull(value);
        }

        public static bool IsNull(Guid value)
        {
            return value == Guid.Empty;
        }

        public static bool IsNull(int value)
        {
            return value == Int32NotKnown;
        }

        public static bool IsNotNull(int value)
        {
            return !IsNull(value);
        }

        public static bool IsNull(long value)
        {
            return value == Int64NotKnown;
        }

        public static bool IsNotNull(long value)
        {
            return !IsNull(value);
        }

        public static bool IsNull(decimal value)
        {
            return value == DecimalNotKnown;
        }

        public static bool IsNotNull(decimal value)
        {
            return !IsNull(value);
        }
        
        public static bool IsNotNull(float value)
        {
            return !IsNull(value);
        }

        public static bool IsNull(double value)
        {
            return Math.Abs(value - DoubleNotKnown) < double.Epsilon;
        }

        public static bool IsNotNull(double value)
        {
            return !IsNull(value);
        }

        public static bool IsNull(DateTime value)
        {
            return value == NullDate;
        }

        public static bool IsNotNull(DateTime value)
        {
            return !IsNull(value);
        }

        #endregion

        #region Interrogation (number)

        public static bool IsNotApplicable(object value)
        {
            if (value == null)
                return false;

            var t = value.GetType();

            if (t == typeof(decimal))
                return (decimal)value == DecimalNotApplicable;

            if (t == typeof(double))
                return Math.Abs((double)value - DoubleNotApplicable) < double.Epsilon;

            if (t == typeof(int))
                return (int)value == Int32NotApplicable;

            if (t == typeof(long))
                return (long)value == Int64NotApplicable;
            
            return false;
        }

        public static bool IsNotAvailable(object value)
        {
            if (value == null)
                return false;

            var t = value.GetType();

            if (t == typeof(decimal))
                return (decimal)value == DecimalNotAvailable;

            if (t == typeof(double))
                return Math.Abs((double)value - DoubleNotAvailable) < double.Epsilon;
            
            if (t == typeof(int))
                return (int)value == Int32NotAvailable;

            if (t == typeof(long))
                return (long)value == Int64NotAvailable;
            
            return false;
        }

        public static bool IsZero(object value)
        {
            if (value == null)
                return false;

            var t = value.GetType();

            if (t == typeof(decimal))
                return (decimal)value == 0;

            if (t == typeof(double))
                return Math.Abs((double)value - 0) < double.Epsilon;

            if (t == typeof(short))
                return (short)value == 0;

            if (t == typeof(int))
                return (int)value == 0;

            if (t == typeof(long))
                return (long)value == 0;

            if (t == typeof(float))
                return Math.Abs((float)value - 0) < float.Epsilon;

            return false;
        }

        public static bool IsOne(object value)
        {
            if (value == null)
                return false;

            var t = value.GetType();

            if (t == typeof(decimal))
                return (decimal)value == 1;

            if (t == typeof(double))
                return Math.Abs((double)value - 1) < double.Epsilon;

            if (t == typeof(short))
                return (short)value == 1;

            if (t == typeof(int))
                return (int)value == 1;

            if (t == typeof(long))
                return (long)value == 1;

            if (t == typeof(float))
                return Math.Abs((float)value - 1) < float.Epsilon;

            return false;
        }

        #endregion

        #region Interrogation (string)

        /// <summary>
        ///     Returns true if it is possible to convert the text into a Boolean
        ///     value (of type System.Boolean).
        /// </summary>
        public static bool IsBoolean(string text)
        {
            return EqualsAny(text, new[] { "true", "yes", "y", "1", "false", "no", "n", "0" });
        }
        
        /// <summary>
        ///     Returns true if it is possible to convert the text into a floating-
        ///     point numeric value (of type System.Decimal).
        /// </summary>
        public static bool IsDecimal(string text)
        {
            if (text == null)
                return false;

            return decimal.TryParse(text, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, DefaultCulture, out _);
        }

        /// <summary>
        ///     Returns true if it is possible to convert the text into a floating-
        ///     point numeric value (of type System.Double).
        /// </summary>
        public static bool IsDouble(string text)
        {
            if (text == null)
                return false;

            return double.TryParse(text, NumberStyles.Float | NumberStyles.AllowThousands | NumberStyles.AllowCurrencySymbol, DefaultCulture, out _);
        }

        /// <summary>
        ///     Returns true if it is possible to convert the text into a 32-bit
        ///     integer value (of type System.Int32).
        /// </summary>
        public static bool IsInt32(string text)
        {
            if (text == null)
                return false;

            return int.TryParse(text, NumberStyles.Integer | NumberStyles.AllowThousands | NumberStyles.AllowCurrencySymbol, DefaultCulture, out _);
        }

        #endregion

        #region Conversion (to string)

        #region Boolean

        public static string ToString(bool value)
        {
            return value ? bool.TrueString : bool.FalseString;
        }

        public static string ToString(bool? value)
        {
            string result = string.Empty;

            if (value.HasValue)
                result = ToString(value.Value);

            return result;
        }

        
        public static string StatusToString(bool? value)
        {
            string result = string.Empty;

            if (value.HasValue)
            {
                result = "Inactive";
                if (value.Value)
                    result = "Active";
            }

            return result;
        }

        #endregion

        #region DateTime

        public static string ToString(DateTime value)
        {
            return ToString(value, @"{0:yyyy\/MM\/dd hh:mm:ss tt}");
        }

        public static string ToString(DateTime? value)
        {
            string result = string.Empty;

            if (value.HasValue)
                result = ToString(value.Value);

            return result;
        }

        public static string ToString(DateTime value, string valueFormat)
        {
            string result = string.Empty;

            if (IsNotNull(value))
                result = string.Format(valueFormat, value);

            return result;
        }

        public static string ToString(DateTime? value, string valueFormat)
        {
            string result = string.Empty;

            if (value.HasValue)
                result = ToString(value.Value, valueFormat);

            return result;
        }

        #endregion

        #region Number

        private static string ToString<T>(T value, string valueFormat, string valueFormatNull, string valueFormatZero, string valueFormatOne)
        {
            var sb = new StringBuilder();

            if (IsNotNull(value))
            {
                if (IsNotApplicable(value))
                    sb.Append("N/AP");

                else if (IsNotAvailable(value))
                    sb.Append("N/AV");

                else if (IsZero(value) && !string.IsNullOrEmpty(valueFormatZero))
                    sb.AppendFormat(valueFormatZero, value);

                else if (IsOne(value) && !string.IsNullOrEmpty(valueFormatOne))
                    sb.AppendFormat(valueFormatOne, value);

                else if (!string.IsNullOrEmpty(valueFormat))
                    sb.AppendFormat(valueFormat, value);

                else
                    sb.AppendFormat("{0:n0}", value);
            }
            else
            {
                if (!string.IsNullOrEmpty(valueFormatNull))
                    sb.AppendFormat(valueFormatNull);
            }

            return sb.ToString();
        }

        #region Decimal

        public static string ToString(decimal value)
        {
            return ToString(value, null, null, null, null);
        }

        public static string ToString(decimal? value)
        {
            return ToString(value, null, null, null, null);
        }

        public static string ToString(decimal value, string valueFormat)
        {
            return ToString(value, valueFormat, null, null, null);
        }

        public static string ToString(decimal? value, string valueFormat)
        {
            return ToString(value, valueFormat, null, null, null);
        }

        public static string ToString(decimal value, string valueFormat, string valueFormatNull, string valueFormatZero, string valueFormatOne)
        {
            return ToString<decimal>(value, valueFormat, valueFormatNull, valueFormatZero, valueFormatOne);
        }

        public static string ToString(decimal? value, string valueFormat, string valueFormatNull, string valueFormatZero, string valueFormatOne)
        {
            return ToString<decimal>(value ?? DecimalNotKnown, valueFormat, valueFormatNull, valueFormatZero, valueFormatOne);
        }

        #endregion

        #region Double

        public static string ToString(double value)
        {
            return ToString(value, null, null, null, null);
        }

        public static string ToString(double? value)
        {
            return ToString(value, null, null, null, null);
        }

        public static string ToString(double value, string valueFormat)
        {
            return ToString(value, valueFormat, null, null, null);
        }

        public static string ToString(double? value, string valueFormat)
        {
            return ToString(value, valueFormat, null, null, null);
        }

        public static string ToString(double value, string valueFormat, string valueFormatNull, string valueFormatZero, string valueFormatOne)
        {
            return ToString<double>(value, valueFormat, valueFormatNull, valueFormatZero, valueFormatOne);
        }

        public static string ToString(double? value, string valueFormat, string valueFormatNull, string valueFormatZero, string valueFormatOne)
        {
            return ToString<double>(value ?? DoubleNotKnown, valueFormat, valueFormatNull, valueFormatZero, valueFormatOne);
        }

        #endregion

        #region Int32

        public static string ToString(int value)
        {
            return ToString(value, null, null, null, null);
        }

        public static string ToString(int? value)
        {
            return ToString(value, null, null, null, null);
        }

        public static string ToString(int value, string valueFormat)
        {
            return ToString(value, valueFormat, null, null, null);
        }

        public static string ToString(int? value, string valueFormat)
        {
            return ToString(value, valueFormat, null, null, null);
        }

        public static string ToString(int value, string valueFormat, string valueFormatNull, string valueFormatZero, string valueFormatOne)
        {
            return ToString<int>(value, valueFormat, valueFormatNull, valueFormatZero, valueFormatOne);
        }

        public static string ToString(int? value, string valueFormat, string valueFormatNull, string valueFormatZero, string valueFormatOne)
        {
            return ToString<int>(value ?? Int32NotKnown, valueFormat, valueFormatNull, valueFormatZero, valueFormatOne);
        }

        #endregion

        #region Int64

        public static string ToString(long value)
        {
            return ToString(value, null, null, null, null);
        }

        public static string ToString(long? value)
        {
            return ToString(value, null, null, null, null);
        }

        public static string ToString(long value, string valueFormat)
        {
            return ToString(value, valueFormat, null, null, null);
        }

        public static string ToString(long? value, string valueFormat)
        {
            return ToString(value, valueFormat, null, null, null);
        }

        public static string ToString(long value, string valueFormat, string valueFormatNull, string valueFormatZero, string valueFormatOne)
        {
            return ToString<long>(value, valueFormat, valueFormatNull, valueFormatZero, valueFormatOne);
        }

        public static string ToString(long? value, string valueFormat, string valueFormatNull, string valueFormatZero, string valueFormatOne)
        {
            return ToString<long>(value ?? Int64NotKnown, valueFormat, valueFormatNull, valueFormatZero, valueFormatOne);
        }

        #endregion

        #endregion

        #region String

        public static string ToString(string value, string valueFormat, string valueFormatNull)
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(value))
            {
                sb.AppendFormat(!string.IsNullOrEmpty(valueFormat) ? valueFormat : "{0}", value);
            }
            else
            {
                if (!string.IsNullOrEmpty(valueFormatNull))
                    sb.AppendFormat(valueFormatNull);
            }

            return sb.ToString();
        }

        #endregion

        #endregion

        #region Conversion (from string)

        #region Boolean

        public static bool ToBoolean(string value)
        {
            bool result = true;

            if (string.IsNullOrEmpty(value))
                result = false;

            else if (EqualsAny(value.ToLower(), new[] { "false", "no", "n", "0" }))
                result = false;

            return result;
        }

        public static bool? ToBooleanNullable(string value)
        {
            bool? result = null;

            if (string.IsNullOrEmpty(value))
                return null;

            if (value.Length > 0)
                result = ToBoolean(value);

            return result;
        }

        #endregion

        #region Char

        public static int ToInt32(char value)
        {
            if ('0' <= value && value <= '9')
                return (int)char.GetNumericValue(value);

            throw new ArgumentException("Unexpected character value: " + value);
        }

        public static char ToChar(string value)
        {
            char result = char.MinValue;

            if (!string.IsNullOrEmpty(value) && Trim(value).Length > 0)
                result = Trim(value)[0];

            return result;
        }

        public static char? ToCharNullable(string value)
        {
            char? result = null;

            if (!string.IsNullOrEmpty(value))
            {
                result = ToChar(value);

                if (result == char.MinValue)
                    result = null;
            }

            return result;
        }

        #endregion
        
        #region Number

        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters")]
        public static bool IsNumber<T>(string value, T notKnown, T notAvailable, T notApplicable, out T result)
        {
            result = notKnown;
            string trimmed = Trim(value);

            if (!string.IsNullOrEmpty(trimmed))
            {
                if (trimmed == "N/AV")
                    result = notAvailable;

                else if (trimmed == "N/AP")
                    result = notApplicable;

                return true;
            }

            return false;
        }

        #region Decimal

        public static decimal ToDecimal(string value)
        {
            if (IsNumber(value, DecimalNotKnown, DecimalNotAvailable, DecimalNotApplicable, out var result))
                if (decimal.TryParse(value, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, DefaultCulture, out var parsed))
                    result = parsed;

            return result;
        }

        public static decimal? ToDecimalNullable(string value)
        {
            decimal? result = ToDecimal(value);

            if (IsNull(result))
                result = null;

            return result;
        }

        #endregion

        #region Double

        public static double ToDouble(string value)
        {
            if (IsNumber(value, DoubleNotKnown, DoubleNotAvailable, DoubleNotApplicable, out var result))
                if (double.TryParse(value, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, DefaultCulture, out var parsed))
                    result = parsed;

            return result;
        }

        public static double? ToDoubleNullable(string value)
        {
            double? result = ToDouble(value);

            if (IsNull(result))
                result = null;

            return result;
        }

        #endregion

        #region Int32

        public static int ToInt32(string value)
        {
            if (IsNumber(value, Int32NotKnown, Int32NotAvailable, Int32NotApplicable, out var result))
                if (int.TryParse(value, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, DefaultCulture, out var parsed))
                    result = parsed;

            return result;
        }

        public static int? ToInt32Nullable(string value)
        {
            int? result = null;

            if (!string.IsNullOrEmpty(value))
            {
                result = ToInt32(value);

                if (result == NullInt32)
                    result = null;
            }

            return result;
        }

        #endregion

        #region Int64

        public static long ToInt64(string value)
        {
            if (IsNumber(value, Int64NotKnown, Int64NotAvailable, Int64NotApplicable, out var result))
                if (long.TryParse(value, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, DefaultCulture, out var parsed))
                    result = parsed;

            return result;
        }

        public static long? ToInt64Nullable(string value)
        {
            long? result = ToInt64(value);

            if (IsNull(result))
                result = null;

            return result;
        }

        #endregion

        #endregion

        #endregion

        #region Conversion (from object)

        public static bool ToBoolean(object value)
        {
            if (value == null || value == DBNull.Value)
                return false;

            return value is string s
                ? ToBoolean(s)
                : Convert.ToBoolean(value, DefaultCulture);
        }

        public static DateTime ToDateTime(object input)
        {
            if (input == null || input == DBNull.Value)
                throw new ArgumentNullException(nameof(input));

            return Convert.ToDateTime(input, DefaultCulture);
        }

        public static decimal ToDecimal(object value)
        {
            return Convert.ToDecimal(value, DefaultCulture);
        }

        public static double ToDouble(object value)
        {
            return Convert.ToDouble(value, DefaultCulture);
        }

        public static int ToInt32(object value)
        {
            return Convert.ToInt32(value, DefaultCulture);
        }

        public static long ToInt64(object value)
        {
            return Convert.ToInt64(value, DefaultCulture);
        }

        #endregion

        #region Helper methods

        private static bool EqualsAny(string text, string[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            foreach (string value in values)
            {
                if (Equals(text, value))
                    return true;
            }

            return false;
        }

        private static string Trim(string text)
        {
            if (text == null)
                return null;

            string result = text.Trim(' ', '\n', '\r', '\t');

            string[][] replacements =
            {
                new[] {"\xa0", " "},
                new[] {"\xa9", "(c)"},
                new[] {"\xad", "-"},
                new[] {"\xae", "(r)"},
                new[] {"\xb7", "*"},
                new[] {"\u2018", "'"},
                new[] {"\u2019", "'"},
                new[] {"\u201c", "\""},
                new[] {"\u201d", "\""},
                new[] {"\u2026", "..."},
                new[] {"\u2002", " "},
                new[] {"\u2003", " "},
                new[] {"\u2009", " "},
                new[] {"\u2013", "-"},
                new[] {"\u2014", "--"},
                new[] {"\u2122", "(tm)"}
            };

            foreach (var replacement in replacements)
            {
                result = result.Replace(replacement[0], replacement[1]);
            }

            return result;
        }

        #endregion

        public static Guid? ToGuidNullable(string value)
        {
            if (Guid.TryParse(value, out Guid id))
                return id;
            return null;
        }
    }
}