using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Standards
{
    public class StandardValidationValidated : Change
    {
        public Guid LogId { get; }
        public bool IsValidated { get; }
        public string Status { get; }
        public string Comment { get; }


        public StandardValidationValidated(Guid logId, bool isValidated, string status, string comment)
        {
            LogId = logId;
            IsValidated = isValidated;
            Status = status;
            Comment = comment;
        }
    }
}
