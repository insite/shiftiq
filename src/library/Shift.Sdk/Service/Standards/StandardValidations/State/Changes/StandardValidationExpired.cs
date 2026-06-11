using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Standards
{
    public class StandardValidationExpired : Change
    {
        public Guid LogId { get; }
        public string Comment { get; }

        public StandardValidationExpired(Guid logId, string comment)
        {
            LogId = logId;
            Comment = comment;
        }
    }
}
