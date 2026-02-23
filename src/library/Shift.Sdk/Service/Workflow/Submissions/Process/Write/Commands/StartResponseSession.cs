using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Responses.Write
{
    public class StartResponseSession : Command
    {
        public StartResponseSession(Guid session, DateTimeOffset? started, bool noStatusChange)
        {
            AggregateIdentifier = session;
            Started = started;
            NoStatusChange = noStatusChange;
        }

        public DateTimeOffset? Started { get; set; }
        public bool NoStatusChange { get; set; }
    }
}