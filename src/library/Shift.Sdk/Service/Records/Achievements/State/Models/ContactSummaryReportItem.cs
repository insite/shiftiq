using System;

namespace InSite.Domain.Records
{
    public class ContactSummaryNoExpiryReportItem
    {
        public string AchievementDescription { get; set; }
        public string AchievementTitle { get; set; }
        public string EventTitle { get; set; }
        public DateTimeOffset EventScheduledStart { get; set; }
        public DateTimeOffset? EventScheduledEnd { get; set; }
        public string ScoreName { get; set; }
        public string ScoreType { get; set; }
        public int ScoreSequence { get; set; }
        public string ScoreComment { get; set; }
        public decimal? ScorePercent { get; set; }
        public string ScoreText { get; set; }
        public decimal? ScoreNumber { get; set; }
        public decimal? ScorePoint { get; set; }
    }

    public class ContactSummaryWithExpiryReportItem
    {
        public string AchievementTitle { get; set; }
        public DateTimeOffset Granted { get; set; }
        public DateTimeOffset ExpirationExpected { get; set; }
        public string ScoreType { get; set; }
        public int ScoreSequence { get; set; }
        public decimal? ScorePercent { get; set; }
        public string ScoreText { get; set; }
        public decimal? ScoreNumber { get; set; }
        public decimal? ScorePoint { get; set; }
        public DateTimeOffset? EventScheduledStart { get; set; }
    }

    public class CertificateWithMissingExpiry
    {
        public Guid UserIdentifier { get; set; }
        public Guid AchievementIdentifier { get; set; }
        public Guid CredentialIdentifier { get; set; }
        public int AchievementExpirationLifetimeQuantity { get; set; }
        public string AchievementExpirationLifetimeUnit { get; set; }
        public string AchievementTitle { get; set; }
        public string UserEmail { get; set; }
    }
}
