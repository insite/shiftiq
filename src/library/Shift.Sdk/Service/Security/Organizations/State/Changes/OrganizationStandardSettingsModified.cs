using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationStandardSettingsModified : Change
    {
        public StandardSettings Standards { get; set; }

        public OrganizationStandardSettingsModified(StandardSettings standards)
        {
            Standards = standards;
        }
    }
}
