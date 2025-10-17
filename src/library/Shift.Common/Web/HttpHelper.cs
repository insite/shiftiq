using System;

namespace Shift.Common
{
    public static class HttpHelper
    {
        public static byte[] UrlTokenDecode(string input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            var length = input.Length;
            if (length < 1)
                return new byte[0];

            var num = input[length - 1] - 48;
            if (num < 0 || num > 10)
                return null;

            var array = new char[length - 1 + num];
            for (var i = 0; i < length - 1; i++)
            {
                var c = input[i];
                switch (c)
                {
                    case '-':
                        array[i] = '+';
                        break;
                    case '_':
                        array[i] = '/';
                        break;
                    default:
                        array[i] = c;
                        break;
                }
            }

            for (var j = length - 1; j < array.Length; j++)
                array[j] = '=';

            return Convert.FromBase64CharArray(array, 0, array.Length);
        }

        public static string UrlTokenEncode(byte[] input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (input.Length < 1)
                return string.Empty;

            var text = Convert.ToBase64String(input);
            if (text == null)
                return null;

            var num = text.Length;
            while (num > 0 && text[num - 1] == '=')
                num--;

            var array = new char[num + 1];
            array[num] = (char)(48 + text.Length - num);

            for (var i = 0; i < num; i++)
            {
                var c = text[i];
                switch (c)
                {
                    case '+':
                        array[i] = '-';
                        break;
                    case '/':
                        array[i] = '_';
                        break;
                    case '=':
                        array[i] = c;
                        break;
                    default:
                        array[i] = c;
                        break;
                }
            }

            return new string(array);
        }
    }
}
