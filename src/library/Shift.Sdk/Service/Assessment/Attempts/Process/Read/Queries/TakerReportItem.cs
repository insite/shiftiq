using System;

namespace InSite.Application.Attempts.Read
{
    public class TakerReportItem
    {
        public Guid AttemptIdentifier { get; set; }
        public Guid FormIdentifier { get; set; }
        public Guid BankIdentifier { get; set; }
        public string FormName { get; set; }
        public string FormTitle { get; set; }
        public DateTimeOffset? AttemptStarted { get; set; }
        public DateTimeOffset? AttemptGraded { get; set; }
        public DateTimeOffset? AttemptSubmitted { get; set; }
        public DateTimeOffset? AttemptImported { get; set; }
        public bool AttemptIsPassing { get; set; }
        public decimal? AttemptScore { get; set; }
        public decimal? AttemptPoints { get; set; }
        public decimal? FormPoints { get; set; }
        public decimal? AttemptDuration { get; set; }
        public int FormAssetVersion { get; set; }
        public DateTimeOffset? FormFirstPublished { get; set; }
        public int FormAsset { get; set; }
    }
}
