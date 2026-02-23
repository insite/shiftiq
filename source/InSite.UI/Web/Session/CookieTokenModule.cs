using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Lobby;

using Shift.Common;
using Shift.Contract;

namespace InSite
{
    public class CookieTokenModule : IHttpModule
    {
        private static string Environment
            => ServiceLocator.AppSettings.Environment.Name.ToString();

        public static SecuritySettings SecuritySettings
            => ServiceLocator.AppSettings.Security;

        public static CookieSettings CookieSettings
            => ServiceLocator.AppSettings.Security.Cookie;

        public static IdCookieSettings IdCookieSettings
            => ServiceLocator.AppSettings.Security.IdCookie;

        #region Constants

        public const string DefaultOrganization = "insite";

        private const string English = "en";

        #endregion

        #region Properties

        public static CookieToken Current
        {
            get
            {
                var context = HttpContext.Current;
                var session = context.Session;
                var request = context.Request;

                CookieToken token = null;

                if (session != null)
                    token = session[CookieSettings.Name] as CookieToken;

                return token ?? GetToken(session, request);
            }
        }

        public static string DefaultLanguage
        {
            get
            {
                var languages = HttpContext.Current.Request.UserLanguages;

                if (languages == null || languages.Length <= 0)
                    return "en";

                var match = Regex.Match(languages[0], @"^(\w{2})-(\w{2})");
                if (match.Success)
                    return match.Groups[1].Value;

                match = Regex.Match(languages[0], @"^(\w{2})");
                return match.Success ? match.Groups[1].Value : English;
            }
        }

        #endregion

        #region Static fields

        private static readonly object WarningSyncRoot = new object();
        private static DateTimeOffset _warningLockedUntil = DateTimeOffset.MinValue;
        private static readonly GuidCache<CookieToken> _activeTokens;
        private static readonly ConcurrentDictionary<string, CookieToken> _activeSessions = new ConcurrentDictionary<string, CookieToken>();

        static CookieTokenModule()
        {
            _activeTokens = new GuidCache<CookieToken>();
            _activeTokens.ItemAdded += ActiveTokens_ItemAdded;
            _activeTokens.ItemRemoved += ActiveTokens_ItemRemoved;
        }

        #endregion

        #region Methods (initialization)

        public void Init(HttpApplication context)
        {
            context.BeginRequest += OnBeginRequest;
            context.AuthenticateRequest += OnAuthenticateRequest;
            context.AcquireRequestState += OnAcquireRequestState;
        }

        public void Dispose()
        {
        }

        #endregion

        #region Methods (event handling)

        private void OnBeginRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;

            if (HttpRequestHelper.IsStaticAsset(app.Context.Request))
            {
                app.Context.Items["SkipCookieAuthentication"] = true;
                return;
            }
        }

        private void OnAuthenticateRequest(object sender, EventArgs e)
        {
            var context = HttpContext.Current;

            if (context.Items["SkipCookieAuthentication"] is bool skip && skip)
                return;

            if (context.User != null)
                return;

            var token = GetToken(context.Session, context.Request);

            var isValid = !token.IsExpired() && token.TargetsEnvironment(Environment) && token.UserEmail.IsNotEmpty();

            if (isValid)
                context.User = new GenericPrincipal(new GenericIdentity(token.UserEmail), null);
        }

        private static void OnAcquireRequestState(object sender, EventArgs e)
        {
            var context = HttpContext.Current;

            if (Global.IsRequestIgnored(context))
                return;

            string extension;

            try
            {
                extension = Path.GetExtension(context.Request.Path);
            }
            catch (ArgumentException)
            {
                return;
            }

            if (!string.IsNullOrEmpty(extension) && !StringHelper.Equals(extension, ".aspx") || !Uri.TryCreate(context.Request.Path, UriKind.RelativeOrAbsolute, out _))
                return;

            var request = context.Request;
            var response = context.Response;
            var session = context.Session;
            var token = GetToken(session, request);

            if (token.IsExpired() || !token.TargetsEnvironment(Environment))
            {
                token.UserEmail = null;
                token.UserIdentifier = null;
                RemoveActiveToken(token, true);
            }

            OverrideFromQueryString(request, token);

            SetToken(response, session, token);

            if (session != null)
                AddActiveToken(token, session.Timeout);
        }

        public static CookieToken SignedIn(
            string userEmail,
            Guid userId,
            bool isAdministrator,
            bool isDeveloper,
            bool isOperator,
            string[] roles,
            string impersonatorOrganization,
            string impersonatorUser,
            string language,
            string timeZoneId,
            string authSource
            )
        {
            var context = HttpContext.Current;
            var request = context.Request;
            var response = context.Response;
            var session = context.Session;
            var token = GetToken(session, request);

            token.UserEmail = userEmail;
            token.UserIdentifier = userId;

            token.IsAdministrator = isAdministrator;
            token.IsDeveloper = isDeveloper;
            token.IsOperator = isOperator;

            token.UserRoles = roles;
            token.ImpersonatorOrganization = impersonatorOrganization;
            token.ImpersonatorUser = impersonatorUser;
            token.Language = language.IsEmpty() || !Language.CodeExists(language) ? DefaultLanguage : language;
            token.TimeZoneId = timeZoneId;
            token.Session = session?.SessionID;
            token.AuthenticationSource = authSource;

            SetToken(response, session, token);
            AddActiveToken(token, session.Timeout);

            return token;
        }

        public static CookieToken SignedOut()
        {
            var context = HttpContext.Current;
            var cache = context.Cache;
            var request = context.Request;
            var response = context.Response;
            var session = context.Session;
            var token = GetToken(session, request);

            token.UserEmail = null;
            token.UserIdentifier = null;

            SetToken(response, session, token);
            RemoveActiveToken(token, true);

            return token;
        }

        public static CookieToken SetOrganization(string organization)
        {
            var context = HttpContext.Current;
            var cache = context.Cache;
            var request = context.Request;
            var response = context.Response;
            var session = context.Session;
            var token = GetToken(session, request);

            token.OrganizationIdentifier = null;
            token.OrganizationCode = null;

            token.CurrentOrganization = organization;

            SetToken(response, session, token);
            AddActiveToken(token, session.Timeout);

            return token;
        }

        public static void ResetCreated()
        {
            var context = HttpContext.Current;
            var cache = context.Cache;
            var request = context.Request;
            var response = context.Response;
            var session = context.Session;
            var token = GetToken(session, request);

            token.ResetCreated();

            SetToken(response, session, token);
            AddActiveToken(token, session.Timeout);
        }

        #endregion

        #region Methods (cookie handling)

        private static CookieToken GetDefaultToken()
        {
            var token = new CookieToken();

            token.Language = DefaultLanguage;

            token.Environment = Environment;

            return token;
        }

        public static CookieToken GetToken(HttpSessionState session, HttpRequest request)
        {
            var token = (CookieToken)session?[CookieSettings.Name];

            if (token == null
                || Guid.TryParse(request.Cookies.Get(IdCookieSettings.Name)?.Value, out var newId)
                    && token.ID != newId
                )
            {
                var data = request.Headers[CookieSettings.Name];
                if (string.IsNullOrEmpty(data))
                    data = request.Cookies.Get(CookieSettings.Name)?.Value;

                var encoder = new CookieTokenEncoder();

                var encrypt = CookieSettings.Encrypt;

                var secret = GetSecret();

                try
                {
                    token = encoder.Deserialize(data, encrypt, secret, true);
                }
                catch (CookieSerializationException)
                {
                    token = null;
                }

                if (!IsValid(token))
                    token = GetDefaultToken();
            }

            token.CurrentBrowser = request.Browser.Browser;
            token.CurrentBrowserVersion = request.Browser.Version;

            return token;
        }

        private static void OverrideFromQueryString(HttpRequest request, CookieToken token)
        {
            var language = request.QueryString["language"];
            if (language.HasValue() && token.Language != language && Language.CodeExists(language))
            {
                token.ResetID();
                token.Language = language;
            }

            var organization = UrlHelper.GetOrganizationCode(request.Url);
            if (organization != null)
            {
                var organizationId = OrganizationSearch.Select(organization)?.OrganizationIdentifier;

                if (token.OrganizationCode != organization || token.OrganizationIdentifier != organizationId)
                {
                    token.ResetID();
                    token.OrganizationCode = organization;
                    token.OrganizationIdentifier = organizationId;
                }
            }
        }

        private static void SetToken(HttpResponse response, HttpSessionState session, CookieToken token)
        {
            if (token == null)
                return;

            if (HttpContext.Current.Request != null && DisableTokenRefresh(HttpContext.Current.Request.RawUrl))
                return;

            token.ValidationKey = CookieToken.CreateValidationKey(token, GetSecret());
            token.Modified = DateTimeOffset.UtcNow.ToString("O");

            if (session != null)
                session[CookieSettings.Name] = token;

            if (!token.IsAuthenticated)
                return;

            var encoder = new CookieTokenEncoder();

            var encrypt = CookieSettings.Encrypt;

            var secret = GetSecret();

            var cookie = new HttpCookie(CookieSettings.Name)
            {
                Domain = ServiceLocator.AppSettings.Partition.Domain,
                Expires = DateTime.UtcNow.AddMinutes(CookieSettings.Lifetime),
                Path = CookieSettings.Path,
                Value = encoder.Serialize(token, encrypt, secret, true),
                SameSite = SameSiteMode.Lax,
                Secure = true,
                HttpOnly = true
            };

            if (cookie.Value != null && cookie.Value.Length > 3072)
            {
                if (DateTimeOffset.UtcNow > _warningLockedUntil)
                {
                    lock (WarningSyncRoot)
                    {
                        if (DateTimeOffset.UtcNow > _warningLockedUntil)
                        {
                            _warningLockedUntil = DateTimeOffset.UtcNow.AddMinutes(5);

                            AppSentry.SentryWarning($"The size of {CookieSettings.Name} cookie has exceeded 3KB");
                        }
                    }
                }
            }

            response.Cookies.Set(cookie);

            SetIdCookie(HttpContext.Current.Request, response, token);
        }

        private static void SetIdCookie(HttpRequest request, HttpResponse response, CookieToken token)
        {
            response.Cookies.Add(new HttpCookie(IdCookieSettings.Name)
            {
                Domain = ServiceLocator.AppSettings.Partition.Domain,
                Expires = DateTime.UtcNow.AddMinutes(CookieSettings.Lifetime),
                Path = CookieSettings.Path,
                Value = token.ID.ToString().ToLower(),
                SameSite = SameSiteMode.Lax,
                Secure = true,
                HttpOnly = true
            });
        }

        private static bool DisableTokenRefresh(string rawUrl)
        {
            if (StringHelper.StartsWith(rawUrl, SignOut.GetUrl()))
                return true;

            return false;
        }

        #endregion

        #region Methods (active sessions)

        private static void ActiveTokens_ItemAdded(object sender, MemoryCacheArgs<Guid, CookieToken> args)
        {
            if (!string.IsNullOrEmpty(args.Data?.Session))
                _activeSessions.AddOrUpdate(args.Data.Session, s => args.Data, (s, d) => args.Data);
        }

        private static void ActiveTokens_ItemRemoved(object sender, MemoryCacheArgs<Guid, CookieToken> args)
        {
            if (string.IsNullOrEmpty(args.Data?.Session))
                return;

            if (!_activeSessions.TryRemove(args.Data.Session, out _))
                return;
        }

        public static CookieToken[] GetActiveTokens() =>
            _activeTokens.GetAll().OrderByDescending(x => x.IsActive()).ThenByDescending(x => x.Modified).ThenBy(x => x.UserEmail).ToArray();

        public static bool IsActive(CookieToken token) => IsTokenActive(token.ID);

        public static bool IsTokenActive(Guid id) => _activeTokens.Exists(id);

        public static bool IsSessionActive(string id) => _activeSessions.ContainsKey(id);

        public static CookieToken FindActiveToken(Guid tokenId) =>
            _activeTokens.Get(tokenId);

        public static CookieToken FindActiveToken(string sessionId) =>
            !string.IsNullOrEmpty(sessionId) && _activeSessions.TryGetValue(sessionId, out var value) ? value : null;

        private static void AddActiveToken(CookieToken token, int timeout)
        {
            if (token == null || token.UserEmail.IsEmpty())
                return;

            RemoveActiveToken(token, false);

            _activeTokens.Add(token.ID, token, timeout * 60, true);
        }

        private static void RemoveActiveToken(CookieToken token, bool isExpired)
        {
            if (token == null)
                return;

            _activeTokens.Remove(token.ID);

            if (token.IsExpired() || isExpired || !token.TargetsEnvironment(Environment))
                StopUserSession(token.Session);
        }

        private static void StopUserSession(string sessionCode)
        {
            var session = TUserSessionSearch.Bind(x => x, x => x.SessionCode == sessionCode).OrderByDescending(x => x.SessionStarted).FirstOrDefault();
            if (session == null)
                return;

            session.SessionStopped = DateTimeOffset.UtcNow;
            session.SessionMinutes = (int)(session.SessionStopped - session.SessionStarted).Value.TotalMinutes;
            TUserSessionStore.Update(session);
        }

        #endregion

        #region Methods (token serialization and validation)

        private static string GetSecret()
            => ServiceLocator.AppSettings.Security.Cookie.Secret;

        private static bool IsValid(CookieToken token)
        {
            return token != null && token.ValidationKey == CookieToken.CreateValidationKey(token, GetSecret());
        }

        #endregion
    }
}