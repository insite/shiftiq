using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.People.Write
{
    public class UnarchivePerson : Command
    {
        public DateTimeOffset Date { get; set; }

        public UnarchivePerson(Guid personId, DateTimeOffset date)
        {
            AggregateIdentifier = personId;
            Date = date;
        }
    }
}
