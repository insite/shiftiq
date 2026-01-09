using System;
using System.Linq;
using System.Web;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Organizations;
using InSite.Persistence;
using InSite.UI;

using Shift.Common;
using Shift.Constant;

using static InSite.UI.Layout.Lobby.Controls.SignInBasePage;

namespace InSite.Web.SignIn
{
    public static partial class SignInLogic
    {
        #region Constants

        public static readonly RandomNumberGenerator _random = new RandomNumberGenerator();

        public const string SignInPageURL = "~/ui/lobby/signin";
        public const string SignInErrorPageURL = "~/ui/lobby/signin-failure";
        public const string SignInMFAPageURL = "~/ui/lobby/signin-mfa";
        public const string SignInChallengePageURL = "~/ui/lobby/signin-challenge";
        public const string SignInSocialPageURL = "~/ui/lobby/signin-social";

        #endregion

        #region Methods (Organizations)

        public static GetUserOrganization_Result GetUserOrganization(User user, bool isAuthentication, string returnUrl, bool hasExternalReturnUrl = false)
        {
            var originalOrganization = GetOrganization(user);

            var isCmdsMultiOrganization = false;
            var isShiftMultiOrganization = false;

            var isCmds = user.AccessGrantedToCmds && ServiceLocator.Partition.IsE03();

            var availableOrganization = isCmds
                ? OrganizationHelper.GetUserOrganizationCmds(user.UserIdentifier, CurrentIdentityFactory.GetOrganizationIdentifier(user.UserIdentifier), originalOrganization, out isCmdsMultiOrganization)
                : OrganizationHelper.GetUserOrganizationCore(user.UserIdentifier, CurrentIdentityFactory.GetOrganizationIdentifier(user.UserIdentifier), originalOrganization, out isShiftMultiOrganization);

            var availablePerson = PersonSearch.Select(availableOrganization.OrganizationIdentifier, user.UserIdentifier);
            if (availablePerson == null
                || availablePerson.UserAccessGranted == null
                || !availablePerson.IsAdministrator && !availablePerson.IsLearner
                )
            {
                var orgName = ServiceLocator.PersonSearch.IsPersonExist(originalOrganization.OrganizationIdentifier, user.UserIdentifier)
                    ? availableOrganization.CompanyName
                    : originalOrganization.CompanyName;

                return new GetUserOrganization_Result
                {
                    Error = (SignInErrorCodes.UnAuthorizedForOrganization, true, $"This user account ({user.Email}) is not granted access to this organization account ({orgName}).")
                };
            }

            return new GetUserOrganization_Result
            {
                Organization = availableOrganization,
                RedirectUrl = GetOrganizationRedirectUrl(
                    availableOrganization, user, availablePerson.IsAdministrator, isCmds,
                    isAuthentication && isCmdsMultiOrganization, isAuthentication && isShiftMultiOrganization,
                    returnUrl, hasExternalReturnUrl)
            };
        }

        private static OrganizationState GetOrganization(User user)
        {
            var persons = PersonSearch.GetOrganizationIds(user.UserIdentifier);
            if (persons.Length == 1)
                return OrganizationSearch.Select(persons[0]);

            var identity = CurrentSessionState.Identity;

            return identity != null && identity.Organization != null
                ? OrganizationSearch.Select(identity.Organization.Key)
                : OrganizationSearch.Select(CookieTokenModule.Current.OrganizationCode);
        }

        private static string GetOrganizationRedirectUrl(OrganizationState organization, User user, bool isAdministrator, bool isCmds, bool isCmdsMultiOrganization, bool isCoreMultiOrganization, string returnUrl, bool hasExternalReturnUrl)
        {
            returnUrl = (returnUrl?.Trim()).IfNullOrEmpty(HttpContext.Current.Request.QueryString["ReturnUrl"]);

            if ((returnUrl.IsEmpty()
                || ServiceLocator.AppSettings.Environment.Name != EnvironmentName.Local
                    && (
                        returnUrl.IndexOf(":") != -1
                        || !Uri.IsWellFormedUriString(returnUrl, UriKind.Relative)
                    )
                ) && !hasExternalReturnUrl)
            {
                returnUrl = null;
            }

            string url;

            if (isCmds)
            {
                url = returnUrl != null
                    ? returnUrl
                    : isCmdsMultiOrganization
                        ? "/ui/portal/security/organizations?login=1"
                        : Urls.CmdsHomeUrl;
            }
            else if (!ServiceLocator.Partition.IsE03() && UserPasswordCheck.IsPasswordExpired(user))
            {
                url = UserPasswordCheck.ChangePasswordUrl;
            }
            else if (returnUrl != null)
            {
                url = returnUrl;
            }
            else if (isCoreMultiOrganization)
            {
                url = "/ui/portal/security/organizations?login=1";
            }
            else if (organization.Toolkits.Portal.ShowMyDashboardAfterLogin)
            {
                url = GetPortalHomeUrl(organization, user);
            }
            else if (isAdministrator)
            {
                url = "/ui/admin/home";
            }
            else
            {
                url = null;
            }

            if (string.IsNullOrEmpty(url) || url.StartsWith("/ui/lobby/signin", StringComparison.OrdinalIgnoreCase))
                url = GetPortalHomeUrl(organization, user);

            return url.StartsWith("http")
                ? url
                : PathHelper.GetOrganizationUrl(ServiceLocator.AppSettings.Environment, organization.OrganizationCode, url);
        }

        private static string GetPortalHomeUrl(OrganizationState organization, User user)
        {
            if (organization?.Toolkits?.Sales?.ManagerGroup != null && organization?.Toolkits?.Sales?.LearnerGroup != null)
            {
                if (user.Memberships.Any(x => x.GroupIdentifier == organization.Toolkits.Sales.ManagerGroup))
                    return RelativeUrl.ManagerPortalHomeUrl;

                if (user.Memberships.Any(x => x.GroupIdentifier == organization.Toolkits.Sales.LearnerGroup))
                    return RelativeUrl.LearnerPortalHomeUrl;
            }

            return RelativeUrl.PortalHomeUrl;
        }

        #endregion

        #region Authentication

        public static (AuthenticationResult authResults, User user) AuthenticateUser(string username, string password, bool autologin)
        {
            if (autologin)
            {
                var user = UserSearch.SelectByEmail(username);

                return user is null
                    ? (AuthenticationResult.InvalidEmail, default)
                    : (AuthenticationResult.Success, user);
            }
            else
            {
                var authResult = UserSearch.ValidateUser(username, password, out var user);

                return (authResult, user);
            }
        }

        public static (bool MFAEnabled, TUserAuthenticationFactor mfa) CheckUserMFAStatus(Guid userId)
        {
            var mfa = TUserAuthenticationFactorSearch.GetMFARecordById(new TUserAuthenticationFactorFilter
            {
                UserIdentifier = userId
            });
            return (IsMultiFactorAuthenticationEnabled(mfa), mfa);
        }

        #endregion

        #region Methods (form redirect)

        public static void Redirect(bool endResponse = true) =>
            RedirectToSignInWithReturnURL(null, endResponse);

        public static void RedirectToSignInWithReturnURL(string returnUrl, bool endResponse = true) =>
            HttpResponseHelper.Redirect(GetUrl(returnUrl), endResponse);

        public static string GetUrl(string returnUrl = null) =>
            GetWebUrl(returnUrl).ToString();

        public static WebUrl GetWebUrl(string returnUrl = null)
        {
            var url = new WebUrl("/ui/lobby/signin");

            if (!string.IsNullOrEmpty(returnUrl))
                url.QueryString.Add("ReturnUrl", returnUrl);

            return url;
        }

        #endregion

        #region Helpers

        public static void UpdateLoginName(string oldLoginName, string newLoginName)
        {
            if (oldLoginName == newLoginName)
                return;

            var identity = CurrentSessionState.Identity;
            var loginChanged = identity.Name == oldLoginName;
            var impersonatorChanged = identity.Impersonator?.User?.Email == oldLoginName;

            if (!loginChanged && !impersonatorChanged)
                return;

            var loginName = loginChanged ? newLoginName : identity.Name;
            var organizationCode = identity.Organization.OrganizationCode;
            var impersonatorUser = impersonatorChanged ? newLoginName : identity.Impersonator?.User?.Email;
            var impersonatorOrganization = identity.Impersonator?.Organization?.Code;
            var token = CookieTokenModule.Current;

            CurrentIdentityFactory.SignedIn(loginName, identity.User.Identifier, organizationCode, impersonatorOrganization, impersonatorUser, token.Language, token.TimeZoneId, token.AuthenticationSource);

            HttpResponseHelper.Redirect("/", true);
        }

        public static bool IsMultiFactorAuthenticationEnabled(SignInUserMFA mfa)
        {
            return mfa != null && mfa.OtpMode != OtpModes.None;
        }

        public static bool IsMultiFactorAuthenticationEnabled(TUserAuthenticationFactor mfa)
        {
            return mfa != null && mfa.OtpMode != OtpModes.None;
        }

        #endregion
    }
}
