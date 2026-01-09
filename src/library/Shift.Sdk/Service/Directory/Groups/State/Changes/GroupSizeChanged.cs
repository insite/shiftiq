using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class GroupSizeChanged : Change
    {
        public string Size { get; }

        public GroupSizeChanged(string size)
        {
            Size = size;
        }
    }
}
