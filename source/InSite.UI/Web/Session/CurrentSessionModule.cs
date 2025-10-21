using System;
using System.Security.Principal;
using System.Web;

using InSite.Api.Controllers;
using InSite.Common.Web;
using InSite.Domain.Foundations;
using InSite.Persistence;

using Shift.Common;
using Shift.Contract;

namespace InSite
{
    public class CurrentSessionModule : IHttpModule
    {
        public void Dispose() { }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += OnBeginRequest;
            context.AcquireRequestState += OnAcquireRequestState;
        }

        /// <summary>
        /// Returns true if the current HTTP request requires authentication.
        /// </summary>
        private bool IsIdentificationRequired(HttpContext context)
        {
            var request = context.Request;
            return (context.CurrentHandler is System.Web.UI.Page)
                || (StringHelper.StartsWith(request.Path, "/UI/Admin/") && StringHelper.EndsWith(request.Path, ".aspx"))
                || (StringHelper.StartsWith(request.Path, "/UI/Portal/") && StringHelper.EndsWith(request.Path, ".aspx"))
                || StringHelper.StartsWith(request.Path, AssetsController.FilesUrl)
                || StringHelper.StartsWith(request.Path, ClientController.ClientUrl)
                || StringHelper.StartsWith(request.Path, FormsController.FormsUrl)
                ;
        }

        /// <summary>
        /// Returns true if the HTTP context has a usable SessionState.
        /// </summary>
        private bool IsSessionDisabled(HttpContext context)
        {
            return context.Session == null;
        }

        /// <summary>
        /// Retrives and validates the organization code from the subdomain in the URL for the HTTP Request.
        /// </summary>
        private static string GetValidatedOrganizationCode(Uri url)
        {
            var code = UrlHelper.GetOrganizationCode(url);
            if (string.IsNullOrEmpty(code))
                HttpResponseHelper.SendHttp400();

            var organization = OrganizationSearch.Select(code);
            if (organization == null)
                HttpResponseHelper.SendHttp400();

            return code;
        }

        /// <summary>
        /// Returns true if the user and/or organization identity has changed since the last request.
        /// </summary>
        private static bool IsIdentityChanged(CookieToken token, ISecurityFramework identity, string organization)
        {
            // If the user is unknown or the organization is different then identity has changed.
            if (identity?.User == null || !StringHelper.Equals(identity.Organization?.Code, organization))
                return true;

            // If the person's code matches the authentication token then identity has NOT changed.
            if (identity.User.PersonCode != null && StringHelper.Equals(identity.User.PersonCode, token.UserEmail))
                return false;

            // If the user's email matches the authentication token then identity has NOT changed.
            if (StringHelper.Equals(identity.User.Email, token.UserEmail))
                return false;

            // Otherwise, assume the user's identity has changed.
            return true;
        }

        private void OnBeginRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;

            if (HttpRequestHelper.IsStaticAsset(app.Context.Request))
            {
                app.Context.Items["SkipSessionInitialization"] = true;
                return;
            }
        }

        /// <summary>
        /// After this method executes CurrentSessionState.Identity is NEVER a null reference.
        /// </summary>
        private void OnAcquireRequestState(object sender, EventArgs e)
        {
            var context = HttpContext.Current;

            if (context.Items["SkipSessionInitialization"] is bool skip && skip)
                return;

            context.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);

            if (Global.IsRequestIgnored(context))
                return;

            if (!IsIdentificationRequired(context) || IsSessionDisabled(context))
                return;

            Initialize(context, CurrentSessionState.Identity);
        }

        public static void Initialize(HttpContext context, ISecurityFramework identity, bool forceRefresh = false)
        {
            var organization = GetValidatedOrganizationCode(context.Request.Url);
            var token = CookieTokenModule.Current;

            if (forceRefresh || IsIdentityChanged(token, identity, organization))
            {
                CookieTokenModule.ResetCreated();

                identity = CurrentIdentityFactory.CreateCurrentIdentity(token, organization, null);

                if (identity.User != null)
                {
                    SessionHelper.StartSession(identity.Organization.Key, identity.User.UserIdentifier);
                    RecentSessionHelper.Clear();
                }
            }
            else
            {
                token.Language = identity.ChangeLanguage(token.Language);
            }

            CurrentSessionState.Identity = identity ?? throw new NullReferenceException("CurrentSessionState.Identity cannot be null here.");

            var url = context.Request.RawUrl;

            if ((context.Request.IsAuthenticated || identity.IsAuthenticated)
                && !StringHelper.Equals(url, UI.Lobby.SignOut.GetUrl())
                && IsTokenObsolete(token)
                )
            {
                UI.Lobby.SignOut.Redirect("CurrentSessionModule:Initialize", $"Token is obsolete for {url}");

                context.ApplicationInstance.CompleteRequest();
            }
            else
            {
                CurrentIdentityFactory.CheckPageAccess();

                HttpContext.Current.User = identity;
            }
        }

        private static bool IsTokenObsolete(CookieToken token)
        {
            var version = ServiceLocator.AppSettings.Release.Version;

            var versionReleased = ExtractDateTimeFromVersion();

            var tokenCreated = token.ParseCreated();

            var isObsolete = versionReleased >= tokenCreated;

            return isObsolete;
        }

        private static DateTimeOffset ExtractDateTimeFromVersion()
        {
            var assemblyVersion = ServiceLocator.AppSettings.Release.Version;

            var parts = assemblyVersion.HasValue() ? assemblyVersion.Split(new[] { '.' }) : null;
            var datePart = parts.Length == 4 ? parts[2] : null;
            var timePart = parts.Length == 4 ? parts[3] : null;

            if (parts != null
                && int.TryParse(datePart, out var _)
                && int.TryParse(timePart, out var _)
                && (datePart.Length == 3 || datePart.Length == 4)
                && timePart.Length <= 4
                )
            {
                string monthText, dayText, hourText, minuteText;

                if (datePart.Length == 3)
                {
                    monthText = datePart.Substring(0, 1);
                    dayText = datePart.Substring(1);
                }
                else
                {
                    monthText = datePart.Substring(0, 2);
                    dayText = datePart.Substring(2);
                }

                if (timePart.Length >= 3)
                {
                    hourText = timePart.Substring(0, timePart.Length - 2);
                    minuteText = timePart.Substring(timePart.Length - 2);
                }
                else
                {
                    hourText = "0";
                    minuteText = timePart;
                }

                var year = DateTimeOffset.UtcNow.Year;
                var month = int.Parse(monthText);
                var day = int.Parse(dayText);
                var hour = int.Parse(hourText);
                var minute = int.Parse(minuteText);

                if (month == 13)
                    month = 1;
                else if (month > DateTimeOffset.UtcNow.Month)
                    year--;

                try
                {
                    return new DateTimeOffset(year, month, day, hour, minute, 0, TimeSpan.Zero);
                }
                catch
                {
                    return DateTimeOffset.MinValue;
                }
            }

            return DateTimeOffset.MinValue;
        }
    }
}