using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class GroupTagAdded : Change
    {
        public string Tag { get; }

        public GroupTagAdded(string tag)
        {
            Tag = tag;
        }
    }
}
