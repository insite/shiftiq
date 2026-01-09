using System;

namespace Shift.Common
{
    /// <summary>
    /// Insulates our code from direct dependencies on the Humanizer assemblies.
    /// </summary>
    public interface IHumanizer
    {
        string Humanize(DateTimeOffset? when);
        string LowerCase(string sentence);
        string Pluralize(string noun);
        string SentenceCase(string sentence);
        string TitleCase(string sentence);
        string ToQuantity(int quantity, string noun);
        string ToQuantity(int quantity, string format, string noun);
    }

    public static class Humanizer
    {
        private static IHumanizer _humanizer;

        public static void Initialize(IHumanizer humanizer)
        {
            _humanizer = humanizer;
        }

        public static string LowerCase(string sentence)
        {
            return _humanizer.LowerCase(sentence);
        }

        public static string SentenceCase(string sentence)
        {
            return _humanizer.SentenceCase(sentence);
        }

        public static string TitleCase(string sentence)
        {
            return _humanizer.TitleCase(sentence);
        }

        public static string ToQuantity(int quantity, string noun)
        {
            return _humanizer.ToQuantity(quantity, noun);
        }

        public static string ToQuantity(int quantity, string format, string noun)
        {
            return _humanizer.ToQuantity(quantity, format, noun);
        }

        public static string Pluralize(string display)
        {
            return _humanizer.Pluralize(display);
        }

        public static string Humanize(DateTimeOffset? when)
        {
            return _humanizer.Humanize(when);
        }
    }
}
