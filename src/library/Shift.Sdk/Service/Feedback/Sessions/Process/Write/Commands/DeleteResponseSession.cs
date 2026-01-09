using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Responses.Write
{
    public class DeleteResponseSession : Command
    {
        public DeleteResponseSession(Guid session)
        {
            AggregateIdentifier = session;
        }

    }
}