using System;

namespace Shift.Contract
{
    public class CreateUserConnection
    {
        public Guid FromUserIdentifier { get; set; }
        public Guid ToUserIdentifier { get; set; }

        public bool IsLeader { get; set; }
        public bool IsManager { get; set; }
        public bool IsSupervisor { get; set; }
        public bool IsValidator { get; set; }

        public DateTimeOffset Connected { get; set; }
    }
}