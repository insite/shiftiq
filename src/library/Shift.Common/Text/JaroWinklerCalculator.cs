using System;

namespace Shift.Common
{
    public static class JaroWinklerCalculator
    {
        /* The Winkler modification will not be applied unless the 
         * percent match was at or above the mWeightThreshold percent 
         * without the modification. 
         * Winkler's paper used a default value of 0.7
         */
        private static readonly double _threshold = 0.7;

        /* Size of the prefix to be concidered by the Winkler modification. 
         * Winkler's paper used a default value of 4
         */
        private static readonly int _prefix = 4;
        
        /// <summary>
        /// Returns the Jaro-Winkler distance between the specified  
        /// strings. The distance is symmetric and will fall in the 
        /// range 0 (perfect match) to 1 (no match). 
        /// </summary>
        public static double Distance( string aString1, string aString2 )
        {
            return 1.0 - Proximity( aString1, aString2 );
        }
        
        /// <summary>
        /// Returns the Jaro-Winkler distance between the specified  
        /// strings. The distance is symmetric and will fall in the 
        /// range 0 (no match) to 1 (perfect match). 
        /// </summary>
        public static double Proximity( string a, string b )
        {
            if (a.Length == 0)
                return b.Length == 0 ? 1.0 : 0.0;

            var aMatches = new bool[a.Length];
            var bMatched = new bool[b.Length];

            var common = CountCommon( a, b, aMatches, bMatched );
            if (common == 0) return 0.0;

            var transposed = CountTransposed( a, b, aMatches, bMatched );

            return CalculateProximity( a, b, transposed, common);
        }

        private static int CountCommon(string a, string b, bool[] aMatched, bool[] bMatched)
        {
            var lSearchRange = Math.Max( 0, Math.Max( a.Length, b.Length ) / 2 - 1 );
            var lNumCommon = 0;
            for (var i = 0; i < a.Length; ++i)
            {
                var lStart = Math.Max( 0, i - lSearchRange );
                var lEnd = Math.Min( i + lSearchRange + 1, b.Length );
                for (var j = lStart; j < lEnd; ++j)
                {
                    if (bMatched[j]) continue;
                    if (a[i] != b[j])
                        continue;
                    aMatched[i] = true;
                    bMatched[j] = true;
                    ++lNumCommon;
                    break;
                }
            }
            return lNumCommon;
        }

        private static int CountTransposed(string a, string b, bool[] aMatched, bool[] bMatched)
        {
            var lNumHalfTransposed = 0;
            var k = 0;
            for (var i = 0; i < a.Length; ++i)
            {
                if (!aMatched[i]) continue;
                while (!bMatched[k]) ++k;
                if (a[i] != b[k])
                    ++lNumHalfTransposed;
                ++k;
            }
            return lNumHalfTransposed;
        }

        private static double CalculateProximity(string a, string b, int halfTransposed, int common)
        {
            var lNumTransposed = halfTransposed / 2;

            double lNumCommonD = common;
            var lWeight = (lNumCommonD / a.Length
                             + lNumCommonD / b.Length
                             + (common - lNumTransposed) / lNumCommonD) / 3.0;

            if (lWeight <= _threshold) return lWeight;
            var lMax = Math.Min( _prefix, Math.Min( a.Length, b.Length ) );
            var lPos = 0;
            while (lPos < lMax && a[lPos] == b[lPos])
                ++lPos;
            if (lPos == 0) return lWeight;
            return lWeight + 0.1 * lPos * (1.0 - lWeight);
        }
    }
}
