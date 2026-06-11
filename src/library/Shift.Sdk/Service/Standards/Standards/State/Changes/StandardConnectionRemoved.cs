using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Standards
{
    public class StandardConnectionRemoved : Change
    {
        public Guid[] ToStandardIds { get; }

        public StandardConnectionRemoved(Guid[] toStandardIds)
        {
            ToStandardIds = toStandardIds;
        }
    }
}
