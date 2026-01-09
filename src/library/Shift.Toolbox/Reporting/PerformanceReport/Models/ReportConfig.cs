namespace Shift.Toolbox.Reporting.PerformanceReport.Models
{
    public class ReportConfig
    {
        public string Language { get; set; }
        public string FileSuffix { get; set; }
        public decimal EmergentScore { get; set; }
        public decimal ConsistentScore { get; set; }
        public string RequiredRole { get; set; }
        public ItemWeight[] RoleWeights { get; set; }
        public ItemWeight[] AssessmentTypeWeights { get; set; }
        public string NursingRoleText { get; set; }
        public string Description { get; set; }
    }
}
