using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Responses.Write
{
    public class LockResponseSession : Command
    {
        public LockResponseSession(Guid session)
        {
            AggregateIdentifier = session;
        }
    }

    public class ChangeResponseGroup : Command
    {
        public Guid? Group { get; set; }

        public ChangeResponseGroup(Guid response, Guid? group)
        {
            AggregateIdentifier = response;
            Group = group;
        }
    }

    public class ChangeResponsePeriod : Command
    {
        public Guid? Period { get; set; }

        public ChangeResponsePeriod(Guid response, Guid? period)
        {
            AggregateIdentifier = response;
            Period = period;
        }
    }
}