using System;

using Shift.Common;

namespace InSite.Application.Records.Read
{
    [Serializable]
    public class VCredentialFilter : Filter
    {
        public bool? AchievementHasCertificate { get; set; }
        public bool? IsPendingExpiry { get; set; }
        public bool? IsPendingReminderRequest { get; set; }
        public bool? IsGranted { get; set; }
        public Guid? AchievementIdentifier { get; set; }
        public Guid? DepartmentIdentifier { get; set; }
        public Guid? UserGradebookIdentifier { get; set; }
        public Guid? ProgramIdentifier { get; set; }
        public Guid? ItemGradebookIdentifier { get; set; }
        public Guid? JournalSetupIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }
        public Guid? EmployerGroupIdentifier { get; set; }
        public string CredentialStatus { get; set; }
        public string UserEmail { get; set; }
        public string UserFullName { get; set; }
        public string UserRegion { get; set; }
        public string PersonCode { get; set; }
        public string EmployerGroupStatus { get; set; }
        public string EmployerGroupName { get; set; }

        public string ContactKeyword { get; set; }
        public string AchievementLabel { get; set; }
        public string[] AchievementLabels { get; set; }
        public string AchievementTitle { get; set; }

        public Guid[] AchievementIdentifiers { get; set; }
        public Guid[] CredentialIdentifiers { get; set; }
        public Guid[] ExcludeAchievements { get; set; }

        public DateTimeOffset? CredentialExpiredSince { get; set; }
        public DateTimeOffset? CredentialExpiredBefore { get; set; }
        public DateTimeOffset? CredentialGrantedSince { get; set; }
        public DateTimeOffset? CredentialGrantedBefore { get; set; }
        public DateTimeOffset? CredentialRevokedSince { get; set; }
        public DateTimeOffset? CredentialRevokedBefore { get; set; }
        public DateTimeOffset? CredentialExpirationExpectedSince { get; set; }
        public DateTimeOffset? CredentialExpirationExpectedBefore { get; set; }
    }
}
