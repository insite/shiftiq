using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Responses.Write
{
    public class ChangeResponseUser : Command
    {
        public Guid User { get; }

        public ChangeResponseUser(Guid session, Guid user)
        {
            AggregateIdentifier = session;
            User = user;
        }
    }
}
