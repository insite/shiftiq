using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.StandardValidations.Write
{
    public class SubmitForValidationStandardValidation : Command
    {
        public Guid LogId { get; set; }

        public SubmitForValidationStandardValidation(Guid standardValidationId, Guid logId)
        {
            AggregateIdentifier = standardValidationId;
            LogId = logId;
        }
    }
}
