using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationUrlsModified : Change
    {
        public OrganizationUrl Url { get; set; }

        public OrganizationUrlsModified(OrganizationUrl url)
        {
            Url = url;
        }
    }
}