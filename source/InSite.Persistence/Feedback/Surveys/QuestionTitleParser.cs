using System.Text.RegularExpressions;

namespace InSite.Persistence
{
    public class QuestionTitleParser
    {
        #region Fields

        private static readonly Regex _pattern = new Regex(
            "^(?<Title>.+)(?:\\s+)\\>\\>(?:\\s+)(?<Explanation>.+)$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        #endregion

        #region Construction

        public QuestionTitleParser(string title)
        {
            Title = title;

            if (string.IsNullOrEmpty(title))
                return;

            var match = _pattern.Match(title);
            if (!match.Success)
                return;

            Title = match.Groups["Title"].Value;
            Explanation = match.Groups["Explanation"].Value;
        }

        #endregion

        #region Properties

        public string Title { get; private set; }
        public string Explanation { get; private set; }

        #endregion
    }
}
