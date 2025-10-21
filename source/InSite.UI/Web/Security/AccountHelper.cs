using System;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Configuration;

using Humanizer;

using InSite.Application.Sites.Read;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Web.Security
{
    public static class AccountHelper
    {
        public const int MaxSignInAttemptsNumber = 5;

        public static int SignInFailedAttemptsCount
        {
            get => (int)(GetSessionValue() ?? 0);
            private set => SetSessionValue(value);
        }

        public static DateTime? SignInLockedUntil
        {
            get
            {
                DateTime? value = null;

                if (SignInFailedAttemptsCount >= MaxSignInAttemptsNumber)
                {
                    value = (DateTime?)GetSessionValue();

                    if (!value.HasValue)
                        SetSessionValue(value = DateTime.UtcNow.AddMinutes(HttpContext.Current.Session.Timeout));

                    if (value.Value <= DateTime.UtcNow)
                    {
                        SetSessionValue(value = null);
                        SignInFailedAttemptsCount = 0;
                    }
                }

                return value;
            }
        }

        private static readonly string _sessionCookieName;

        static AccountHelper()
        {
            var sessionStateSection = (SessionStateSection)ConfigurationManager.GetSection("system.web/sessionState");
            _sessionCookieName = sessionStateSection?.CookieName;
        }

        internal static void OnSignInFailedAttempt()
        {
            SignInFailedAttemptsCount++;
        }

        internal static void OnPasswordChanged(Guid organizationId, Guid contactKey, string browserAddress)
        {
            var contact = UserSearch.Select(contactKey);

            organizationId = ServiceLocator.Partition.IsE03() ? OrganizationIdentifiers.CMDS : organizationId;

            ServiceLocator.AlertMailer.Send(organizationId, contact.UserIdentifier, new Domain.Messages.AlertPasswordChanged
            {
                BrowserAddress = browserAddress,
                UserTimeZone = contact.TimeZone,
                Recipient = contact.UserIdentifier,
                UserEmail = contact.Email,
                Tenant = organizationId
            });
        }

        internal static HttpCookie GetSessionCookie()
        {
            return !string.IsNullOrEmpty(_sessionCookieName)
                ? HttpContext.Current.Request.Cookies[_sessionCookieName]
                : null;
        }

        internal static TUserSession WriteLoginEvent(AuthenticationResult authResult, User user, Guid organization, string authSource, string error = null)
        {
            var request = HttpContext.Current.Request;
            var email = user.Email.Length > 254 ? user.Email.Substring(0, 254) : user.Email;

            var entity = new TUserSession
            {
                SessionIdentifier = UniqueIdentifier.Create(),
                SessionCode = GetSessionCookie()?.Value,
                SessionStarted = DateTimeOffset.UtcNow,
                UserAgent = request.UserAgent,
                UserBrowser = request.Browser.Browser,
                UserBrowserVersion = request.Browser.Version,
                UserHostAddress = request.UserHostAddress,
                UserEmail = email,
                UserLanguage = CookieTokenModule.DefaultLanguage,
                AuthenticationErrorType = GetError(authResult),
                AuthenticationErrorMessage = error,
                OrganizationIdentifier = organization,
                UserIdentifier = user.UserIdentifier,
                AuthenticationSource = authSource
            };

            if (authResult == AuthenticationResult.Success)
                entity.SessionIsAuthenticated = true;

            TUserSessionStore.Insert(entity);

            return entity;
        }

        internal static string GetError(AuthenticationResult result)
        {
            if (result == AuthenticationResult.Success)
                return null;

            if (result == AuthenticationResult.NoPerson)
                return LabelSearch.GetTranslation("User Sign In Error - Person Not Found", CookieTokenModule.Current.Language,
                    CurrentSessionState.Identity.Organization.Identifier);

            if (result == AuthenticationResult.Unapproved)
                return LabelSearch.GetTranslation("User Sign In Error - Account Not Approved", CookieTokenModule.Current.Language,
                    CurrentSessionState.Identity.Organization.Identifier);

            if (result == AuthenticationResult.Disabled)
                return LabelSearch.GetTranslation("User Sign In Error - Account Disabled", CookieTokenModule.Current.Language,
                    CurrentSessionState.Identity.Organization.Identifier);

            if (result == AuthenticationResult.InvalidPassword)
                return LabelSearch.GetTranslation("User Sign In Error - Invalid Password", CookieTokenModule.Current.Language,
                    CurrentSessionState.Identity.Organization.Identifier);

            if (result == AuthenticationResult.InvalidEmail)
                return LabelSearch.GetTranslation("User Sign In Error - Invalid Email", CookieTokenModule.Current.Language,
                    CurrentSessionState.Identity.Organization.Identifier);

            return LabelSearch.GetTranslation("User Sign In Error", CookieTokenModule.Current.Language,
                CurrentSessionState.Identity.Organization.Identifier);
        }

        internal static string GetLicenseAgreement(bool includeAddendum, bool shortVersion)
        {
            var language = GetCurrentLanguage();

            var agreement = GetLicenseAgreement(language, shortVersion);

            var html = Markdown.ToHtml(agreement);

            if (includeAddendum)
            {
                var addendum = GetLicenseAddendum(language);

                if (!string.IsNullOrEmpty(addendum))
                    html += $"<div class='mt-5'>{Markdown.ToHtml(addendum)}</div>";
            }
            
            return html;
        }

        private static string GetCurrentLanguage()
        {
            var language = CookieTokenModule.Current.Language;

            if (language != "en" && language != "fr")
                language = "en";

            return language;
        }

        private static string GetLicenseAgreement(string language, bool shortVersion)
        {
            var url = shortVersion 
                ? $"~/UI/Lobby/Models/Agreement-Short-{language}.md"
                : $"~/UI/Lobby/Models/Agreement-{language}.md";
            var path = HttpContext.Current.Request.MapPath(url);
            var text = System.IO.File.ReadAllText(path);
            return text;
        }

        private static string GetLicenseAddendum(string language)
        {
            string text = null;

            var organization = CurrentSessionState.Identity.Organization.Identifier;

            var filter = new QPageFilter
            {
                OrganizationIdentifier = organization,
                PageSlugExact = "eula"
            };

            var pages = ServiceLocator.PageSearch.Bind(x => new { x.PageIdentifier, x.LastChangeTime }, filter);

            if (pages.Length > 0)
            {
                var pageId = pages.OrderByDescending(x => x.LastChangeTime).First().PageIdentifier;
                text = ServiceLocator.ContentSearch.GetText(pageId, ContentLabel.Body, language);
            }

            return text;
        }

        #region Methods (helpers)

        private static object GetSessionValue([CallerMemberName] string name = null)
        {
            return string.IsNullOrEmpty(name)
                ? null
                : HttpContext.Current.Session[typeof(AccountHelper) + "." + name];
        }

        private static void SetSessionValue(object value, [CallerMemberName] string name = null)
        {
            if (!string.IsNullOrEmpty(name))
                HttpContext.Current.Session[typeof(AccountHelper) + "." + name] = value;
        }

        internal static string GetBruteForceError(string buttonName, DateTime lockedUntil)
        {
            var lockMinutes = (int)Math.Round((lockedUntil - DateTime.UtcNow).TotalMinutes);

            return "To prevent brute-force attacks on this system," +
                $" the {buttonName} button is disabled on this page after {MaxSignInAttemptsNumber} failed attempts." +
                $" Please wait {"minute".ToQuantity(lockMinutes)}," +
                $" or reset your browser cache, or contact your administrator to try again.";
        }

        #endregion
    }
}