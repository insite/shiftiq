using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Responses.Write
{
    public class ConfirmResponseSession : Command
    {
        public ConfirmResponseSession(Guid session)
        {
            AggregateIdentifier = session;
        }
    }
}