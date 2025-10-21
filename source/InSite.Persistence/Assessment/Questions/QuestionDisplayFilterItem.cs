namespace InSite.Persistence
{
    public class QuestionDisplayFilterItem
    {
        #region Properties

        public string Tag { get; private set; }
        public int Count { get; private set; }
        public int Maximum { get; private set; }
        public bool Allows => Count < Maximum;

        #endregion

        #region Methods

        public void Increment() => Count++;

        public static bool TryParse(string input, out QuestionDisplayFilterItem result)
        {
            result = null;

            if (string.IsNullOrEmpty(input))
                return false;

            var separatorIndex = input.LastIndexOf(':');
            if (separatorIndex == -1)
                return false;

            var strCount = input.Substring(separatorIndex + 1).Trim();
            if (!int.TryParse(strCount, out var count))
                return false;

            result = new QuestionDisplayFilterItem
            {
                Tag = input.Substring(0, separatorIndex).Trim(),
                Maximum = count
            };

            return true;
        }

        #endregion
    }
}
