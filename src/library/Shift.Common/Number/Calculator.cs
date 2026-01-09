using System;
using System.Collections.Generic;
using System.Linq;

namespace Shift.Common
{
    public static class Calculator
    {
        #region Statistic

        public static decimal CalculateAverage(decimal[] values)
            => values.Average();

        public static double CalculateMedian(IEnumerable<double> values)
        {
            var data = values.OrderBy(x => x).ToArray();
            if (data.Length <= 1)
                return double.NaN;

            var index = (int)Math.Ceiling((double)data.Length / 2) - 1;

            return data.Length % 2 != 0
                ? data[index]
                : (data[index] + data[index + 1]) / 2;
        }

        public static double CalculatePopulationVariance(IEnumerable<double> values)
        {
            var mean = values.Average();

            return values.Average(x => Math.Pow(x - mean, 2));
        }

        public static double CalculateCovariance(IEnumerable<double> x, IEnumerable<double> y)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));

            if (y == null)
                throw new ArgumentNullException(nameof(y));

            var length = x.Count();

            if (length != y.Count())
                throw new ArgumentException("Cannot calculate the covariance for arrays with different lengths");

            if (length == 0)
                throw new ArgumentException("Cannot calculate the covariance for an empty array");

            if (length == 1)
                throw new ArgumentException("Cannot calculate the covariance for an array with only one element");

            var xMean = x.Average();
            var yMean = y.Average();

            return x.Zip(y, (xValue, yValue) => (xValue - xMean) * (yValue - yMean)).Sum() / (length - 1);
        }

        public static double CalculateStandardDeviation(IEnumerable<double> values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var count = values.Count();

            if (count <= 1)
                return double.NaN;

            var mean = values.Average();

            return Math.Sqrt(values.Sum(x => Math.Pow(x - mean, 2)) / (count - 1));
        }

        public static double CalculateCronbachAlpha(List<List<double>> data) =>
            CalculateCronbachAlpha(data.Select(x => x.ToArray()).ToArray());

        public static double CalculateCronbachAlpha(double[][] data)
        {
            var count = -1;

            for (var i = 0; i < data.Length; i++)
            {
                if (count == -1)
                {
                    count = data[i].Length;
                }
                else if (count != data[i].Length)
                {
                    return double.NaN;
                }
            }

            if (count <= 1)
            {
                return double.NaN;
            }

            var qSumVar = 0.0;
            var tVar = 0.0;

            for (var i = 0; i < count; i++)
            {
                var scores = data.Select(x => x[i]);

                qSumVar += CalculatePopulationVariance(scores);
            }

            {
                var scores = data.Select(x => x.Sum());

                tVar = CalculatePopulationVariance(scores);
            }

            return count / (count - 1.0) * (1.0 - qSumVar / tVar);
        }

        #endregion

        #region Correlation

        public static double CalculateCorrelation(IEnumerable<double> x, IEnumerable<double> y)
        {
            if (x.Count() == 0 || y.Count() == 0)
                return 0;

            var sd = CalculateStandardDeviation(x) * CalculateStandardDeviation(y);

            if (sd == 0)
                return 0;

            return CalculateCovariance(x, y) / sd;
        }

        /// <summary>
        /// Calculate "Pearson Correlation Coefficient"
        /// </summary>
        /// <param name="p_m">Percentage of true answers on question "m"</param>
        /// <param name="p_k">Percentage of true answers on question "k"</param>
        /// <param name="p_m_k">Percentage of true answers on both questions: "m" and "k"</param>
        public static double CalculateCorrelation(double p_m, double p_k, double p_m_k)
        {
            if (p_m_k == 0) return 0;

            return (p_m_k - p_m * p_k) / Math.Sqrt(p_m * (1 - p_m) * p_k * (1 - p_k));
        }

        #endregion

        #region Division

        /// <summary>
        ///     Returns zero if the denominator is zero.
        /// </summary>
        public static Double Divide(Double numerator, Double denominator)
        {
            Double x = numerator;
            Double y = denominator;

            if (Math.Abs(y - 0) > Double.Epsilon)
                return x / y;

            return 0;
        }

        #endregion

        #region Conversion

        /// <summary>
        /// Convert a positive Base 10 numeric value to Base 26. Zero is unsupported (by design).
        /// </summary>
        public static string ToBase26(int number)
        {
            if (number < 1)
                return "?";

            var array = new LinkedList<int>();

            while (number > 26)
            {
                var value = number % 26;
                if (value == 0)
                {
                    number = number / 26 - 1;
                    array.AddFirst(26);
                }
                else
                {
                    number /= 26;
                    array.AddFirst(value);
                }
            }

            if (number > 0) array.AddFirst(number);
            return new string(array.Select(s => (char)('A' + s - 1)).ToArray());
        }

        /// <summary>
        /// Convert a positive Base 26 numeric value to Base 10. Zero is unsupported (by design).
        /// </summary>
        public static int FromBase26(string number)
        {
            var base10 = 0;

            var digits = number.ToUpper().ToCharArray();
            var last = digits.Length - 1;

            for (var x = 0; x < digits.Length; x++)
            {
                if (digits[x] < 'A' || digits[x] > 'Z')
                    throw new ArgumentException(@"This is not a valid Base 26 number: " + number, number);

                base10 += (int)(Math.Pow(26, x) * (digits[last - x] - 'A' + 1));
            }

            return base10;
        }

        #endregion

        #region Other

        public static decimal GetPercentDecimal(decimal part, int total) =>
            Math.Round(part / total, 2, MidpointRounding.AwayFromZero);

        public static decimal GetPercentDecimal(decimal part, decimal total) =>
            Math.Round(part / total, 2, MidpointRounding.AwayFromZero);

        public static decimal GetPercentDecimal(int part, int total, int decimals) =>
            Math.Round((decimal)part / total, decimals, MidpointRounding.AwayFromZero);

        public static decimal GetPercentDecimal(decimal part, decimal total, int decimals) =>
            Math.Round(part / total, decimals, MidpointRounding.AwayFromZero);

        public static double GetPercentDouble(double part, double total, int decimals) =>
            Math.Round(part / total, decimals, MidpointRounding.AwayFromZero);

        public static int GetPercentInteger(double part, double total) =>
            (int)Math.Round(100d * part / total, 0, MidpointRounding.AwayFromZero);

        #endregion
    }
}
