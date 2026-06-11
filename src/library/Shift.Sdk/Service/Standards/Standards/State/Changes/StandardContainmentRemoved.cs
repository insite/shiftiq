using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Standards
{
    public class StandardContainmentRemoved : Change
    {
        public Guid[] ChildStandardIds { get; }

        public StandardContainmentRemoved(Guid[] childStandardIds)
        {
            ChildStandardIds = childStandardIds;
        }
    }
}
