using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using InSite.Application.Contacts.Read;
using InSite.Common.Web;
using InSite.Domain.Foundations;
using InSite.Persistence;
using InSite.UI;

using Shift.Common;
using Shift.Contract;

namespace InSite
{
    public static class CurrentIdentityFactory
    {
        public static void CheckPageAccess()
        {
            var identity = CurrentSessionState.Identity;

            // If the request is authenticated but we don't know who the user is, then we need to 
            // restart the authentication process.

            var context = HttpContext.Current;
            var request = context.Request;

            if (!request.IsAuthenticated && !identity.IsAuthenticated)
                return;

            if (identity.User == null || identity.Organization == null)
            {
                SignedOut();

                var reason = string.Empty;

                if (identity.User == null)
                    reason = "User is null";

                if (identity.Organization == null)
                    reason = "Organization is null";

                UI.Lobby.SignOut.Redirect($"CurrentIdentityFactory:CheckPageAccess", reason);

                context.ApplicationInstance.CompleteRequest();
            }

            if (!StringHelper.Equals(request.RawUrl, UI.Lobby.SignOut.GetUrl()))
            {
                var activeOrganizationCode = UrlHelper.GetOrganizationCode(HttpContext.Current.Request.Url);
                if (!string.Equals(identity.Organization.OrganizationCode, activeOrganizationCode, StringComparison.OrdinalIgnoreCase))
                    SetOrganizationAndRedirect(identity.Organization.OrganizationCode, null);
            }
        }

        public static ISecurityFramework CreateCurrentIdentity(CookieToken token, string organizationCode, List<QPerson> people)
        {
            try
            {
                var search = new CurrentIdentitySearch();

                var identity = search.Get(
                    token.UserEmail,
                    organizationCode,
                    token.Language,
                    token.ImpersonatorUser,
                    token.ImpersonatorOrganization,
                    people,
                    ServiceLocator.GroupSearch,
                    ServiceLocator.PersonSearch
                );

                token.Language = identity.Language;

                return identity;
            }
            catch (MissingPersonException ex)
            {
                // If an impersonator has attempted to select an organization account where he does not have access,
                // then redirect him back to his original organization account, and to the 403 (Forbidden) error page,
                // with a message to indicate the problem.

                if (token.ImpersonatorUser != null && token.ImpersonatorOrganization != null)
                {
                    var url = ServiceLocator.Urls.GetApplicationUrl(token.ImpersonatorOrganization)
                        + "/403?message="
                        + StringHelper.EncodeBase64Url(ex.Message);

                    HttpResponseHelper.Redirect(url);
                }
            }

            // This case should never occur.
            throw new NotImplementedException();
        }

        public static void Rebind(string userName, Guid organizationIdentifier)
        {
            CurrentSessionState.Identity.User = UserSearch.SelectWebContact(userName, organizationIdentifier);
        }

        public static void RedirectOrganization(string organizationCode, string redirectUrl)
        {
            var url = PathHelper.GetOrganizationUrl(ServiceLocator.AppSettings.Environment, organizationCode, HttpContext.Current.Request.RawUrl);

            if (!string.IsNullOrEmpty(redirectUrl))
            {
                var index = url.IndexOf('/', url.IndexOf(':') + 3);

                if (index > 0)
                    url = url.Substring(0, index);

                if (!redirectUrl.StartsWith("/"))
                    url += "/";

                url += redirectUrl;
            }

            HttpResponseHelper.Redirect(url);
        }

        public static Guid ActiveOrganizationIdentifier
            => CurrentSessionState.Identity.Organization.Identifier;

        public static Guid? GetOrganizationIdentifier(Guid userThumbprint)
        {
            return PersonalizationRepository.GetValue<Guid?>(Guid.Empty, userThumbprint, nameof(ActiveOrganizationIdentifier), false);
        }

        public static void SetOrganization(Guid organizationId, string organizationCode, string url, Guid user)
        {
            if (user != null)
                PersonalizationRepository.SetValue<Guid?>(Guid.Empty, user, nameof(ActiveOrganizationIdentifier), organizationId);

            HttpContext.Current.Items[nameof(ActiveOrganizationIdentifier)] = organizationId;

            SetOrganizationAndRedirect(organizationCode, url);
        }

        public static void SetOrganizationAndRedirect(string organization, string redirectUrl)
        {
            var token = CookieTokenModule.Current;

            var identity = CreateCurrentIdentity(token, organization, null);
            if (identity.Organization?.Code != organization)
                throw ApplicationError.Create("Organization Not Found: " + organization);

            CurrentSessionState.Identity = identity;

            if (identity.User != null && identity.Organization != null)
            {
                SessionHelper.StartSession(identity.Organization.Key, identity.User.UserIdentifier);

                RecentSessionHelper.Clear();
            }

            RedirectOrganization(organization, redirectUrl);
        }

        public static void SignedIn(
            string userEmail,
            Guid userId,
            string organization,
            string impersonatorOrganization,
            string impersonatorUser,
            string language,
            string timeZoneId,
            string authSource
            )
        {
            var organizationEntity = OrganizationSearch.Select(organization);
            if (organizationEntity == null)
                throw ApplicationError.Create("Organization Not Found: " + organization);

            var parentOrganizationId = organizationEntity.ParentOrganizationIdentifier;

            var roles = ServiceLocator.GroupSearch.BindGroups(g => g.GroupName,
                    g => (g.Organization.OrganizationCode == organization || (parentOrganizationId != null && g.OrganizationIdentifier == parentOrganizationId))
                      && g.GroupType == "Role"
                      && g.QMemberships.Any(m => m.GroupIdentifier == g.GroupIdentifier
                                              && m.UserIdentifier == userId))
                    .ToArray();

            var people = ServiceLocator.PersonSearch.GetPersons(new QPersonFilter { UserIdentifier = userId });
            var person = people.First(x => x.OrganizationIdentifier == organizationEntity.OrganizationIdentifier);

            var isAdministrator = people.Any(x => x.IsAdministrator);
            var isDeveloper = person?.IsDeveloper ?? false;
            var isOperator = person?.IsOperator ?? false;

            var token = CookieTokenModule.SignedIn(
                userEmail,
                userId,
                isAdministrator,
                isDeveloper,
                isOperator,
                roles,
                impersonatorOrganization,
                impersonatorUser,
                language,
                timeZoneId,
                authSource
            );

            var identity = CreateCurrentIdentity(token, organization, people);

            if (identity.Organization == null)
                throw ApplicationError.Create("Organization Not Found: " + organization);

            CurrentSessionState.Identity = identity;
        }

        public static void SignedOut()
        {
            CurrentSessionState.Identity = null;
        }
    }
}