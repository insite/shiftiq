using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class GroupDeleted : Change
    {
        public string Reason { get; }

        public GroupDeleted(string reason)
        {
            Reason = reason;
        }
    }
}
