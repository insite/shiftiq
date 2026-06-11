using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationSalesSettingsModified : Change
    {
        public SalesSettings Settings { get; set; }

        public OrganizationSalesSettingsModified(SalesSettings settings)
        {
            Settings = settings;
        }
    }
}
