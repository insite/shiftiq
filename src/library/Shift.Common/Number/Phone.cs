using System;
using System.Text.RegularExpressions;

namespace Shift.Common
{
    [Serializable]
    public class Phone
    {
        private const int MaxDigits = 22;
        private const int MaxDbLength = 32;

        private static readonly Regex RegexPattern1 = new Regex(@"(.+)ext(.*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RegexPattern2 = new Regex(@"(.+)x(.*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public string Extension { get; set; }
        public string Number { get; set; }
        public string PlainNumber => GetPlainNumber(Number);

        public Phone(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            Number = text;

            var match = RegexPattern1.Match(text);
            if (!match.Success)
                match = RegexPattern2.Match(text);

            if (match.Success)
            {
                Number = match.Groups[1].Value;
                Extension = match.Groups[2].Value;
            }
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Number))
                return string.Empty;

            var number = GetPlainNumber(Number);

            if (number.Length > MaxDigits)
                number = number.Substring(0, MaxDigits);

            var extension = GetPlainNumber(Extension);

            if (number.Length == 10)
            {
                number = $"({number.Substring(0, 3)}) {number.Substring(3, 3)}-{number.Substring(6, 4)}";
            }
            else if (number.Length > 10)
            {
                var startIndex = (number.Length - 4) % 3;
                var count = (number.Length - 4) / 3;
                var formatted = startIndex > 0
                    ? number.Substring(0, startIndex) + "-"
                    : string.Empty;

                for (var i = 0; i < count; i++)
                    formatted += number.Substring(startIndex + i * 3, 3) + "-";

                number = formatted + number.Substring(number.Length - 4, 4);
            }

            var result = !string.IsNullOrEmpty(extension)
                ? $"{number} ext {extension}"
                : number;

            if (result.Length > MaxDbLength)
                result = result.Substring(0, MaxDbLength);

            return result;
        }

        public static string Format(string number, string areaCode)
        {
            if (string.IsNullOrEmpty(number))
                return null;

            var phone = GetPlainNumber(number);
            if (phone.Length == 7 && !string.IsNullOrEmpty(areaCode))
                phone = areaCode + phone;

            return Format(phone);
        }

        public static string Format(string number)
        {
            return (new Phone(number)).ToString();
        }

        public static string GetPlainNumber(string number)
        {
            var output = string.Empty;

            if (!string.IsNullOrEmpty(number))
            {
                var text = StringHelper.TrimAndClean(number);

                foreach (var c in text)
                {
                    if ('0' <= c && c <= '9')
                        output += c;
                }
            }

            return output;
        }
    }
}
