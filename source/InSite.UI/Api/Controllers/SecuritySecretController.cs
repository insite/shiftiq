using System;
using System.ComponentModel;
using System.Net.Http;
using System.Web.Http;

using InSite.Api.Settings;
using InSite.Application.People.Write;
using InSite.UI.Lobby.Utilities;

using Shift.Common;
using Shift.Constant;

namespace InSite.Api.Controllers
{
    [DisplayName("Security")]
    [RoutePrefix("api/secret")]
    public class SecuritySecretController : ApiBaseController
    {
        private readonly SecuritySettings _settings;

        public SecuritySecretController()
        {
            _settings = Global.GetSecuritySettings();
        }

        /// <summary>
        /// Generates a new client secret for the authenticated user, replacing any existing secret.
        /// </summary>
        /// <param name="expiry">The number of days until the secret expires. If omitted, the default is assumed (90 days).</param>
        /// <returns>The newly generated client secret.</returns>
        [HttpPost]
        [Route("generate")]
        public HttpResponseMessage GenerateNewClientSecret(int? expiry = null)
        {
            if (CurrentUser == null)
                throw new ArgumentNullException("User");

            if (CurrentOrganization == null)
                throw new ArgumentNullException("Organization");

            var person = ServiceLocator.PersonSearch.GetPerson(CurrentUser.Identifier, CurrentOrganization.Identifier)
                ?? throw new ArgumentNullException("person");

            var personId = person.PersonIdentifier;

            var tokenSettings = _settings.Token;

            var tokenLifetimeInMinutes = tokenSettings.Lifetime;

            var secretExpiryInDays = expiry ?? tokenSettings.GetClientSecretLifetimeInDays();

            var secret = TokenHelper.GetClientSecret(personId, true, secretExpiryInDays, tokenLifetimeInMinutes);

            return JsonSuccess(secret);
        }

        /// <summary>
        /// Extends the lifetime of the authenticated user's existing client secret without changing the secret value.
        /// </summary>
        /// <param name="expiry">The desired number of days until the secret expires. Capped at a maximum of 90 days.</param>
        /// <returns>The updated client secret with the new expiry.</returns>
        [HttpPut]
        [Route("extend")]
        public HttpResponseMessage ExtendClientSecretLifetime(int expiry)
        {
            if (CurrentUser == null)
                throw new ArgumentNullException("User");

            if (CurrentOrganization == null)
                throw new ArgumentNullException("Organization");

            var person = ServiceLocator.PersonSearch.GetPerson(CurrentUser.Identifier, CurrentOrganization.Identifier)
                ?? throw new ArgumentNullException("person");

            var personId = person.PersonIdentifier;

            var existingSecret = ServiceLocator.PersonSecretSearch.GetByPerson(personId, SecretName.ShiftClientSecret);

            if (existingSecret == null)
                return JsonBadRequest("No existing client secret found.");

            var tokenSettings = _settings.Token;

            var tokenLifetimeInMinutes = tokenSettings.Lifetime;

            ServiceLocator.SendCommand(new RemovePersonSecret(personId, existingSecret.SecretIdentifier));

            var secretId = UniqueIdentifier.Create();

            // Limit the maximum lifetime for a client secret.

            var secretExpiryInDays = Math.Min(expiry, tokenSettings.GetClientSecretLifetimeInDays());

            var secretExpiry = DateTimeOffset.Now.AddDays(secretExpiryInDays);

            var add = new AddPersonSecret(personId, secretId, SecretType.Authentication, SecretName.ShiftClientSecret, existingSecret.SecretValue, secretExpiry, tokenLifetimeInMinutes);

            ServiceLocator.SendCommand(add);

            var secret = ServiceLocator.PersonSecretSearch.GetSecret(secretId);

            return JsonSuccess(secret);
        }
    }
}