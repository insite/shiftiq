using System;

namespace Shift.Toolbox.Reporting.PerformanceReport.Models
{
    public class UserScore
    {
        public Guid AreaId { get; set; }
        public string AssessmentType { get; set; }
        public string[] Roles { get; set; }
        public decimal MaxScore { get; set; }
        public decimal Score { get; set; }
        public DateTime Graded { get; set; }

        public override string ToString()
        {
            return $"AreaId={AreaId}; Type={AssessmentType}; Score={Score}";
        }
    }
}
