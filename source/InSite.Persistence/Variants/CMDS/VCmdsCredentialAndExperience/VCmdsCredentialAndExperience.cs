using System;

namespace InSite.Persistence.Plugin.CMDS
{
    public class VCmdsCredentialAndExperience
    {
        public Guid AchievementIdentifier { get; set; }
        public Guid? AchievementOrganizationIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
        public Guid? CredentialIdentifier { get; set; }

        public string AchievementLabel { get; set; }
        public string AchievementTitle { get; set; }
        public string AuthorityLocation { get; set; }
        public string AuthorityName { get; set; }
        public string AuthorityReference { get; set; }
        public string AchievementDescription { get; set; }
        public string CredentialDescription { get; set; }
        public string UserFullName { get; set; }

        public bool? CredentialIsTimeSensitive { get; set; }
        
        public bool AchievementReportingDisabled { get; set; }
        public bool CredentialIsMandatory { get; set; }
        public bool CredentialIsPlanned { get; set; }
        public Guid ExperienceIdentifier { get; set; }

        public DateTimeOffset? CredentialExpired { get; set; }
        public DateTimeOffset? CredentialExpirationExpected { get; set; }
        public DateTimeOffset? CredentialGranted { get; set; }

        public string AchievementVisibility { get; set; }
        public string CredentialStatus { get; set; }
        public bool IsSuccess { get; set; }
        public decimal? CreditHours { get; set; }
        public decimal? GradePercent { get; set; }
        public int? LifetimeMonths { get; set; }
        public bool? IsInTrainingPlan { get; set; }
        public string ProgramTitle { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }

        public decimal? CredentialGrantedScore { get; set; }
        public decimal? CredentialRevokedScore { get; set; }
    }
}
