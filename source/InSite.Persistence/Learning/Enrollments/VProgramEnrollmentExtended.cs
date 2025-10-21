using System.ComponentModel.DataAnnotations.Schema;

namespace InSite.Persistence
{
    [NotMapped]
    public class VProgramEnrollmentExtended : VProgramEnrollment
    {
        public string CompletionCounter { get; set; }
        public string CompletionPercent { get; set; }
        public string DaysTaken { get; set; }
        public string DeleteEnrollmentLink { get; set; }
        public string EnrollmentToolTip { get; set; }
        public string LearnerNameLink { get; set; }
    }
}
