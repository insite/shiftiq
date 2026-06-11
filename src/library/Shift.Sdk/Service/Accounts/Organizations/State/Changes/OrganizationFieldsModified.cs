using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationFieldsModified : Change
    {
        public OrganizationFields Fields { get; set; }

        public OrganizationFieldsModified(OrganizationFields fields)
        {
            Fields = fields;
        }
    }
}
