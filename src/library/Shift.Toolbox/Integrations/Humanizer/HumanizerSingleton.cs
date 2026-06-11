using System;

using Humanizer;

using Shift.Common;

namespace Shift.Toolbox
{
    public class HumanizerSingleton : IHumanizer
    {
        public string Humanize(DateTimeOffset? when)
        {
            return when.Humanize();
        }

        public string LowerCase(string noun)
        {
            return noun.ToLower();
        }

        public string Pluralize(string noun)
        {
            return noun.Pluralize();
        }

        public string SentenceCase(string sentence)
        {
            return sentence.Humanize(LetterCasing.Sentence);
        }

        public string TitleCase(string sentence)
        {
            if (sentence == null)
                return null;
            return sentence.Humanize(LetterCasing.Title);
        }

        public string ToQuantity(int quantity, string noun)
        {
            return noun.ToQuantity(quantity, "n0");
        }

        public string ToQuantity(int quantity, string format, string noun)
        {
            return noun.ToQuantity(quantity, format);
        }
    }
}