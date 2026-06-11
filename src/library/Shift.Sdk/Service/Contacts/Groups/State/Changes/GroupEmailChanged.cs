using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class GroupEmailChanged : Change
    {
        public string Email { get; }

        public GroupEmailChanged(string email)
        {
            Email = email;
        }
    }
}
