using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class UserConnected : Change
    {
        public Guid ToUserId { get; set; }

        public bool IsLeader { get; set; }
        public bool IsManager { get; set; }
        public bool IsSupervisor { get; set; }
        public bool IsValidator { get; set; }

        public DateTimeOffset Connected { get; set; }

        public UserConnected(Guid toUserId, bool isLeader, bool isManager, bool isSupervisor, bool isValidator, DateTimeOffset connected)
        {
            ToUserId = toUserId;

            IsLeader = isLeader;
            IsManager = isManager;
            IsSupervisor = isSupervisor;
            IsValidator = isValidator;

            Connected = connected;
        }
    }
}
