using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationContactSettingsModified : Change
    {
        public ContactSettings Contacts { get; set; }

        public OrganizationContactSettingsModified(ContactSettings contacts)
        {
            Contacts = contacts;
        }
    }
}
