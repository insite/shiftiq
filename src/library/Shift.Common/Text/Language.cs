using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Shift.Common
{
    public static class Language
    {
        public const string Default = "en";

        [Serializable]
        public sealed class LanguageInfo
        {
            public string Name { get; private set; }
            public string Code { get; private set; }

            public LanguageInfo(string name, string code)
            {
                Name = name;
                Code = code;
            }

            public LanguageInfo Clone()
                => new LanguageInfo(Name, Code);
        }

        private static readonly LanguageInfo[] LanguagesSet = {
            new LanguageInfo("Arabic", "ar"),
            new LanguageInfo("Chinese", "zh"),
            new LanguageInfo("Dutch", "nl"),
            new LanguageInfo("English", "en"),
            new LanguageInfo("Esperanto", "eo"),
            new LanguageInfo("French", "fr"),
            new LanguageInfo("German", "de"),
            new LanguageInfo("Hebrew", "he"),
            new LanguageInfo("Italian", "it"),
            new LanguageInfo("Japanese", "ja"),
            new LanguageInfo("Korean", "ko"),
            new LanguageInfo("Latin", "la"),
            new LanguageInfo("Norwegian", "no"),
            new LanguageInfo("Punjabi", "pa"),
            new LanguageInfo("Polish", "pl"),
            new LanguageInfo("Portuguese", "pt"),
            new LanguageInfo("Russian", "ru"),
            new LanguageInfo("Spanish", "es"),
            new LanguageInfo("Swedish", "sv"),
            new LanguageInfo("Ukrainian", "uk"),
        };

        public static string GetDisplayName(object language)
        {
            var name = language as string;
            if (string.IsNullOrEmpty(name))
                return "Unknown";

            var info = CultureInfo.GetCultureInfo(name);
            return info.DisplayName;
        }

        public static string GetEnglishName(string language)
        {
            var ci = new CultureInfo(language);
            return ci.EnglishName;
        }

        public static LanguageInfo GetInfo(string code)
        {
            return LanguagesSet.SingleOrDefault(x => x.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        }

        public static LanguageInfo[] GetInfo(params string[] codes)
        {
            if (codes == null)
                throw new ArgumentNullException(nameof(codes));

            var hashCodes = new HashSet<string>(codes, StringComparer.OrdinalIgnoreCase);

            return LanguagesSet.Where(x => hashCodes.Contains(x.Code)).ToArray();
        }

        public static LanguageInfo[] GetAllInfo()
        {
            return LanguagesSet.ToArray();
        }

        public static string GetName(string code)
        {
            return LanguagesSet
                .Where(x => x.Code.Equals(code, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Name)
                .SingleOrDefault();
        }

        public static string GetCode(string name)
        {
            return LanguagesSet
                .Where(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Code)
                .SingleOrDefault();
        }

        public static bool CodeExists(string code)
        {
            return !string.IsNullOrEmpty(code)
                && LanguagesSet.Any(x => x.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        }

        public static bool LanguageExists(string name)
        {
            return LanguagesSet
                .Any(x => x.Name == name);
        }
    }
}
