using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationStandardContentLabelsModified : Change
    {
        public string[] Labels { get; set; }

        public OrganizationStandardContentLabelsModified(string[] labels)
        {
            Labels = labels;
        }
    }
}
