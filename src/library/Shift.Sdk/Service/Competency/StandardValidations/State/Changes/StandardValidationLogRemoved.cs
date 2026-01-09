using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Standards
{
    public class StandardValidationLogRemoved : Change
    {
        public Guid LogId { get; }

        public StandardValidationLogRemoved(Guid logId)
        {
            LogId = logId;
        }
    }
}
