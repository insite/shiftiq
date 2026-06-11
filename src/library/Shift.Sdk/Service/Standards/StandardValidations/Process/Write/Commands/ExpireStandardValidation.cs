using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.StandardValidations.Write
{
    public class ExpireStandardValidation : Command
    {
        public Guid LogId { get; set; }
        public string Comment { get; private set; }

        public ExpireStandardValidation(Guid standardValidationId, Guid logId, string comment)
        {
            AggregateIdentifier = standardValidationId;
            LogId = logId;
            Comment = comment;
        }
    }
}
