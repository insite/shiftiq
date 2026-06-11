using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class SetRenamed : Change
    {
        public Guid Set { get; set; }
        public string Name { get; set; }

        public SetRenamed(Guid set, string name)
        {
            Set = set;
            Name = name;
        }
    }
}
