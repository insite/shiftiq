using System;
using System.Diagnostics.CodeAnalysis;

namespace Shift.Common
{
    /// <remarks>
    /// Represents a pseudo-random number generator, wrapping Microsoft's implementation in the .NET Framework Random class, so we
    /// don't need to worry about seeding the generator in our client code.
    /// </remarks>
    [SuppressMessage("NDepend", "ND3101:DontUseSystemRandomForSecurityPurposes", Scope = "method", Justification = "Random number generation is not security-sensitive here, therefore weak psuedo-random numbers are permitted.")]
    public class RandomNumberGenerator
    {
        private readonly Random _random;

        public static readonly RandomNumberGenerator Instance = new RandomNumberGenerator();

        public RandomNumberGenerator()
        {
            var seed = Guid.NewGuid().GetHashCode();
            _random = new Random(seed);
        }

        /// <remarks>
        /// Returns a random integer between -2147483648 (inclusive) and 2147483647 (exclusive).
        /// </remarks>
        public int Next()
        {
            return Next(int.MinValue, int.MaxValue);
        }

        /// <remarks>
        /// Returns a random integer between the lower bound (inclusive) and the upper bound (exclusive). In other words, a series
        /// of calls to Next(1,10) will receive values in the range 1..9 (NOT in the range 1..10).
        /// </remarks>
        public int Next(int inclusiveLowerBound, int exclusiveUpperBound)
        {
            return _random.Next(inclusiveLowerBound, exclusiveUpperBound);
        }
    }
}