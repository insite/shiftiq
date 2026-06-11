using System;

namespace Shift.Sdk.Service.Platform
{
    public class DashboardCounts
    {
        public int BankCount { get; set; }
        public int ActiveBankCount { get; set; }
        public int FormCount { get; set; }
        public int PublishedFormCount { get; set; }
        public int PersonCount { get; set; }
        public int ActivePersonCount { get; set; }
        public int ApprovedPersonCount { get; set; }
        public int CourseCount { get; set; }
        public int PublishedCourseCount { get; set; }
        public int StartedEnrollmentCount { get; set; }
        public int CompletedEnrollmentCount { get; set; }
    }
}