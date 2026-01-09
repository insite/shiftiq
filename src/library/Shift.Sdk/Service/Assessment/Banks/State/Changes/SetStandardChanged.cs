using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class SetStandardChanged : Change
    {
        public Guid Set { get; set; }
        public Guid Standard { get; set; }

        public SetStandardChanged(Guid set, Guid standard)
        {
            Set = set;
            Standard = standard;
        }
    }
}
