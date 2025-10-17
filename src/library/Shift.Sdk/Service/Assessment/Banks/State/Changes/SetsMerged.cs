using System;

using Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class SetsMerged : Change
    {
        public Guid Set { get; set; }

        public SetsMerged(Guid set)
        {
            Set = set;
        }
    }
}
