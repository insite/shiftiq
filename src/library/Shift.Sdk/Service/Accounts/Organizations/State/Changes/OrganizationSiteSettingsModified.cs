using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationSiteSettingsModified : Change
    {
        public SiteSettings Sites { get; set; }

        public OrganizationSiteSettingsModified(SiteSettings sites)
        {
            Sites = sites;
        }
    }
}
