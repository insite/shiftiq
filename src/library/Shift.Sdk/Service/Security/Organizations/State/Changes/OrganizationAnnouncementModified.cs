using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationAnnouncementModified : Change
    {
        public string Announcement { get; set; }

        public OrganizationAnnouncementModified(string announcement)
        {
            Announcement = announcement;
        }
    }
}
