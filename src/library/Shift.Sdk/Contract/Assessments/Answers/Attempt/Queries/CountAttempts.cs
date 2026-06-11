using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountAttempts : Query<int>, IAttemptCriteria
    {
        public Guid? OrganizationId { get; set; }

        public string AttemptStatus { get; set; }

        public DateTimeOffset? AttemptGradedSince { get; set; }
        public DateTimeOffset? AttemptGradedBefore { get; set; }
        public DateTimeOffset? AttemptImportedSince { get; set; }
        public DateTimeOffset? AttemptImportedBefore { get; set; }
        public DateTimeOffset? AttemptPingedSince { get; set; }
        public DateTimeOffset? AttemptPingedBefore { get; set; }
        public DateTimeOffset? AttemptStartedSince { get; set; }
        public DateTimeOffset? AttemptStartedBefore { get; set; }
        public DateTimeOffset? AttemptSubmittedSince { get; set; }
        public DateTimeOffset? AttemptSubmittedBefore { get; set; }
    }
}