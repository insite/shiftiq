using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class GroupTagRemoved : Change
    {
        public string Tag { get; }

        public GroupTagRemoved(string tag)
        {
            Tag = tag;
        }
    }
}
