using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IUserConnectionCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        bool? IsLeader { get; set; }
        bool? IsManager { get; set; }
        bool? IsSupervisor { get; set; }
        bool? IsValidator { get; set; }

        DateTimeOffset? Connected { get; set; }
    }
}