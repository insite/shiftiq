using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IAttemptCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        string AttemptStatus { get; set; }

        DateTimeOffset? AttemptGradedSince { get; set; }
        DateTimeOffset? AttemptGradedBefore { get; set; }
        DateTimeOffset? AttemptImportedSince { get; set; }
        DateTimeOffset? AttemptImportedBefore { get; set; }
        DateTimeOffset? AttemptPingedSince { get; set; }
        DateTimeOffset? AttemptPingedBefore { get; set; }
        DateTimeOffset? AttemptStartedSince { get; set; }
        DateTimeOffset? AttemptStartedBefore { get; set; }
        DateTimeOffset? AttemptSubmittedSince { get; set; }
        DateTimeOffset? AttemptSubmittedBefore { get; set; }
    }
}