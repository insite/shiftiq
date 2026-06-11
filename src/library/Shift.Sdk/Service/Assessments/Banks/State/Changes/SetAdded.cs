using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class SetAdded : Change
    {
        public Guid Set { get; set; }
        public string Name { get; set; }
        public Guid Standard { get; set; }

        public SetAdded(Guid set, string name, Guid standard)
        {
            Set = set;
            Name = name;
            Standard = standard;
        }
    }
}
