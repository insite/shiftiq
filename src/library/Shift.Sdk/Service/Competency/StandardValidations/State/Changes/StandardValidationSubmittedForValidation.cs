using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Standards
{
    public class StandardValidationSubmittedForValidation : Change
    {
        public Guid LogId { get; set; }

        public StandardValidationSubmittedForValidation(Guid logId)
        {
            LogId = logId;
        }
    }
}
