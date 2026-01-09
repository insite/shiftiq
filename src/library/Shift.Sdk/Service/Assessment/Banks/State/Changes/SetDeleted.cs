using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class SetDeleted : Change
    {
        public Guid Set { get; set; }

        public SetDeleted(Guid set)
        {
            Set = set;
        }
    }
}
