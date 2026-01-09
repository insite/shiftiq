using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.StandardValidations.Write
{
    public class RemoveStandardValidationLog : Command
    {
        public Guid LogId { get; set; }

        public RemoveStandardValidationLog(Guid standardValidationId, Guid logId)
        {
            AggregateIdentifier = standardValidationId;
            LogId = logId;
        }
    }
}
