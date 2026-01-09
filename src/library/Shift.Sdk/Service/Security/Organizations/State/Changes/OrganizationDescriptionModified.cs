using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationDescriptionModified : Change
    {
        public CompanyDescription Description { get; set; }

        public OrganizationDescriptionModified(CompanyDescription description)
        {
            Description = description;
        }
    }
}
