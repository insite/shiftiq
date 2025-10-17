using System;

namespace InSite.Application.Records.Read
{
    public class AcademicYearOutcome
    {
        public Guid StandardIdentifier { get; set; }
        public string StandardTitle { get; set; }
        public string ParentStandardTitle { get; set; }
        public decimal AvgScore { get; set; }
        public double? StdScore { get; set; }
        public int StudentCount { get; set; }
    }
}
