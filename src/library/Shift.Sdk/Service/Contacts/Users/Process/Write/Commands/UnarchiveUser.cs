using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Users.Write
{
    public class UnarchiveUser : Command
    {
        public DateTimeOffset Date { get; set; }

        public UnarchiveUser(Guid userId, DateTimeOffset date)
        {
            AggregateIdentifier = userId;
            Date = date;
        }
    }
}
