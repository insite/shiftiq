using System;

namespace InSite.Application.Records.Read
{
    public class QCredentialHistory
    {
        public Guid AggregateIdentifier { get; set; }
        public int AggregateVersion { get; set; }
        public Guid AchievementIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
        public DateTimeOffset ChangeTime { get; set; }
        public Guid ChangeBy { get; set; }
        public DateTimeOffset? CredentialActionDate { get; set; }
        public string CredentialActionType { get; set; }
        public decimal? CredentialScore { get; set; }
    }
}
