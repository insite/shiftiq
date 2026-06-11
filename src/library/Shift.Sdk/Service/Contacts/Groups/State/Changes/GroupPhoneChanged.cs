using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class GroupPhoneChanged : Change
    {
        public string Phone { get; }
        public string Fax { get; }

        public GroupPhoneChanged(string phone, string fax)
        {
            Phone = phone;
            Fax = fax;
        }
    }
}
