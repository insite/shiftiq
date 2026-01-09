using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountUserConnections : Query<int>, IUserConnectionCriteria
    {
        public bool? IsLeader { get; set; }
        public bool? IsManager { get; set; }
        public bool? IsSupervisor { get; set; }
        public bool? IsValidator { get; set; }

        public DateTimeOffset? Connected { get; set; }
    }
}