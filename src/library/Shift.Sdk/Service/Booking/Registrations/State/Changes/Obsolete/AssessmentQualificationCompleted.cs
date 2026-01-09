
using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class AssessmentQualificationCompleted : Change
    {
        public string[] Errors { get; set; }
        public string[] Warnings { get; set; }

        public AssessmentQualificationCompleted(string[] errors, string[] warnings)
        {
            Errors = errors;
            Warnings = warnings;
        }
    }
}
