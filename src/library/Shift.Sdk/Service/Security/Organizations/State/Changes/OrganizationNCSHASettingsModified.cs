using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationNCSHASettingsModified : Change
    {
        public NCSHASettings Settings { get; set; }

        public OrganizationNCSHASettingsModified(NCSHASettings settings)
        {
            Settings = settings;
        }
    }
}
