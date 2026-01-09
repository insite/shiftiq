using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationAssessmentSettingsModified : Change
    {
        public AssessmentSettings Assessments { get; set; }

        public OrganizationAssessmentSettingsModified(AssessmentSettings assessments)
        {
            Assessments = assessments;
        }
    }
}
