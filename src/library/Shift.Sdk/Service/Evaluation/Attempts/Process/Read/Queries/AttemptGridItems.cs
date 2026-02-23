using System;

namespace InSite.Application.Attempts.Read
{
    public class AttemptGridItem
    {
        public Guid AttemptIdentifier { get; set; }

        public Guid LearnerUserIdentifier { get; set; }
        public string LearnerName { get; set; }
        public string LearnerCode { get; set; }
        public string LearnerEmail { get; set; }
        public string AttemptTag { get; set; }

        public decimal? FormPoints { get; set; }
        public decimal? FormPassingScore { get; set; }

        public bool AttemptIsPassing { get; set; }
        public decimal? AttemptPoints { get; set; }
        public decimal AttemptScore { get; set; }
        public string SebVersion { get; set; }
        public DateTimeOffset? AttemptStarted { get; set; }
        public DateTimeOffset? AttemptSubmitted { get; set; }
        public DateTimeOffset? AttemptGraded { get; set; }
    }
}
