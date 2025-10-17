namespace Shift.Toolbox.Reporting.AssessmentAudit.Models
{
    public class RoleScore
    {
        public string Role { get; set; }
        public decimal? Score { get; set; }
        public decimal? MaxScore { get; set; }

        public decimal? ScaledScore => Score.HasValue && MaxScore.HasValue && MaxScore.Value != 0
            ? Score.Value / MaxScore.Value
            : (decimal?)null;
    }
}
