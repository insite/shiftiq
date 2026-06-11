using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class RenumberEvent : Command
    {
        public int Number { get; set; }

        public RenumberEvent(Guid id, int number)
        {
            AggregateIdentifier = id;
            Number = number;
        }
    }
}
