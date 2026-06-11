using System;

namespace Shift.Toolbox.Reports.AssessmentAudit.Models
{
    public class AreaScore
    {
        public Guid AreaId { get; set; }
        public string RequiredRole { get; set; }
        public RoleScore[] RoleScores { get; set; }
    }
}
