using System;

namespace Shift.Contract
{
    public partial class CredentialMatch
    {
        public Guid AchievementId { get; set; }
        public Guid CredentialId { get; set; }
        public Guid LearnerId { get; set; }

        public DateTimeOffset? CredentialIssued { get; set; }
        public string CredentialStatus { get; set; }
        public string CredentialNecessity { get; set; }

        public bool CredentialIsRequired { get; set; }
    }
}