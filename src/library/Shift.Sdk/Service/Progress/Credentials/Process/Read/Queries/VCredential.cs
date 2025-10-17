using System;

using Shift.Common;

namespace InSite.Application.Records.Read
{
    public class VCredential
    {
        public Guid AchievementIdentifier { get; set; }
        public Guid CredentialIdentifier { get; set; }
        public Guid? ParentOrganizationIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
        public Guid? EmployerGroupStatusItemIdentifier { get; set; }

        public string AchievementDescription { get; set; }
        public string AchievementExpirationLifetimeUnit { get; set; }
        public string AchievementExpirationType { get; set; }
        public string AchievementLabel { get; set; }
        public string AchievementTitle { get; set; }
        public string AchievementCertificateLayoutCode { get; set; }

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
        public string CredentialRevokedReason { get; set; }
        public string CredentialStatus { get; set; }
        public string EmployerGroupName { get; set; }
        public string EmployerGroupStatus { get; set; }
        public string EmployerGroupRegion { get; set; }
        public string UserEmail { get; set; }
        public string UserFirstName { get; set; }
        public string UserFullName { get; set; }
        public string UserLastName { get; set; }
        public string UserRegion { get; set; }
        public string BadgeImageUrl { get; set; }
        public string PersonCode { get; set; }

        public bool AchievementIsEnabled { get; set; }
        public bool UserAccessGrantedToCmds { get; set; }
        public bool UserEmailEnabled { get; set; }
        public bool? HasBadgeImage { get; set; }

        public int? AchievementExpirationLifetimeQuantity { get; set; }
        public int? CredentialExpirationLifetimeQuantity { get; set; }
        public Guid? EmployerGroupIdentifier { get; set; }

        public decimal? CredentialHours { get; set; }

        public Guid? OriginalEmployerGroupIdentifier { get; set; }
        public string OriginalEmployerGroupName { get; set; }
        public string OriginalEmployerGroupStatus { get; set; }
        public string OriginalEmployerGroupRegion { get; set; }

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
        public string CredentialGrantedDescription { get; set; }
        public decimal? CredentialGrantedScore { get; set; }
        public DateTimeOffset? CredentialRevoked { get; set; }
        public DateTimeOffset? UserArchived { get; set; }
        public string CertificateFingerPrint => $"{CredentialIdentifier};{AchievementIdentifier};{CredentialStatus};{CredentialGranted.ToJsTime()};{(int?)(CredentialGrantedScore * 100)};{CredentialExpirationExpected.ToJsTime()};{UserIdentifier};{UserEmail};{UserFirstName};{UserLastName};{AchievementTitle}";
        public bool IsCredentialGranted => CredentialGranted.HasValue;

        public bool IsExpired
        {
            get
            {
                if (Enum.TryParse(CredentialStatus, true, out Shift.Constant.CredentialStatus status))
                    return status == Shift.Constant.CredentialStatus.Expired;

                return false;
            }
        }
    }
}
