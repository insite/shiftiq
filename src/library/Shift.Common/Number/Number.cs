using System.Linq;

namespace Shift.Common
{
    public static class Number
    {
        public static bool IsNumeric(string text)
        {
            return !string.IsNullOrEmpty(text) && text.All(char.IsDigit);
        }

        public static int CheckRange(int value, int? minValue = null, int? maxValue = null)
        {
            if (minValue.HasValue && value < minValue.Value)
                value = minValue.Value;

            if (maxValue.HasValue && value > maxValue.Value)
                value = maxValue.Value;

            return value;
        }

        public static decimal CheckRange(decimal value, decimal? minValue = null, decimal? maxValue = null)
        {
            if (minValue.HasValue && value < minValue.Value)
                value = minValue.Value;

            if (maxValue.HasValue && value > maxValue.Value)
                value = maxValue.Value;

            return value;
        }
        
        public static double CheckRange(double value, double? minValue = null, double? maxValue = null)
        {
            if (minValue.HasValue && value < minValue.Value)
                value = minValue.Value;

            if (maxValue.HasValue && value > maxValue.Value)
                value = maxValue.Value;

            return value;
        }

        public static float CheckRange(float value, float? minValue = null, float? maxValue = null)
        {
            if (minValue.HasValue && value < minValue.Value)
                value = minValue.Value;

            if (maxValue.HasValue && value > maxValue.Value)
                value = maxValue.Value;

            return value;
        }

        public static bool IsInRange(int value, int? minValue = null, int? maxValue = null)
        {
            if (minValue.HasValue && value < minValue.Value)
                return false;

            if (maxValue.HasValue && value > maxValue.Value)
                return false;

            return true;
        }

        public static bool IsInRange(decimal value, decimal? minValue = null, decimal? maxValue = null)
        {
            if (minValue.HasValue && value < minValue.Value)
                return false;

            if (maxValue.HasValue && value > maxValue.Value)
                return false;

            return true;
        }

        public static bool IsInRange(double value, double? minValue = null, double? maxValue = null)
        {
            if (minValue.HasValue && value < minValue.Value)
                return false;

            if (maxValue.HasValue && value > maxValue.Value)
                return false;

            return true;
        }

        public static int? NullIfOutOfRange(int? value, int? minValue = null, int? maxValue = null)
        {
            if (!value.HasValue)
                return value;

            if (minValue.HasValue && value < minValue.Value)
                value = null;

            if (maxValue.HasValue && value > maxValue.Value)
                value = null;

            return value;
        }
    }
}