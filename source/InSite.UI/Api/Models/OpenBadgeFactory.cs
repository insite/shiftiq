using System;

using InSite.Application.Records.Read;
using InSite.Domain.Organizations;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Api.Models.Records.OpenBadges
{
    public class OpenBadgeFactory
    {
        private const string DefaultBadgeDescription = "No Description";
        private const string DefaultCriteriaNarrative = "To earn the **{0}** open badge, a learner must...";

        public ProfileObject CreateIssuer(OrganizationState organization, string host)
        {
            return new ProfileObject($"https://{host}/api/openbadges/issuer", ProfileType.Issuer)
            {
                Name = organization.CompanyDescription.LegalName.IfNullOrEmpty(organization.Name),
                Description = organization.CompanyDescription.CompanySummary.NullIfEmpty(),
                Url = BaseLinkedData.GetUrl(organization.PlatformCustomization.TenantUrl.WebSite)
                    ?? new Uri("about:blank"),
                Email = organization.PlatformCustomization.TenantLocation.Email.IfNullOrEmpty("-")
            };
        }

        public BadgeObject CreateBadge(QAchievement achievement, OrganizationState organization, string host)
        {
            var badgeUrl = achievement.BadgeImageUrl.EmptyIfNull();
            if (badgeUrl.StartsWith("/"))
                badgeUrl = $"https://{host}" + badgeUrl;

            var domain = ServiceLocator.AppSettings.Security.Domain;

            var environment = ServiceLocator.AppSettings.Release.GetEnvironment();

            var defaultBadgeUrl = UrlHelper.GetAbsoluteUrl(domain, environment, "/images/badges/shiftiq-badge.png", organization.Code);

            return new BadgeObject($"https://{host}/api/openbadges/achievements/{achievement.AchievementIdentifier}")
            {
                Name = achievement.AchievementTitle,
                Description = achievement.AchievementDescription.IfNullOrEmpty(DefaultBadgeDescription),
                Image = BaseLinkedData.GetUrl(badgeUrl) ?? new Uri(defaultBadgeUrl),
                Tags = new[] { "subscriber", "reader" },

                Issuer = CreateIssuer(organization, host),
                Criteria = new CriteriaObject
                {
                    Narrative = DefaultCriteriaNarrative.Format(achievement.AchievementTitle)
                }
            };
        }

        public AssertionObject CreateAssertion(VCredential credential, QAchievement achievement, OrganizationState organization, string host)
        {
            return new AssertionObject($"https://{host}/api/openbadges/credentials/{credential.CredentialIdentifier}")
            {
                IssuedOn = credential.CredentialGranted.Value,
                Expires = credential.CredentialExpired ?? credential.CredentialExpirationExpected,

                Recipient = new IdentityObject(IdentityType.Email, credential.UserEmail, true),
                Badge = CreateBadge(achievement, organization, host),
                Verification = new VerificationObject(VerificationType.HostedBadge)
            };
        }

        public AssertionRevokedObject CreateRevocation(VCredential credential, string host)
        {
            return new AssertionRevokedObject($"https://{host}/api/openbadges/credentials/{credential.CredentialIdentifier}")
            {
                RevocationReason = credential.CredentialRevokedReason
                    ?? $"This credential was revoked {credential.CredentialRevoked}."
            };
        }
    }
}