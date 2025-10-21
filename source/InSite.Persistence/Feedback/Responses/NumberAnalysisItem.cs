using System;

namespace InSite.Persistence
{
    [Serializable]
    public class NumberAnalysisItem
    {
        public int QuestionId { get; set; }
        public decimal Minimum { get; set; }
        public decimal Maximum { get; set; }
        public decimal Sum { get; set; }
        public decimal Average { get; set; }
        public decimal? StandardDeviation { get; set; }
        public decimal? Variance { get; set; }
        public decimal Count { get; set; }
    }
}
