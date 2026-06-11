using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationPortalSettingsModified : Change
    {
        public PortalSettings Portal { get; set; }

        public OrganizationPortalSettingsModified(PortalSettings portal)
        {
            Portal = portal;
        }
    }
}
