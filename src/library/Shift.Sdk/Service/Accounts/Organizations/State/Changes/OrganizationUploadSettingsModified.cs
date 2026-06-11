using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationUploadSettingsModified : Change
    {
        public UploadSettings Settings { get; set; }

        public OrganizationUploadSettingsModified(UploadSettings settings)
        {
            Settings = settings;
        }
    }
}
