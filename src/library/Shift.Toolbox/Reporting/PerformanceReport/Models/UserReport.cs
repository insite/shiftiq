using System;

namespace Shift.Toolbox.Reporting.PerformanceReport.Models
{
    public enum UserReportType { Report1, Report2, Report3, Report4, Report5, Report6 }

    public class UserReport
    {
        public UserReportType ReportType { get; set; }
        public string FullName { get; set; }
        public string PersonCode { get; set; }
        public DateTime ReportIssued { get; set; }
        public Area[] Areas { get; set; }
        public AreaScoreResult AreaScores { get; set; }
        public string NursingRoleText { get; set; }
        public string Description { get; set; }
    }
}
