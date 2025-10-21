using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

using InSite.Api.Models.Records.OpenBadges;
using InSite.Api.Settings;
using InSite.Application.Records.Read;
using InSite.Domain.Organizations;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Api.Controllers
{
    [DisplayName("Progress")]
    [ApiAuthenticationRequirement(ApiAuthenticationType.None)]
    public partial class OpenBadgesController : ApiBaseController
    {
        private readonly OpenBadgeFactory _factory = new OpenBadgeFactory();

        [HttpGet]
        [Route("api/openbadges/issuer")]
        public HttpResponseMessage Issuer()
        {
            try
            {
                var organization = GetOrganization();
                var issuer = _factory.CreateIssuer(organization, HttpContext.Current.Request.Url.Host);

                return JsonSuccess(issuer);
            }
            catch (Exception ex)
            {
                AppSentry.SentryError(ex);
                return JsonError(ex.Message);
            }
        }

        private const string ErrorInvalidAchievementId = "Invalid achievement identifier.";
        private const string ErrorAchievementNotFound = "Achievement not found.";

        [HttpGet]
        [Route("api/openbadges/achievements/{id}")]
        public HttpResponseMessage Achievements(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var achievementId))
                    return JsonError(ErrorInvalidAchievementId, HttpStatusCode.BadRequest);

                var organization = GetOrganization();
                if (!SelectAchievement(organization, achievementId, out var achievement))
                    return JsonError(ErrorAchievementNotFound, HttpStatusCode.NotFound);

                var badge = _factory.CreateBadge(achievement, organization, HttpContext.Current.Request.Url.Host);

                return JsonSuccess(badge);
            }
            catch (Exception ex)
            {
                AppSentry.SentryError(ex);
                return JsonError(ex.Message);
            }
        }

        private static bool SelectAchievement(OrganizationState organization, Guid achievementId, out QAchievement achievement)
        {
            achievement = ServiceLocator.AchievementSearch.GetAchievement(achievementId);

            return achievement != null && achievement.OrganizationIdentifier == organization.Identifier;
        }

        private const string ErrorInvalidCredentialId = "Invalid credential identifier.";
        private const string ErrorCredentialNotFound = "Credential not found.";

        [HttpGet]
        [Route("api/openbadges/credentials/{id}")]
        public HttpResponseMessage Credential(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var credentialId))
                    return JsonError(ErrorInvalidCredentialId, HttpStatusCode.BadRequest);

                var organization = GetOrganization();
                if (!SelectCredential(organization, credentialId, out var credential, out var achievement))
                    return JsonError(ErrorCredentialNotFound, HttpStatusCode.NotFound);

                var host = HttpContext.Current.Request.Url.Host;

                if (credential.CredentialRevoked.HasValue)
                {
                    var revocation = _factory.CreateRevocation(credential, host);
                    return JsonError(revocation, HttpStatusCode.Gone);
                }

                if (!credential.CredentialGranted.HasValue)
                    return JsonError(ErrorCredentialNotFound, HttpStatusCode.NotFound);

                var assertion = _factory.CreateAssertion(credential, achievement, organization, host);

                return JsonSuccess(assertion);
            }
            catch (Exception ex)
            {
                AppSentry.SentryError(ex);
                return JsonError(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/openbadges/validate")]
        public HttpResponseMessage Validate()
        {
            var organization = GetOrganization();
            if (organization.Code != "demo")
                return JsonError("NOT FOUND", HttpStatusCode.NotFound);

            var request = HttpContext.Current.Request;
            var data = request["data"];
            var identity = request["identity"];

            if (data.StartsWith("http"))
                data = JsonConvert.SerializeObject(data);
            else if (!data.StartsWith("{"))
                return JsonError("NOT FOUND", HttpStatusCode.NotFound);

            AssertionObject obj = null;

            try
            {
                obj = JsonConvert.DeserializeObject<AssertionObject>(data);

                obj.Verify();

                return Success(true, null);
            }
            catch (ApplicationError appex)
            {
                return Success(false, appex.Message);
            }
            catch (Exception ex)
            {
                return JsonError(ex.Message);
            }

            HttpResponseMessage Success(bool valid, string error)
            {
                return JsonSuccess(new
                {
                    valid = valid,
                    assertion = obj,
                    identity = identity.IsEmpty() ? (bool?)null : obj != null && obj.Recipient.Verify(identity),
                    error = error
                });
            }
        }

        private static bool SelectCredential(OrganizationState organization, Guid credentialId, out VCredential credential, out QAchievement achievement)
        {
            credential = ServiceLocator.AchievementSearch.GetCredential(credentialId);
            achievement = null;

            return credential != null
                && credential.OrganizationIdentifier == organization.Identifier
                && SelectAchievement(organization, credential.AchievementIdentifier, out achievement);
        }
    }
}
