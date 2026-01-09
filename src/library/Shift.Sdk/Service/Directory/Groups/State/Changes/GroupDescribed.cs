using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class GroupDescribed : Change
    {
        public string Category { get; }
        public string Code { get; }
        public string Description { get; }
        public string Label { get; }

        public GroupDescribed(string category, string code, string description, string label)
        {
            Category = category;
            Code = code;
            Description = description;
            Label = label;
        }
    }
}
