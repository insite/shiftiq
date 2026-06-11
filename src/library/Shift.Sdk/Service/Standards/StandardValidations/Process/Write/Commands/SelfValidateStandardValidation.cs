using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.StandardValidations.Write
{
    public class SelfValidateStandardValidation : Command
    {
        public Guid LogId { get; set; }
        public string Status { get; set; }

        public SelfValidateStandardValidation(Guid standardValidationId, Guid logId, string status)
        {
            AggregateIdentifier = standardValidationId;
            LogId = logId;
            Status = status;
        }
    }
}
