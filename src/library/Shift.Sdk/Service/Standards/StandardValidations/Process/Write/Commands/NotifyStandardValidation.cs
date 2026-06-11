using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.StandardValidations.Write
{
    public class NotifyStandardValidation : Command
    {
        public DateTimeOffset? Date { get; set; }

        public NotifyStandardValidation(Guid standardValidationId, DateTimeOffset? date)
        {
            AggregateIdentifier = standardValidationId;
            Date = date;
        }
    }
}
