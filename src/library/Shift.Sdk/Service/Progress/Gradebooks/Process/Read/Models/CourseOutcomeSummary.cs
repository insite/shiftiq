using System;

namespace InSite.Application.Records.Read
{
    public class CourseOutcomeSummary
    {
        public Guid GradebookIdentifier { get; set; }
        public string GradebookTitle { get; set; }
        public string Reference { get; set; }
        public string Term { get; set; }
        public Guid StandardIdentifier { get; set; }
        public string StandardTitle { get; set; }
        public string ParentStandardTitle { get; set; }
        public decimal AvgScore { get; set; }
        public double? StdScore { get; set; }
        public int StudentCount { get; set; }
    }
}
