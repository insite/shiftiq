using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class ChangeEventCreditHours : Command
    {
        public decimal? CreditHours { get; set; }

        public ChangeEventCreditHours(Guid @event, decimal? creditHours)
        {
            AggregateIdentifier = @event;
            CreditHours = creditHours;
        }
    }
}
