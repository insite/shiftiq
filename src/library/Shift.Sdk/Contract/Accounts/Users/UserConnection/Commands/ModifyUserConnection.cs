using System;

namespace Shift.Contract
{
    public class ModifyUserConnection
    {
        public Guid FromUserId { get; set; }
        public Guid ToUserId { get; set; }

        public bool IsLeader { get; set; }
        public bool IsManager { get; set; }
        public bool IsSupervisor { get; set; }
        public bool IsValidator { get; set; }

        public DateTimeOffset Connected { get; set; }
    }
}