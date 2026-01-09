using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.People.Write
{
    public class ArchivePerson : Command
    {
        public DateTimeOffset Date { get; set; }

        public ArchivePerson(Guid personId, DateTimeOffset date)
        {
            AggregateIdentifier = personId;
            Date = date;
        }
    }
}
