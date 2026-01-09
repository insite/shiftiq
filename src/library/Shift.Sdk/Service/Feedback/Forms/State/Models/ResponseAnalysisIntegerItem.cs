using System;

namespace InSite.Domain.Surveys.Forms
{
    public class ResponseAnalysisIntegerItem
    {
        public Guid QuestionIdentifier { get; set; }
        public double Minimum { get; set; }
        public double Maximum { get; set; }
        public double Sum { get; set; }
        public double Average { get; set; }
        public double? StandardDeviation { get; set; }
        public double Variance { get; set; }
        public int Count { get; set; }
    }
}
