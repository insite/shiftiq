using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using Shift.Constant;

namespace Shift.Common
{
    [Serializable]
    [SuppressMessage("NDepend", "ND3101:DontUseSystemRandomForSecurityPurposes", Scope = "method", Justification = "Random number generation is not security-sensitive here, therefore weak psuedo-random numbers are permitted.")]
    public class RandomStringGenerator
    {
        private const int RetryLimit = 60;

        private readonly Random _random;
        private readonly HashSet<string> _cache;

        private readonly int _length;
        private readonly string _characters;

        private static class CharactersSet
        {
            public const string Alphabetic = "abcdefghijklmnopqrstuvwxyz";
            public const string AlphabeticCaseSensitive = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            public const string Numeric = "0123456789";
            public const string Alphanumeric = "abcdefghijklmnopqrstuvwxyz0123456789";
            public const string AlphanumericCaseSensitive = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            public const string AlphanumericCaseSensitiveAndSymbols = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ~`!@#$%^&*()_-+={[}]|\\:;\"'<,>.?/";
            public const string Hex = "0123456789abcdef";
            public const string Passcode = "23456789abcdefghjkmnpqrtwxy";
        }

        private static readonly Random _defaultRandom = new Random();
        private static readonly Random _passcodeRandom = new Random();
        private static readonly Random _userPasswordRandom = new Random();

        public RandomStringGenerator(RandomStringType valueType, int length)
        {
            if (length <= 2)
                throw new ArgumentException(ErrorMessage.RandomStringTooShort, nameof(length));

            _length = length;
            _random = new Random();
            _cache = new HashSet<string>();
            _characters = GetCharactersSet(valueType);
        }

        public string Next()
        {
            var tryCount = 0;
            var sb = new StringBuilder();

            while (true)
            {
                sb.Clear();

                AppendSequence(_random, _characters, _length, sb);

                var result = sb.ToString();
                if (!_cache.Contains(result))
                {
                    _cache.Add(result);
                    return result;
                }

                if (tryCount > RetryLimit)
                    break;

                tryCount++;
            }

            throw ApplicationError.Create("Failed to generate a unique random string value.");
        }

        public static string Create(Random random, int length)
            => GetSequence(random, CharactersSet.Alphanumeric, length);

        public static string Create(Random random, RandomStringType type, int length)
            => GetSequence(random, GetCharactersSet(type), length);

        public static string Create(int length)
        {
            lock (_defaultRandom)
                return Create(_defaultRandom, length);
        }

        public static string Create(RandomStringType type, int length)
        {
            lock (_defaultRandom)
                return Create(_defaultRandom, type, length);
        }

        /// <remarks>
        /// Generates a passcode of the form "xxxx-xxxx-xxxx" where "x" is a character or a digit that cannot be easily
        /// confused with any other character or digit. For example, the digit "0" and the letter "O" are excluded 
        /// because they can be easily confused with one another.
        /// </remarks>
        public static string CreatePasscode()
        {
            var result = new StringBuilder();

            lock (_passcodeRandom)
            {
                AppendSequence(_passcodeRandom, CharactersSet.Passcode, 4, result);
                result.Append("-");
                AppendSequence(_passcodeRandom, CharactersSet.Passcode, 4, result);
                result.Append("-");
                AppendSequence(_passcodeRandom, CharactersSet.Passcode, 4, result);
            }

            return result.ToString();
        }

        /// <remarks>
        /// Generates a 12-character password, starting with 3 uppercase letters, followed by a symbol, 4 lowercase 
        /// letters, and then 4 digits. The symbol is randomly selected from the set ".-!?$".
        /// </remarks>
        public static string CreateUserPassword()
        {
            const string symbols = ".-!?$";

            var result = new StringBuilder();

            lock (_userPasswordRandom)
            {
                for (int i = 0; i < 3; i++)
                {
                    var code = _userPasswordRandom.Next('Z' - 'A' + 1);
                    result.Append((char)('A' + code));
                }

                result.Append(symbols[_userPasswordRandom.Next(symbols.Length)]);

                for (int i = 0; i < 4; i++)
                {
                    var code = _userPasswordRandom.Next('z' - 'a' + 1);
                    result.Append((char)('a' + code));
                }

                for (int i = 0; i < 4; i++)
                {
                    var code = _userPasswordRandom.Next('9' - '0' + 1);
                    result.Append((char)('0' + code));
                }
            }

            return result.ToString();
        }

        #region Methods (helpers)

        private static string GetCharactersSet(RandomStringType type)
        {
            switch (type)
            {
                case RandomStringType.Alphabetic:
                    return CharactersSet.Alphabetic;
                case RandomStringType.AlphabeticCaseSensitive:
                    return CharactersSet.AlphabeticCaseSensitive;
                case RandomStringType.Numeric:
                    return CharactersSet.Numeric;
                case RandomStringType.Alphanumeric:
                    return CharactersSet.Alphanumeric;
                case RandomStringType.AlphanumericCaseSensitive:
                    return CharactersSet.AlphanumericCaseSensitive;
                case RandomStringType.AlphanumericCaseSensitiveAndSymbols:
                    return CharactersSet.AlphanumericCaseSensitiveAndSymbols;
                case RandomStringType.Hex:
                    return CharactersSet.Hex;
                case RandomStringType.Passcode:
                    return CharactersSet.Passcode;
                default:
                    throw ApplicationError.Create("Unexpected Value Type: {0}", type.GetName());
            }
        }

        private static void AppendSequence(Random random, string characters, int length, StringBuilder sb)
        {
            var setLength = characters.Length;

            for (var i = 0; i < length; i++)
            {
                var index = random.Next(setLength);
                sb.Append(characters[index]);
            }
        }

        private static string GetSequence(Random random, string characters, int length)
        {
            var sb = new StringBuilder();

            AppendSequence(random, characters, length, sb);

            return sb.ToString();
        }

        #endregion
    }
}