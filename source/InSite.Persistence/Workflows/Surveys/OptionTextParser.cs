using System.Text.RegularExpressions;

namespace InSite.Persistence
{
    public class OptionTextParser
    {
        #region Fields

        private static readonly Regex _pattern1 = new Regex(
            "^(?<Text>.+)(?:\\s+)\\>\\>(?:\\s+)(?<True>.+)$", 
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex _pattern2 = new Regex(
            "^(?<Text>.+)(?:\\s+)T\\>\\>(?:(?:\\s*)(?<True>.+))?(?:\\s+)F\\>\\>(?:(?:\\s*)(?<False>.+))?$", 
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        #endregion

        #region Construction

        public OptionTextParser(string text)
        {
            Text = text;

            if (string.IsNullOrEmpty(text))
                return;

            var match = _pattern1.Match(text);
            if (!match.Success)
                match = _pattern2.Match(text);

            if (!match.Success)
                return;

            Text = match.Groups["Text"].Value;
            ExplanationWhenSelected = match.Groups["True"].Value;
            ExplanationWhenUnselected = match.Groups["False"].Value;
        }

        #endregion

        #region Properties

        public string Text { get; private set; }
        public string ExplanationWhenSelected { get; private set; }
        public string ExplanationWhenUnselected { get; private set; }

        #endregion
    }
}