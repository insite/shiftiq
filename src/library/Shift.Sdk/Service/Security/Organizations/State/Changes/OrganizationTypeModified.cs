using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationTypeModified : Change
    {
        public string Type { get; set; }

        public OrganizationTypeModified(string type)
        {
            Type = type;
        }
    }
}
