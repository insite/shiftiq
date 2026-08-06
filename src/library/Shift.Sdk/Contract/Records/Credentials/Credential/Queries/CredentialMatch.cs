using System;

namespace Shift.Contract
{
    public partial class CredentialMatch
    {
        public Guid AchievementId { get; set; }
        public Guid CredentialId { get; set; }
        public Guid UserId { get; set; }

        public DateTimeOffset? CredentialIssued { get; set; }
        public string CredentialStatus { get; set; }
        public string CredentialNecessity { get; set; }

        public bool CredentialIsRequired { get; set; }

        public string AchievementLabel { get; set; }
        public string AchievementTitle { get; set; }

        public string PersonCode { get; set; }
        public DateTimeOffset? AchievementEffectiveDate { get; set; }
        public DateTimeOffset? AchievementExpiryDate { get; set; }
        public bool AchievementValid { get; set; }
    }
}
