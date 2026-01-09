using Shift.Common;

namespace InSite.Admin.Records.Reports.LearnerActivity.Models
{
    public class SummaryCounterData
    {
        public Counter[] ProgramNames { get; set; }
        public Counter[] GradebookNames { get; set; }
        public Counter[] EnrollmentStatuses { get; set; }
        public Counter[] EngagementStatuses { get; set; }
        public Counter[] LearnerGenders { get; set; }
        public Counter[] LearnerEmployers { get; set; }
        public Counter[] LearnerCitizenships { get; set; }
        public Counter[] MembershipStatuses { get; set; }
    }
}