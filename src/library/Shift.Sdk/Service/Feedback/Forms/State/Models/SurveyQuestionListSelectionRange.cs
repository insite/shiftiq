namespace InSite.Domain.Surveys.Forms
{
    public class SurveyQuestionListSelectionRange
    {
        public bool Enabled { get; set; }
        public int? Min { get; set; }
        public int? Max { get; set; }

        internal bool IsEqual(SurveyQuestionListSelectionRange other)
        {
            return this.Enabled == other.Enabled
                && this.Min == other.Min
                && this.Max == other.Max;
        }

        internal void Set(SurveyQuestionListSelectionRange other)
        {
            if (other != null)
            {
                Enabled = other.Enabled;
                Min = other.Min;
                Max = other.Max;
            }
        }
    }
}
