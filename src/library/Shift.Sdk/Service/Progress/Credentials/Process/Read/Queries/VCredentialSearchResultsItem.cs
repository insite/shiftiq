using System;

namespace InSite.Application.Records.Read
{
    public class VCredentialSearchResultsItem
    {
        public Guid AchievementIdentifier { get; set; }
        public string AchievementTitle { get; set; }
        public string AchievementTag { get; set; }
        public Guid UserIdentifier { get; set; }
        public string UserFullName { get; set; }
        public string UserEmail { get; set; }
        public string PersonCode { get; set; }
        public Guid CredentialIdentifier { get; set; }
        public string CredentialStatus { get; set; }
        public DateTimeOffset? CredentialGranted { get; set; }
        public DateTimeOffset? CredentialRevoked { get; set; }
        public Guid? EmployerGroupIdentifier { get; set; }
        public string EmployerGroupName { get; set; }
        public string EmployerGroupStatus { get; set; }
        public string EmployerGroupRegion { get; set; }
        public string AchievementCertificateLayoutCode { get; set; }
        public bool? HasBadgeImage { get; set; }
        public string BadgeImageUrl { get; set; }
        public DateTimeOffset? CredentialExpirationExpected { get; set; }
        public DateTimeOffset? CredentialExpired { get; set; }
        public string Department { get; set; }
    }
}
