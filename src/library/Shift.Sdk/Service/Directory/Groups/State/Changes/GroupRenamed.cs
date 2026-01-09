using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class GroupRenamed : Change
    {
        public string Type { get; }
        public string Name { get; }

        public GroupRenamed(string type, string name)
        {
            Type = type;
            Name = name;
        }
    }
}
