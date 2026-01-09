using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;

namespace Shift.Common
{
    public static class StringHelper
    {
        public static readonly CultureInfo CurrentCulture = new CultureInfo("en-US");

        private static readonly Regex WordsParseRegex = new Regex("\\w+", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Dictionary<string, string> CleanReplacements;
        private static readonly Regex CleanRegex;
        private static readonly char[] TrimCharacters = new[] { ' ', '\n', '\r', '\t' };
        private static readonly Regex TrimInsideRegex = new Regex(@"[\s]+", RegexOptions.Compiled);

        private static readonly uint[] ByteHexMapping;
        private static readonly int[] HexNibbleMapping;

        static StringHelper()
        {
            CleanReplacements = new Dictionary<string, string>
            {
                {"\xa0", " "},
                {"\xa9", "(c)"},
                {"\xad", "-"},
                {"\xae", "(r)"},
                {"\xb7", "*"},
                {"\u2018", "'"},
                {"\u2019", "'"},
                {"\u201c", "\""},
                {"\u201d", "\""},
                {"\u2026", "..."},
                {"\u2002", " "},
                {"\u2003", " "},
                {"\u2009", " "},
                {"\u2013", "-"},
                {"\u2014", "--"},
                {"\u2122", "(tm)"}
            };
            CleanRegex = new Regex("(" + string.Join("|", CleanReplacements.Keys) + ")", RegexOptions.Compiled);

            ByteHexMapping = new uint[256];
            for (var i = 0; i < ByteHexMapping.Length; i++)
            {
                var s = i.ToString("X2");
                ByteHexMapping[i] = ((uint)s[0]) + ((uint)s[1] << 16);
            }

            var hexNibble = new List<int>();
            for (var ch = '0'; ch <= 'F'; ch++)
            {
                if (ch >= '0' && ch <= '9')
                    hexNibble.Add(ch - '0');
                else if (ch >= 'A' && ch <= 'F')
                    hexNibble.Add(ch - 'A' + 10);
                else
                    hexNibble.Add(0);
            }
            HexNibbleMapping = hexNibble.ToArray();
        }

        #region Comparing

        /// <summary>
        ///     Returns true if the text contains any one of the values in the
        ///     array. This function is not case-sensitive.
        /// </summary>
        public static bool ContainsAny(string text, string[] values)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            if (values == null)
                throw new ArgumentNullException(nameof(values));

            string lowercaseText = text.ToLower(CurrentCulture);

            foreach (string value in values)
            {
                if (lowercaseText.Contains(value.ToLower(CurrentCulture)))
                    return true;
            }

            return false;
        }

        /// <summary>
        ///     Returns true if the text ends with any one of the values in the
        ///     array. This function is not case-sensitive.
        /// </summary>
        public static bool EndsWithAny(string text, string[] values)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            if (values == null)
                throw new ArgumentNullException(nameof(values));

            foreach (string value in values)
            {
                if (text.EndsWith(value, true, null))
                    return true;
            }

            return false;
        }

        public static bool EndsWith(string text, string value)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return text.EndsWith(value, true, null);
        }

        /// <summary>
        ///     Returns true if the text equals the value. This function is not
        ///     case-sensitive. Two null references compare equal to each other.
        /// </summary>
        public static bool Equals(string text, string value)
        {
            return string.Compare(text, value, true, CurrentCulture) == 0;
        }

        public static bool EqualsCaseSensitive(string text, string value, bool treatNullAsEmpty)
        {
            return treatNullAsEmpty && EqualsCaseSensitive(text ?? "", value ?? "")
                || !treatNullAsEmpty && EqualsCaseSensitive(text, value);
        }

        public static bool EqualsCaseSensitive(string text, string value)
        {
            return string.Equals(text, value, StringComparison.CurrentCulture);
        }

        /// <summary>
        ///     Returns true if the text equals any one of the values in the
        ///     array. This function is not case-sensitive.
        /// </summary>
        public static bool EqualsAny(string text, string[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            for (var i = 0; i < values.Length; i++)
            {
                if (Equals(text, values[i]))
                    return true;
            }

            return false;
        }

        public static bool IsFileNameMatch(string one, string two)
        {
            if (string.IsNullOrWhiteSpace(one) || string.IsNullOrWhiteSpace(two))
                return false;

            var x = Sanitize(Uri.UnescapeDataString(one), '-');
            var y = Sanitize(Uri.UnescapeDataString(two), '-');

            return Equals(x, y);
        }

        /// <summary>
        ///     Returns the collection of all regular-expression matches between
        ///     the text and each of the patterns in the array. This function is
        ///     not case-sensitive.
        /// </summary>
        public static MatchCollection Matches(string text, params string[] patterns)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            if (patterns == null)
                throw new ArgumentNullException(nameof(patterns));

            var sb = new StringBuilder(patterns.Length + 50);

            foreach (string pattern in patterns)
                sb.AppendFormat("({0})|", pattern.Trim());

            string expression = sb.ToString().TrimEnd('|');

            var re = new Regex(expression, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return re.Matches(text);
        }

        /// <summary>
        ///     Returns true if the text starts with any one of the values in the
        ///     array. This function is not case-sensitive.
        /// </summary>
        public static bool StartsWithAny(string text, string[] values)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            if (values == null)
                throw new ArgumentNullException(nameof(values));

            foreach (string value in values)
            {
                if (text.StartsWith(value, true, null))
                    return true;
            }

            return false;
        }

        public static bool StartsWith(string text, string value)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return text.StartsWith(value, true, null);
        }

        #endregion

        #region Encoding (HEX)

        public static string ByteArrayToHex(byte[] value)
        {
            var result = new char[value.Length * 2];
            for (int i = 0, j = 0; i < value.Length; i++, j += 2)
                ByteToHex(result, j, value[i]);
            return new string(result);
        }

        public static byte[] HexToByteArray(string value)
        {
            var result = new byte[value.Length / 2];
            for (int i = 0, j = 0; j < value.Length; i++, j += 2)
                result[i] = HexToByte(value[j], value[j + 1]);
            return result;
        }

        public static string ByteToHex(byte value)
        {
            var result = new char[2];
            ByteToHex(result, 0, value);
            return new string(result);
        }

        public static byte HexToByte(string value)
        {
            return HexToByte(value[0], value[1]);
        }

        public static string Int16ToHex(short value)
        {
            var result = new char[4];
            ByteToHex(result, 0, (byte)(value >> 8));
            ByteToHex(result, 2, (byte)value);
            return new string(result);
        }

        public static short HexToInt16(string value)
        {
            return (short)((HexToByte(value[0], value[1]) << 8)
                 | (HexToByte(value[2], value[3])));
        }

        public static string Int32ToHex(int value)
        {
            var result = new char[8];
            ByteToHex(result, 0, (byte)(value >> 24));
            ByteToHex(result, 2, (byte)(value >> 16));
            ByteToHex(result, 4, (byte)(value >> 8));
            ByteToHex(result, 6, (byte)value);
            return new string(result);
        }

        public static int HexToInt32(string value)
        {
            return (HexToByte(value[0], value[1]) << 24)
                 | (HexToByte(value[2], value[3]) << 16)
                 | (HexToByte(value[4], value[5]) << 8)
                 | (HexToByte(value[6], value[7]));
        }

        public static string Int64ToHex(long value)
        {
            var result = new char[16];
            ByteToHex(result, 0, (byte)(value >> 56));
            ByteToHex(result, 2, (byte)(value >> 48));
            ByteToHex(result, 4, (byte)(value >> 40));
            ByteToHex(result, 6, (byte)(value >> 32));
            ByteToHex(result, 8, (byte)(value >> 24));
            ByteToHex(result, 10, (byte)(value >> 16));
            ByteToHex(result, 12, (byte)(value >> 8));
            ByteToHex(result, 14, (byte)value);
            return new string(result);
        }

        public static long HexToInt64(string value)
        {
            return ((long)HexToByte(value[0], value[1]) << 56)
                 | ((long)HexToByte(value[2], value[3]) << 48)
                 | ((long)HexToByte(value[4], value[5]) << 40)
                 | ((long)HexToByte(value[6], value[7]) << 32)
                 | ((long)HexToByte(value[8], value[9]) << 24)
                 | ((long)HexToByte(value[10], value[11]) << 16)
                 | ((long)HexToByte(value[12], value[13]) << 8)
                 | ((long)HexToByte(value[14], value[15]));
        }

        private static void ByteToHex(char[] array, int offset, byte value)
        {
            var mapping = ByteHexMapping[value];
            array[offset] = (char)mapping;
            array[offset + 1] = (char)(mapping >> 16);
        }

        private static byte HexToByte(char high, char low)
        {
            return (byte)(HexNibbleMapping[char.ToUpper(high) - '0'] << 4 | HexNibbleMapping[char.ToUpper(low) - '0']);
        }

        #endregion

        #region Encoding (Base 64)

        public static string DecodeBase64(string text)
        {
            var buffer = Convert.FromBase64String(text);
            return Encoding.UTF8.GetString(buffer);
        }

        public static string DecodeBase64Url(string text)
        {
            var buffer = HttpHelper.UrlTokenDecode(text);
            return Encoding.UTF8.GetString(buffer ?? throw new InvalidOperationException());
        }

        public static string EncodeBase64(string text)
        {
            var buffer = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(buffer);
        }

        public static string Snip(string text, int maxLength, string elipsis = "...")
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            if (text.Length <= maxLength)
                return text;

            if (text.Length > elipsis.Length)
                return text.Substring(0, maxLength - elipsis.Length) + elipsis;

            if (text.Length <= elipsis.Length)
                throw new ArgumentOutOfRangeException();

            return text.Substring(0, maxLength);
        }

        public static string EncodeBase64Url(string text)
        {
            var buffer = Encoding.UTF8.GetBytes(text);
            return HttpHelper.UrlTokenEncode(buffer);
        }

        public static void DecodeBase64(string data, Action<Stream> read)
        {
            var bytes = Convert.FromBase64String(data);
            using (var ms = new MemoryStream(bytes))
                read(ms);
        }

        public static void DecodeBase64Url(string data, Action<Stream> read)
        {
            var bytes = HttpHelper.UrlTokenDecode(data);
            using (var ms = new MemoryStream(bytes))
                read(ms);
        }

        public static TResult DecodeBase64<TResult>(string data, Func<Stream, TResult> read)
        {
            var bytes = Convert.FromBase64String(data);
            using (var ms = new MemoryStream(bytes))
                return read(ms);
        }

        public static TResult DecodeBase64Url<TResult>(string data, Func<Stream, TResult> read)
        {
            var bytes = HttpHelper.UrlTokenDecode(data);
            using (var ms = new MemoryStream(bytes))
                return read(ms);
        }

        public static string EncodeBase64(Action<Stream> write)
        {
            byte[] data;

            using (var ms = new MemoryStream())
            {
                write(ms);
                data = ms.ToArray();
            }

            return Convert.ToBase64String(data);
        }

        public static string EncodeBase64Url(Action<Stream> write)
        {
            byte[] data;

            using (var ms = new MemoryStream())
            {
                write(ms);
                data = ms.ToArray();
            }

            return HttpHelper.UrlTokenEncode(data);
        }

        public static string EncodeBase64(object data, string key)
        {
            return EncodeBase64(key, stream => new BinaryFormatter().Serialize(stream, data));
        }

        public static string EncodeBase64Url(object data, string key)
        {
            return EncodeBase64Url(key, stream => new BinaryFormatter().Serialize(stream, data));
        }

        public static string EncodeBase64(string key, Action<Stream> write)
        {
            var data = EncryptionHelper.Encode(key, null, write);

            return Convert.ToBase64String(data);
        }

        public static string EncodeBase64(string key, byte[] salt, Action<Stream> write)
        {
            if (salt.IsEmpty())
                throw new ArgumentNullException(nameof(salt));

            var data = EncryptionHelper.Encode(key, salt, write);
            data = ArrayHelper.Combine(salt, data);

            return Convert.ToBase64String(data);
        }

        public static string EncodeBase64Url(string key, Action<Stream> write)
        {
            var data = EncryptionHelper.Encode(key, null, write);

            return HttpHelper.UrlTokenEncode(data);
        }

        public static string EncodeBase64Url(string key, byte[] salt, Action<Stream> write)
        {
            if (salt.IsEmpty())
                throw new ArgumentNullException(nameof(salt));

            var data = EncryptionHelper.Encode(key, salt, write);
            data = ArrayHelper.Combine(salt, data);

            return HttpHelper.UrlTokenEncode(data);
        }

        public static object DecodeBase64(string data, string key)
        {
            return DecodeBase64(data, key, stream => new BinaryFormatter().Deserialize(stream));
        }

        public static object DecodeBase64Url(string data, string key)
        {
            return DecodeBase64Url(data, key, stream => new BinaryFormatter().Deserialize(stream));
        }

        public static object DecodeBase64(string data, string key, Func<Stream, object> read)
        {
            var byteData = Convert.FromBase64String(data);

            return EncryptionHelper.Decode(byteData, 0, byteData.Length, key, null, read);
        }

        public static object DecodeBase64(string data, string key, int saltLength, Func<Stream, object> read)
        {
            var byteData = Convert.FromBase64String(data);
            if (byteData == null)
                throw new ApplicationError("Invalid input data");

            if (saltLength <= 0 || byteData.Length <= saltLength)
                throw new ApplicationError("Invalid salt length: " + saltLength);

            var salt = ArrayHelper.Get(byteData, 0, saltLength);

            return EncryptionHelper.Decode(byteData, 0, byteData.Length, key, null, read);
        }

        public static object DecodeBase64Url(string data, string key, Func<Stream, object> read)
        {
            var byteData = HttpHelper.UrlTokenDecode(data);
            if (byteData == null)
                throw new ApplicationError("Invalid input data");

            return EncryptionHelper.Decode(byteData, 0, byteData.Length, key, null, read);
        }

        public static object DecodeBase64Url(string data, string key, int saltLength, Func<Stream, object> read)
        {
            var byteData = HttpHelper.UrlTokenDecode(data);
            if (byteData == null)
                throw new ApplicationError("Invalid input data");

            if (saltLength <= 0 || byteData.Length <= saltLength)
                throw new ApplicationError("Invalid salt length: " + saltLength);

            var salt = ArrayHelper.Get(byteData, 0, saltLength);

            return EncryptionHelper.Decode(byteData, saltLength, byteData.Length - salt.Length, key, salt, read);
        }

        #endregion

        #region Encoding (HTML)

        public static string PadLeftWithHtmlSpaces(string text, int totalWidth)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            const string space = "&nbsp;";
            return PadLeft(text, space, (totalWidth - text.Length) * space.Length);
        }

        public static string PadRightWithHtmlSpaces(string text, int totalWidth)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            const string space = "&nbsp;";
            return PadRight(text, space, (totalWidth - text.Length) * space.Length);
        }

        public static string ReplaceNewLinesWithHtmlBreaks(string text)
        {
            if (text == null)
                return null;

            var re = new Regex(@"[\n|\r]");
            return re.Replace(text.Replace("\r\n", "<br />"), "<br />");
        }

        public static string ReplaceHtmlBreaksWithNewLines(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            string temp = Replace(text, new[] { "<br />", "<br >", "<br/>", "<br>" }, "\n", false);
            return Replace(temp, "</p>", "</p>\n");
        }

        public static string ReplaceSpacesWithHtmlSpaces(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            return text.Replace(" ", "&nbsp;");
        }

        public static string StripHtml(string html)
        {
            return StripHtml(html, string.Empty, true);
        }

        public static string StripHtml(string html, string placeholder)
        {
            return StripHtml(html, placeholder, true);
        }

        public static string StripHtml(string html, string placeholder, bool stripExcessSpaces)
        {
            if (string.IsNullOrEmpty(html))
                return null;

            string text = Regex.Replace(html, @"<(.|\n)*?>", placeholder);
            text = ReplaceHtmlSpecialCharacters(text);
            text = RemoveWordSpecialCharacters(text);

            if (stripExcessSpaces)
            {
                // If there is excess whitespace, this will compress empty 
                // strings like the one in here: "THE      WORD"

                char[] delim = { ' ' };
                string[] lines = text.Split(delim, StringSplitOptions.RemoveEmptyEntries);

                var sb = new StringBuilder();
                foreach (string s in lines)
                {
                    sb.Append(s);
                    sb.Append(" ");
                }
                text = sb.ToString().Trim();
            }

            return text;
        }

        private static readonly char[] BrokeHtmlStartChars = new char[] { '<', '&' };

        public static string BreakHtml(string html)
        {
            if (html.IsEmpty())
                return html;

            var index = html.IndexOfAny(BrokeHtmlStartChars);
            if (index == -1)
                return html;

            var output = new StringBuilder();
            var minIndex = html.Length - 2;
            var prevInsertIndex = 0;

            while (index >= 0 && index <= minIndex)
            {
                var ch1 = html[index];
                var ch2 = html[index + 1];

                if (ch1 == '<')
                {
                    if (IsLatinLetter(ch2) || ch2 == '!' || ch2 == '/' || ch2 == '?')
                    {
                        Insert(ch1, ' ', ch2);
                        prevInsertIndex = index + 2;
                    }
                }
                else if (ch1 == '&')
                {
                    if (IsLatinLetter(ch2) || ch2 == '#')
                    {
                        if (index > 0 && html[index - 1] != ' ')
                            Insert(' ', ch1, ' ', ch2);
                        else
                            Insert(ch1, ' ', ch2);

                        prevInsertIndex = index + 2;
                    }
                }
                else
                    throw new NotImplementedException();

                index = html.IndexOfAny(BrokeHtmlStartChars, index + 1);
            }

            if (output.Length == 0)
                return html;

            if (prevInsertIndex < html.Length)
                output.Append(html, prevInsertIndex, html.Length - prevInsertIndex);

            return output.ToString();

            void Insert(params char[] value)
            {
                output.Append(html, prevInsertIndex, index - prevInsertIndex).Append(value);
            }

            bool IsLatinLetter(char ch)
            {
                return ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z';
            }
        }

        public static string ReplaceHtmlSpecialCharacters(string input)
        {
            string[][] specials =
            {
                new[] {" ", "&nbsp;", "&#160;", "&#xA0;", "Non-breaking space"}
                , new[] {"&", "&amp;", "&#38;", "&#x26;", "Ampersand"}
                , new[] {"\"", "&quot;", "&#34;", "&#x22;", "Double quote"}
                , new[] {"'", "&lsquo;", "&#145;", "&#x91;", "&#8216;", "Left single-quote"}
                , new[] {"'", "&rsquo;", "&#146;", "&#x92;", "&#8217;", "Right single-quote"}
                , new[] {"\"", "&ldquo;", "&#147;", "&#x93;", "&#8220;", "Left double-quote"}
                , new[] {"\"", "&rdquo;", "&#148;", "&#x94;", "&#8221;", "Right double-quote"}
                , new[] {"-", "&ndash;", "&#150;", "&#x96;", "&#8211;", "En dash"}
                , new[] {"-", "&mdash;", "&#151;", "&#x97;", "&#8212;", "Em dash"}
                , new[] {"*", "&bull;", "&#149;", "&#x95;", "Bullet"}
                , new[] {"~", "&tilde", "&#152;", "&#x98;", "Tilde"}
                , new[] {"À", "&Agrave;", "&#192;", "&#xC0;", "Capital A-grave"}
                , new[] {"à", "&agrave;", "&#224;", "&#xE0;", "Lowercase a-grave"}
                , new[] {"Â", "&Acirc;", "&#194;", "&#xC2;", "Capital A-circumflex"}
                , new[] {"â", "&acirc;", "&#226;", "&#xE2;", "Lowercase a-circumflex"}
                , new[] {"Æ", "&AElig;", "&#198;", "&#xC6;", "Capital AE Ligature"}
                , new[] {"æ", "&aelig;", "&#230;", "&#xE6;", "Lowercase AE Ligature"}
                , new[] {"Ç", "&Ccedil;", "&#199;", "&#xC7;", "Capital C-cedilla"}
                , new[] {"ç", "&ccedil;", "&#231;", "&#xE7;", "Lowercase c-cedilla"}
                , new[] {"È", "&Egrave;", "&#200;", "&#xC8;", "Capital E-grave"}
                , new[] {"è", "&egrave;", "&#232;", "&#xE8;", "Lowercase e-grave"}
                , new[] {"É", "&Eacute;", "&#201;", "&#xC9;", "Capital E-acute"}
                , new[] {"é", "&eacute;", "&#233;", "&#xE9;", "Lowercase e-acute"}
                , new[] {"Ê", "&Ecirc;", "&#202;", "&#xCA;", "Capital E-circumflex"}
                , new[] {"ê", "&ecirc;", "&#234;", "&#xEA;", "Lowercase e-circumflex"}
                , new[] {"Ë", "&Euml;", "&#203;", "&#xCB;", "Capital E-umlaut"}
                , new[] {"ë", "&euml;", "&#235;", "&#xEB;", "Lowercase e-umlaut"}
                , new[] {"Î", "&Icirc;", "&#206;", "&#xCE;", "Capital I-circumflex"}
                , new[] {"î", "&icirc;", "&#238;", "&#xEE;", "Lowercase i-circumflex"}
                , new[] {"Ï", "&Iuml;", "&#207;", "&#xCF;", "Capital I-umlaut"}
                , new[] {"ï", "&iuml;", "&#239;", "&#xEF;", "Lowercase i-umlaut"}
                , new[] {"Ô", "&Ocirc;", "&#212;", "&#xD4;", "Capital O-circumflex"}
                , new[] {"ô", "&ocirc;", "&#244;", "&#xF4;", "Lowercase o-circumflex"}
                , new[] {"Œ", "&OElig;", "&#140;", "&#x152;", "Capital OE ligature"}
                , new[] {"œ", "&oelig;", "&#156;", "&#x153;", "Lowercase oe ligature"}
                , new[] {"Ù", "&Ugrave;", "&#217;", "&#xD9;", "Capital U-grave"}
                , new[] {"ù", "&ugrave;", "&#249;", "&#xF9;", "Lowercase u-grave"}
                , new[] {"Û", "&Ucirc;", "&#219;", "&#xDB;", "Capital U-circumflex"}
                , new[] {"û", "&ucirc;", "&#251;", "&#xFB;", "Lowercase U-circumflex"}
                , new[] {"Ü", "&Uuml;", "&#220;", "&#xDC;", "Capital U-umlaut"}
                , new[] {"ü", "&uuml;", "&#252;", "&#xFC;", "Lowercase U-umlaut"}
                , new[] {"«", "&laquo;", "&#171;", "&#xAB;", "Left angle quotes"}
                , new[] {"»", "&raquo;", "&#187;", "&#xBB;", "Right angle quotes"}
                , new[] {"€", "&euro;", "&#128;", "&#x80;", "Euro"}
            };

            string output = input;

            foreach (var special in specials)
            {
                for (int i = 1; i < special.Length - 1; i++)
                    output = output.Replace(special[i], special[0]);
            }

            return output;
        }

        public static string RemoveWordSpecialCharacters(string input)
        {
            string[] specials =
            {
                @"v\:* {behavior:url(#default#VML);}"
                , @"o\:* {behavior:url(#default#VML);}"
                , @"w\:* {behavior:url(#default#VML);}"
                , @".shape {behavior:url(#default#VML);}"
            };

            string output = input;

            foreach (string special in specials)
                output = output.Replace(special, string.Empty);

            return output;
        }

        #endregion

        #region Encoding (MD5)

        /// <summary>
        ///     Returns an MD5 hash of the input text.
        /// </summary>
        public static string CreateHashMd5(string text, byte[] salt = null)
        {
            var hash = EncryptionHelper.ComputeHashMd5(text, salt);
            return ByteArrayToHex(hash);
        }

        /// <summary>
        ///     Returns true if the MD5 hash of the input text matches the hash
        ///     value given.
        /// </summary>
        public static bool VerifyHashMd5(string text, string hash, byte[] salt = null)
        {
            var textHash = CreateHashMd5(text, salt);

            return string.Equals(hash, textHash, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        ///     Returns true if the MD5 hash of the input text matches the hash
        ///     value given.
        /// </summary>
        public static bool VerifyHashMd5(string text, byte[] hash, byte[] salt = null)
        {
            var textHash = EncryptionHelper.ComputeHashMd5(text, salt);
            return textHash.SequenceEqual(hash);
        }

        #endregion

        #region Encoding (SHA256)

        /// <summary>
        ///     Returns an SHA256 hash of the input text.
        /// </summary>
        public static string CreateHashSha256(string text, byte[] salt = null)
        {
            var hash = EncryptionHelper.ComputeHashSha256(text, salt);
            return ByteArrayToHex(hash);
        }

        /// <summary>
        ///     Returns true if the SHA256 hash of the input text matches the hash
        ///     value given.
        /// </summary>
        public static bool VerifyHashSha256(string text, string hash, byte[] salt = null)
        {
            var textHash = CreateHashSha256(text, salt);

            return string.Equals(hash, textHash, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        ///     Returns true if the SHA256 hash of the input text matches the hash
        ///     value given.
        /// </summary>
        public static bool VerifyHashSha256(string text, byte[] hash, byte[] salt = null)
        {
            var textHash = EncryptionHelper.ComputeHashSha256(text, salt);
            return textHash.SequenceEqual(hash);
        }

        #endregion

        #region Padding

        /// <summary>
        ///     Left pads the input text using the passed pad.
        /// </summary>
        public static string PadLeft(string text, string pad, int totalWidth)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            if (text.Length >= totalWidth)
                return text;

            var padded = new StringBuilder();

            while (padded.Length < totalWidth - text.Length)
            {
                padded.Append(pad);
            }

            padded.Append(text);

            return padded.ToString();
        }

        /// <summary>
        ///     Right pads the input text using the passed pad.
        /// </summary>
        public static string PadRight(string text, string pad, int totalWidth)
        {
            if (text == null)
                text = string.Empty;

            if (text.Length >= totalWidth)
                return text;

            var padded = new StringBuilder();
            padded.Append(text);

            while (padded.Length < totalWidth)
            {
                padded.Append(pad);
            }

            return padded.ToString();
        }

        #endregion

        #region Removing

        /// <summary>
        ///     Removes the matches for each of the patterns from the input text.
        ///     This function is not case-sensitive.
        /// </summary>
        public static string Remove(string text, string[] patterns)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            if (patterns == null)
                throw new ArgumentNullException(nameof(patterns));

            foreach (string pattern in patterns)
            {
                text = Regex.Replace(text, pattern, string.Empty, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }

            return text;
        }

        /// <summary>
        ///     Removes newline characters from the input text.
        /// </summary>
        public static string RemoveNewLines(string text)
        {
            return RemoveNewLines(text, false);
        }

        /// <summary>
        ///     Removes newline characters from the input text, substituting a
        ///     blank space for each newline.
        /// </summary>
        public static string RemoveNewLines(string text, bool addSpace)
        {
            if (text.IsEmpty())
                return text;

            var result = new StringBuilder();

            for (var i = 0; i < text.Length; i++)
            {
                var ch = text[i];
                if (ch == '\n')
                {
                    if (addSpace)
                        result.Append(' ');
                }
                else if (ch != '\r')
                {
                    result.Append(ch);
                }
            }

            return result.ToString();
        }

        /// <summary>
        ///     Removes all non-alphanumeric characters from the input text.
        /// </summary>
        public static string RemoveNonAlphanumericCharacters(string text)
        {
            if (text.IsEmpty())
                return text;

            var result = new StringBuilder();

            foreach (var ch in text)
                if (IsLatinLetterOrDigit(ch))
                    result.Append(ch);

            return result.ToString();
        }

        /// <summary>
        ///     Replace all non-alphanumeric characters from the input text with specified symbol
        /// </summary>
        public static string Sanitize(string value, char separator = '-', bool makeLowercase = true, char[] extraChars = null)
        {
            var result = new StringBuilder();

            if (!string.IsNullOrEmpty(value))
            {
                var isPrevSeparator = false;

                Func<char, bool> isValid;

                if (extraChars != null)
                {
                    var ext = new HashSet<char>(extraChars);

                    isValid = x => IsLatinLetterOrDigit(x) || ext.Contains(x);
                }
                else
                {
                    isValid = IsLatinLetterOrDigit;
                }

                if (makeLowercase)
                {
                    for (var i = 0; i < value.Length; i++)
                        Append(char.ToLower(value[i], CurrentCulture));
                }
                else
                {
                    for (var i = 0; i < value.Length; i++)
                        Append(value[i]);
                }

                if (result.Length > 0 && result[result.Length - 1] == separator)
                    result.Remove(result.Length - 1, 1);

                void Append(char ch)
                {
                    if (isValid(ch))
                    {
                        result.Append(ch);
                        isPrevSeparator = false;
                    }
                    else if (result.Length > 0 && !isPrevSeparator)
                    {
                        result.Append(separator);
                        isPrevSeparator = true;
                    }
                }
            }

            return result.ToString();
        }

        private static bool IsDigit(char ch) => '0' <= ch && ch <= '9';

        private static bool IsUpperLatinLetter(char ch) => 'A' <= ch && ch <= 'Z';

        private static bool IsLowerLatinLetter(char ch) => 'a' <= ch && ch <= 'z';

        private static bool IsLatinLetterOrDigit(char ch) => IsLowerLatinLetter(ch) || IsDigit(ch) || IsUpperLatinLetter(ch);

        #endregion

        #region Replacing

        /// <summary>
        ///     Replaces every occurrence of every sub-String within the input text
        ///     with the specified replacement text.
        /// </summary>
        public static string Replace(string text, IEnumerable<string> findWhat, string replaceWith, bool removeUnderscores)
        {
            if (findWhat == null)
                throw ApplicationError.Create("UnexpectedNullParameterValue: findWhat");

            string output = text;

            foreach (string f in findWhat)
            {
                if (f.Length > 0)
                    output = Replace(output, f, replaceWith);
            }

            if (removeUnderscores)
                output = output.Replace("_", string.Empty);

            return output.Trim();
        }

        /// <summary>
        ///     Replaces every occurrence of a sub-String within the input text
        ///     with the specified replacement text. This function is not case-
        ///     sensitive.
        /// </summary>
        public static string Replace(string text, string findWhat, string replaceWith)
        {
            var re = new Regex(findWhat, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return re.Replace(text, replaceWith);
        }

        /// <summary>
        ///     Performs a very fast text search and replace.
        /// </summary>
        public static string ReplaceFast(string text, string findWhat, string replaceWith, StringComparison comparison)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            if (string.IsNullOrEmpty(findWhat))
                return text;

            int lenPattern = findWhat.Length;
            int idxPattern = -1;
            int idxLast = 0;

            var result = new StringBuilder();

            while (true)
            {
                idxPattern = text.IndexOf(findWhat, idxPattern + 1, comparison);

                if (idxPattern < 0)
                {
                    result.Append(text, idxLast, text.Length - idxLast);
                    break;
                }

                result.Append(text, idxLast, idxPattern - idxLast);
                result.Append(replaceWith);

                idxLast = idxPattern + lenPattern;
            }

            return result.ToString();
        }

        /// <summary>
        ///     Replaces the first occurrence of a substring within the input text.
        /// </summary>
        public static string ReplaceFirst(string input, string oldValue, string newValue)
        {
            var regEx = new Regex(oldValue, RegexOptions.Multiline);
            return regEx.Replace(input, newValue, 1);
        }

        /// <summary>
        ///     Replaces the last occurrence of a substring within the input text.
        ///     This function is not case-sensitive.
        /// </summary>
        public static string ReplaceLast(string input, string oldValue, string newValue)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (oldValue == null)
                throw new ArgumentNullException(nameof(oldValue));

            if (newValue == null)
                throw new ArgumentNullException(nameof(newValue));

            int index = input.LastIndexOf(oldValue, StringComparison.OrdinalIgnoreCase);
            if (index < 0)
                return input;

            var sb = new StringBuilder(input.Length - oldValue.Length + newValue.Length);
            sb.Append(input.Substring(0, index));
            sb.Append(newValue);
            sb.Append(input.Substring(index + oldValue.Length, input.Length - index - oldValue.Length));

            return sb.ToString();
        }

        /// <summary>
        ///     Removes each of the words in the input String. This function is not
        ///     case-sensitive.
        /// </summary>
        public static string ReplaceWords(string input, params string[] words)
        {
            return ReplaceWords(input, char.MinValue, words);
        }

        /// <summary>
        ///     Removes each of the words in the input String, replacing each
        ///     character of each replaced word with a character mask. This
        ///     function is not case-sensitive.
        /// </summary>
        public static string ReplaceWords(string text, char mask, params string[] words)
        {
            if (words == null)
                throw new ArgumentNullException(nameof(words));

            string stringMask = mask == char.MinValue ? string.Empty : mask.ToString(CurrentCulture);

            foreach (string s in words)
            {
                var totalMask = new StringBuilder();
                totalMask.Append(stringMask);

                var regEx = new Regex(s, RegexOptions.IgnoreCase | RegexOptions.Multiline);

                if (stringMask.Length > 0)
                {
                    for (int i = 1; i < s.Length; i++)
                        totalMask.Append(stringMask);
                }

                text = regEx.Replace(text, totalMask.ToString());
            }

            return text;
        }

        public static string Replace(this string input, string oldValue, string newValue, StringComparison comparisonType)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (input.Length == 0)
                return input;

            if (string.IsNullOrEmpty(oldValue))
                throw new ArgumentNullException(nameof(oldValue));

            var sb = new StringBuilder(input.Length);

            var hasNewValue = !string.IsNullOrEmpty(newValue);
            var foundIndex = -1;
            var startIndex = 0;

            while ((foundIndex = input.IndexOf(oldValue, startIndex, comparisonType)) != -1)
            {
                var precedingTextLength = foundIndex - startIndex;
                if (precedingTextLength > 0)
                    sb.Append(input, startIndex, precedingTextLength);

                if (hasNewValue)
                    sb.Append(newValue);

                startIndex = foundIndex + oldValue.Length;
            }

            var remainingTextLength = input.Length - startIndex;
            if (remainingTextLength > 0)
                sb.Append(input, startIndex, remainingTextLength);

            return sb.ToString();
        }

        #endregion

        #region Reversing

        /// <summary>
        ///     Reverses the input text.
        /// </summary>
        public static string Reverse(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            var reverse = new char[text.Length];
            for (int i = 0, k = text.Length - 1; i < text.Length; i++, k--)
            {
                if (char.IsSurrogate(text[k]))
                {
                    reverse[i + 1] = text[k--];
                    reverse[i++] = text[k];
                }
                else
                {
                    reverse[i] = text[k];
                }
            }
            return new string(reverse);
        }

        #endregion

        #region Splitting

        public static string[] Split(string text)
        {
            return Split(text, new[] { ',', ';', '|', '\r', '\n' });
        }

        public static string[] Split(string text, char delimiter)
        {
            return Split(text, new[] { delimiter });
        }

        public static string[] Split(string text, char[] delimiters)
        {
            if (text == null)
                return new string[0];

            var items = text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            var nonEmpties = new List<string>();

            foreach (var item in items)
            {
                var s = TrimAndClean(item);

                if (!string.IsNullOrEmpty(s))
                    nonEmpties.Add(s);
            }

            return nonEmpties.ToArray();
        }

        public static string Join(IEnumerable<string> items)
        {
            return string.Join(", ", items);
        }

        public static string Join(IEnumerable<string> items, string delimiter)
        {
            return string.Join(delimiter, items);
        }

        public static string Join(IEnumerable<string> items, string delimiter, string qualifier)
        {
            if (items == null)
                throw ApplicationError.Create("UnexpectedNullParameterValue: items");

            int i = 0;
            var sb = new StringBuilder();
            foreach (var item in items)
            {
                if (i > 0)
                    sb.Append(delimiter);

                if (!string.IsNullOrEmpty(qualifier))
                    sb.Append(qualifier);

                sb.Append(item);

                if (!string.IsNullOrEmpty(qualifier))
                    sb.Append(qualifier);

                i++;
            }
            return sb.ToString();
        }

        #endregion

        #region Trimming

        public static string Leftmost(string text, int count)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            if (count < 0)
                count = 0;

            if (text.Length > count)
                return text.Substring(0, count);

            return text;
        }

        public static string Rightmost(string text, int count)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            if (count < 0)
                count = 0;

            if (text.Length > count)
                return text.Substring(text.Length - count, count);

            return text;
        }

        public static string Trim(string text)
        {
            return text.IsEmpty()
                ? text
                : text.Trim(TrimCharacters);
        }

        public static string Clean(string text)
        {
            return text.IsEmpty()
                ? text
                : CleanRegex.Replace(text, m => CleanReplacements[m.Value]);
        }

        public static string TrimAndClean(string text)
        {
            return text.IsEmpty()
                ? text
                : CleanRegex.Replace(text.Trim(TrimCharacters), m => CleanReplacements[m.Value]);
        }

        public static string TrimInside(string text)
        {
            return TrimInsideRegex.Replace(text, " ");
        }

        #endregion

        #region Wrapping

        /// <summary>
        ///     Wraps the passed String at the at the next whitespace on or after
        ///     the total character count has been reached for that line. Uses the
        ///     environment's new line symbol for the break text.
        /// </summary>
        public static string WordWrap(string text, int characterCountPerLine)
        {
            return WordWrap(text, characterCountPerLine, false, System.Environment.NewLine);
        }

        /// <summary>
        ///     Wraps the passed String after the the total character count has
        ///     been reached (if cutoff is true) or at the next whitespace (if
        ///     cutoff is false). Uses the environment's new line symbol for the
        ///     break text.
        /// </summary>
        public static string WordWrap(string text, int characterCountPerLine, bool cutoff)
        {
            return WordWrap(text, characterCountPerLine, cutoff, System.Environment.NewLine);
        }

        /// <summary>
        ///     Wraps the passed String after the the total character count has
        ///     been reached (if cutoff is true) or at the next whitespace (if
        ///     cutoff is false). Uses the specified break-text for line-breaks;
        /// </summary>
        public static string WordWrap(string text, int characterCountPerLine, bool cutoff, string breakText)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            var sb = new StringBuilder(text.Length + 100);
            int counter = 0;

            if (cutoff)
            {
                while (counter < text.Length)
                {
                    if (text.Length > counter + characterCountPerLine)
                    {
                        sb.Append(text.Substring(counter, characterCountPerLine));
                        sb.Append(breakText);
                    }
                    else
                    {
                        sb.Append(text.Substring(counter));
                    }
                    counter += characterCountPerLine;
                }
            }
            else
            {
                string[] strings = text.Split(' ');
                for (int i = 0; i < strings.Length; i++)
                {
                    // added one to represent the space.
                    counter += strings[i].Length + 1;
                    if (i != 0 && counter > characterCountPerLine)
                    {
                        sb.Append(breakText);
                        counter = 0;
                    }

                    sb.Append(strings[i] + ' ');
                }
            }
            // to get rid of the extra space at the end.
            return sb.ToString().TrimEnd();
        }

        #endregion

        #region Markdown

        private static Tuple<Regex, string>[] MarkdownRegexes = new Tuple<Regex, string>[]
        {
            new Tuple<Regex, string>(new Regex("($|\n)={2,}"), ""), // Headers
            new Tuple<Regex, string>(new Regex("#"), ""), // Headers
            new Tuple<Regex, string>(new Regex("~~"), ""), // Strikethrough
            new Tuple<Regex, string>(new Regex("`{3}.*\n"), ""), // Codeblocks
            new Tuple<Regex, string>(new Regex("<[^>]*>"), ""), // HtmlTags
            new Tuple<Regex, string>(new Regex("^[=\\-]{2,}\\s*$"), ""), // SetTextStyleHeaders
            new Tuple<Regex, string>(new Regex("\\[\\^.+?\\](\\: .*?$)?"), ""), // Footnotes1
            new Tuple<Regex, string>(new Regex("\\s{0,2}\\[.*?\\]: .*?$"), ""), // Footnotes2
            new Tuple<Regex, string>(new Regex("\\!\\[.*?\\][\\[\\(].*?[\\]\\)]"), ""), // Images
            new Tuple<Regex, string>(new Regex("\\[(.*?)\\][\\[\\(].*?[\\]\\)]"), "$1"), // Links
            new Tuple<Regex, string>(new Regex("\\*"), ""), // Bolds
            new Tuple<Regex, string>(new Regex("($|\n)>"), ""), // Quotas
        };

        private static Regex MarkdownRegexReturns = new Regex("\r");
        private static Regex MarkdownRegexNewLines = new Regex("\n");
        private static Regex MarkdownRegexMultipleSpaces = new Regex("[ ]{2,}");

        public static string StripMarkdown(string content, bool stripNewLines = false)
        {
            if (string.IsNullOrEmpty(content))
                return content;

            foreach (var regex in MarkdownRegexes)
                content = regex.Item1.Replace(content, regex.Item2);

            if (stripNewLines)
            {
                content = MarkdownRegexReturns.Replace(content, "");
                content = MarkdownRegexNewLines.Replace(content, " ");
                content = MarkdownRegexMultipleSpaces.Replace(content, " ");
            }

            return content;
        }

        #endregion

        #region Other

        public static string JoinFormat<T>(string format, IEnumerable<T> data) =>
            JoinFormat(format, string.Empty, data);

        public static string JoinFormat<T>(string format, string separator, IEnumerable<T> data)
        {
            if (data == null)
                return string.Empty;

            var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);

            for (var i = 0; i < props.Length; i++)
                format = format.Replace("{" + props[i].Name + "}", "{" + i + "}");

            var html = new StringBuilder();
            var values = new object[props.Length];

            foreach (var item in data)
            {
                for (var i = 0; i < props.Length; i++)
                    values[i] = props[i].GetValue(item);

                if (html.Length > 0)
                    html.Append(separator);

                html.AppendFormat(format, values);
            }

            return html.ToString();
        }

        public static string FirstValue(params string[] values)
        {
            foreach (var value in values)
                if (!string.IsNullOrEmpty(value))
                    return value;

            return null;
        }

        public static string FirstValue(params Func<string>[] getters)
        {
            foreach (var get in getters)
            {
                var value = get();
                if (!string.IsNullOrEmpty(value))
                    return value;
            }

            return null;
        }

        public static string ToIdentifier(string value)
        {
            if (value == null)
                return null;

            var identifier = Regex.Replace(value.Trim(), @"[^a-zA-Z0-9]", "_");
            identifier = Regex.Replace(identifier, @"\s+", " ");
            return identifier == string.Empty ? null : identifier;
        }

        public static string TruncateString(string str, int maxLength, bool appendWithEllipsis = false)
        {
            if (string.IsNullOrEmpty(str) || str.Length <= maxLength)
                return str;

            var lastWordIndex = -1;
            var separatorLength = appendWithEllipsis ? 4 : 1;
            var m = WordsParseRegex.Match(str);

            while (m.Success)
            {
                if (m.Index + separatorLength + m.Length > maxLength)
                    break;

                lastWordIndex = m.Index + m.Length;

                m = m.NextMatch();
            }

            if (lastWordIndex == -1)
            {
                lastWordIndex = maxLength;

                if (appendWithEllipsis)
                    lastWordIndex -= 3;
            }

            str = str.Substring(0, lastWordIndex);

            if (appendWithEllipsis)
                str = AppendWithEllipsis(str);

            return str;
        }

        public static string Acronym(string text)
        {
            var sb = new StringBuilder();
            var words = Split(text, new[] { ' ', '-' });
            foreach (var word in words)
                sb.Append(word[0]);
            return sb.ToString().ToUpper();
        }

        public static string AppendWithEllipsis(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            var lastChar = str[str.Length - 1];
            if (lastChar == '.' || lastChar == '!' || lastChar == '?')
                str += "..";
            else
                str += "...";

            return str;
        }

        /// <summary>
        /// Given a person's first and last name, we'll make our best guess to extract up to two initials, hopefully
        /// representing their first and last name, skipping any middle initials, Jr/Sr/III suffixes, etc. The letters 
        /// will be returned together in ALL CAPS, e.g. "TW". 
        /// 
        /// The way it parses names for many common styles:
        /// 
        /// Mason Zhwiti                -> MZ
        /// mason lowercase zhwiti      -> MZ
        /// Mason G Zhwiti              -> MZ
        /// Mason G. Zhwiti             -> MZ
        /// John Queue Public           -> JP
        /// John Q. Public, Jr.         -> JP
        /// John Q Public Jr.           -> JP
        /// Thurston Howell III         -> TH
        /// Thurston Howell, III        -> TH
        /// Malcolm X                   -> MX
        /// A Ron                       -> AR
        /// A A Ron                     -> AR
        /// Madonna                     -> M
        /// Chris O'Donnell             -> CO
        /// Malcolm McDowell            -> MM
        /// Robert "Rocky" Balboa, Sr.  -> RB
        /// 1Bobby 2Tables              -> BT
        /// Éric Ígor                   -> ÉÍ
        /// 행운의 복숭아                -> 행복
        /// 
        /// <a href="https://stackoverflow.com/a/28373431">Source</a>.
        /// </summary>
        /// <param name="name">The full name of a person.</param>
        /// <returns>One to two uppercase initials, without punctuation.</returns>
        public static string ExtractInitialsFromName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            // first remove all: punctuation, separator chars, control chars, and numbers (unicode style regexes)
            var initials = Regex.Replace(name, @"[\p{P}\p{S}\p{C}\p{N}]+", "");

            // Replacing all possible whitespace/separator characters (unicode style), with a single, regular ascii space.
            initials = Regex.Replace(initials, @"\p{Z}+", " ");

            // Remove all Sr, Jr, I, II, III, IV, V, VI, VII, VIII, IX at the end of names
            initials = Regex.Replace(initials.Trim(), @"\s+(?:[JS]R|I{1,3}|I[VX]|VI{0,3})$", "", RegexOptions.IgnoreCase);

            // Extract up to 2 initials from the remaining cleaned name.
            initials = Regex.Replace(initials, @"^(\p{L})[^\s]*(?:\s+(?:\p{L}+\s+(?=\p{L}))?(?:(\p{L})\p{L}*)?)?$", "$1$2").Trim();

            if (initials.Length > 2)
            {
                // Worst case scenario, everything failed, just grab the first two letters of what we have left.
                initials = initials.Substring(0, 2);
            }

            return initials.ToUpperInvariant();
        }

        #endregion
    }
}