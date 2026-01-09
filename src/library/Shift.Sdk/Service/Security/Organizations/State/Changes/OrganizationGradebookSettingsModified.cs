using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationGradebookSettingsModified : Change
    {
        public GradebookSettings Gradebooks { get; set; }

        public OrganizationGradebookSettingsModified(GradebookSettings gradebooks)
        {
            Gradebooks = gradebooks;
        }
    }
}
