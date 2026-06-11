using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Standards
{
    public class StandardGroupRemoved : Change
    {
        public Guid[] GroupIds { get; }

        public StandardGroupRemoved(Guid[] groupIds)
        {
            GroupIds = groupIds;
        }
    }
}
