using System;

using Common.Timeline.Commands;

namespace InSite.Application.Responses.Write
{
    public class StartResponseSession : Command
    {
        public StartResponseSession(Guid session, DateTimeOffset? started)
        {
            AggregateIdentifier = session;
            Started = started;
        }

        public DateTimeOffset? Started { get; set; }
    }
}