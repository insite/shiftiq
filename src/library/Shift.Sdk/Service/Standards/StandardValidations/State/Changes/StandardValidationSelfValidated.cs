using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Standards
{
    public class StandardValidationSelfValidated : Change
    {
        public Guid LogId { get; }
        public string Status { get; }

        public StandardValidationSelfValidated(Guid logId, string status)
        {
            LogId = logId;
            Status = status;
        }
    }
}
