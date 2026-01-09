using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Users.Write
{
    public class ConnectUser : Command
    {
        public Guid ToUserId { get; set; }
        public bool IsLeader { get; set; }
        public bool IsManager { get; set; }
        public bool IsSupervisor { get; set; }
        public bool IsValidator { get; set; }
        public DateTimeOffset Connected { get; set; }

        public ConnectUser(Guid fromUserId, Guid toUserId,
            bool isLeader, bool isManager, bool isSupervisor, bool isValidator,
            DateTimeOffset connected)
        {
            AggregateIdentifier = fromUserId;
            ToUserId = toUserId;

            IsLeader = isLeader;
            IsManager = isManager;
            IsSupervisor = isSupervisor;
            IsValidator = isValidator;

            Connected = connected;
        }
    }
}
