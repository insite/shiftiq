using System;

namespace InSite.Application.Attempts.Read
{
    public class TLearnerAttemptSummary
    {
        public Guid LearnerUserIdentifier { get; set; }
        public Guid? AttemptLastFailedIdentifier { get; set; }
        public Guid? AttemptLastGradedIdentifier { get; set; }
        public Guid? AttemptLastPassedIdentifier { get; set; }
        public Guid? AttemptLastStartedIdentifier { get; set; }
        public Guid? AttemptLastSubmittedIdentifier { get; set; }
        public Guid FormIdentifier { get; set; }

        public int AttemptFailedCount { get; set; }
        public int AttemptGradedCount { get; set; }
        public int AttemptImportedCount { get; set; }
        public int AttemptPassedCount { get; set; }
        public int AttemptStartedCount { get; set; }
        public int AttemptSubmittedCount { get; set; }
        public int AttemptTotalCount { get; set; }
        public int AttemptVoidedCount { get; set; }

        public DateTimeOffset? AttemptLastFailed { get; set; }
        public DateTimeOffset? AttemptLastGraded { get; set; }
        public DateTimeOffset? AttemptLastPassed { get; set; }
        public DateTimeOffset? AttemptLastStarted { get; set; }
        public DateTimeOffset? AttemptLastSubmitted { get; set; }
    }
}
