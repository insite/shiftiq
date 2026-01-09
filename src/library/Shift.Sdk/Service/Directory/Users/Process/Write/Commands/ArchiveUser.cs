using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Users.Write
{
    public class ArchiveUser : Command
    {
        public DateTimeOffset Date { get; set; }

        public ArchiveUser(Guid userId, DateTimeOffset date)
        {
            AggregateIdentifier = userId;
            Date = date;
        }
    }
}
