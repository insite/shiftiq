using System;

namespace InSite.Persistence.Plugin.CMDS
{
    public class VCmdsCredential
    {
        public Guid AchievementIdentifier { get; set; }
        public Guid CredentialIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public string AchievementDescription { get; set; }
        public string AchievementExpirationLifetimeUnit { get; set; }
        public string AchievementExpirationType { get; set; }
        public string AchievementLabel { get; set; }
        public string AchievementTitle { get; set; }
        public string AchievementVisibility { get; set; }

        public Guid? AuthorityIdentifier { get; set; }
        public string AuthorityLocation { get; set; }
        public string AuthorityName { get; set; }
        public string AuthorityType { get; set; }
        public string AuthorityReference { get; set; }

        public string CredentialDescription { get; set; }
        public string CredentialExpirationLifetimeUnit { get; set; }
        public string CredentialExpirationType { get; set; }
        public string CredentialNecessity { get; set; }
        public string CredentialPriority { get; set; }
        public string CredentialStatus { get; set; }
        public string EmployerGroupName { get; set; }
        public string PersonCode { get; set; }
        public string PersonFullName { get; set; }
        public string UserEmail { get; set; }
        public string UserFirstName { get; set; }
        public string UserFullName { get; set; }
        public string UserLastName { get; set; }
        public string UserRegion { get; set; }

        public bool AchievementAllowSelfDeclared { get; set; }
        public bool AchievementIsEnabled { get; set; }
        public bool CredentialIsMandatory { get; set; }
        public bool IsInTrainingPlan { get; set; }

        public int? AchievementExpirationLifetimeQuantity { get; set; }
        public int? CredentialExpirationLifetimeQuantity { get; set; }
        public Guid? EmployerGroupIdentifier { get; set; }

        public decimal? CredentialHours { get; set; }

        public DateTimeOffset? AchievementExpirationFixedDate { get; set; }
        public DateTimeOffset? CredentialAssigned { get; set; }
        public DateTimeOffset? CredentialExpirationExpected { get; set; }
        public DateTimeOffset? CredentialExpirationFixedDate { get; set; }
        public DateTimeOffset? CredentialExpirationReminderDelivered0 { get; set; }
        public DateTimeOffset? CredentialExpirationReminderDelivered1 { get; set; }
        public DateTimeOffset? CredentialExpirationReminderDelivered2 { get; set; }
        public DateTimeOffset? CredentialExpirationReminderDelivered3 { get; set; }
        public DateTimeOffset? CredentialExpirationReminderRequested0 { get; set; }
        public DateTimeOffset? CredentialExpirationReminderRequested1 { get; set; }
        public DateTimeOffset? CredentialExpirationReminderRequested2 { get; set; }
        public DateTimeOffset? CredentialExpirationReminderRequested3 { get; set; }
        public DateTimeOffset? CredentialExpired { get; set; }
        public DateTimeOffset? CredentialGranted { get; set; }
        public DateTimeOffset? CredentialRevoked { get; set; }
        public DateTimeOffset? UserArchived { get; set; }
        public DateTimeOffset? UserUtcArchived { get; set; }

        public virtual VCmdsAchievement Achievement { get; set; }
        public virtual User User { get; set; }
    }
}
