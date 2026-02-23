using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Responses.Write
{
    public class CompleteResponseSession : Command
    {
        public DateTimeOffset? Completed { get; set; }

        public CompleteResponseSession(Guid session, DateTimeOffset? completed)
        {
            AggregateIdentifier = session;
            Completed = completed;
        }
    }
}