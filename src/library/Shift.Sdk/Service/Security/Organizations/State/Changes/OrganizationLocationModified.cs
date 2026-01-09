using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationLocationModified : Change
    {
        public Location Location { get; set; }

        public OrganizationLocationModified(Location location)
        {
            Location = location;
        }
    }
}
