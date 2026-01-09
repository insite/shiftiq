using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationPlatformUrlModified : Change
    {
        public PlatformUrl Url { get; set; }

        public OrganizationPlatformUrlModified(PlatformUrl url)
        {
            Url = url;
        }
    }
}
