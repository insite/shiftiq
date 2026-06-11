using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface ISubmissionCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        DateTimeOffset? LastChangeTimeSince { get; set; }
        DateTimeOffset? LastChangeTimeBefore { get; set; }

        DateTimeOffset? ResponseSessionCompletedSince { get; set; }
        DateTimeOffset? ResponseSessionCompletedBefore { get; set; }

        DateTimeOffset? ResponseSessionCreatedSince { get; set; }
        DateTimeOffset? ResponseSessionCreatedBefore { get; set; }

        DateTimeOffset? ResponseSessionStartedSince { get; set; }
        DateTimeOffset? ResponseSessionStartedBefore { get; set; }
    }
}
