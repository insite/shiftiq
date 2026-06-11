using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IUserSessionCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        DateTimeOffset? SessionStartedSince { get; set; }
        DateTimeOffset? SessionStartedBefore { get; set; }
        DateTimeOffset? SessionStoppedSince { get; set; }
        DateTimeOffset? SessionStoppedBefore { get; set; }
    }
}
