using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Users.Write
{
    public class DisconnectUser : Command
    {
        public Guid ToUserId { get; set; }

        public DisconnectUser(Guid fromUserId, Guid toUserId)
        {
            AggregateIdentifier = fromUserId;
            ToUserId = toUserId;
        }
    }
}
