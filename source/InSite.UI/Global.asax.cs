using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Routing;

using InSite.Api.Controllers;
using InSite.Application;
using InSite.Common.Web;
using InSite.Domain.Foundations;
using InSite.Persistence;
using InSite.Web.Helpers;
using InSite.Web.Routing;

using Shift.Common;
using Shift.Constant;
using Shift.Toolbox;

namespace InSite
{
    public class Global : HttpApplication
    {
        private IDisposable _sentry;

        private static ExceptionHandler[] _exceptionHandlers = null;

        public static bool IsRequestIgnored(HttpContext context)
        {
            if (context == null)
                return true;

            if (IsWebApiRequest(context.Request.Path))
                return false;

            if (HttpRequestHelper.IsStaticAsset(context.Request))
                return true;

            if (HttpRequestHelper.IsEmbeddedResource(context.Request))
                return true;

            return false;
        }

        public static bool IsLocalEnvironment()
            => ServiceLocator.AppSettings.Environment.Name == EnvironmentName.Local;

        public static bool IsWebApiRequest(string url)
            => url.IsNotEmpty() && StringHelper.StartsWith(url, "/api/");

        public static bool IsWebUIRequest(string url)
            => url.IsNotEmpty() && StringHelper.StartsWith(url, "/ui/");

        public static string Translate(string label)
        {
            var token = CookieTokenModule.Current;
            var organization = OrganizationSearch.Select(token.OrganizationCode)?.OrganizationIdentifier ?? Guid.Empty;
            return LabelSearch.GetTranslation(label, token.Language, organization);
        }

        public static AzureAD AzureAD { get; set; }

        public static GoogleLogin GoogleLogin { get; set; }

        protected void Application_Start(object sender, EventArgs e)
        {
            var requirements = new StartupRequirements();

            // Configure dependency injection.

            Web.Injection.DependencyInjector.Startup(requirements);

            var appSettings = ServiceLocator.AppSettings;

            _exceptionHandlers = ExceptionHandler.FromArray(appSettings.Platform.Integrity.ExceptionHandlers);

            var domain = appSettings.Security.Domain;
            var oAuthRedirectUrl = new OAuthRedirectUrl(appSettings.Environment, ServiceLocator.Partition.Slug, domain, appSettings.Integration.OAuthSecret.AzureADSecret.RedirectUrl);

            AzureAD = new AzureAD(appSettings.Integration.OAuthSecret.AzureADSecret, oAuthRedirectUrl);

            GoogleLogin = new GoogleLogin(appSettings.Integration.OAuthSecret.Google, oAuthRedirectUrl);

            InitQuestPDF.Run();

            ServiceLocator.Logger.Information($"Started {DateTime.Now:yyyy-MM-dd}");

            // Initialize logging.
            _sentry = AppSentry.Initialize();
            MessageHelper.LogToSentry = AppSentry.SentryInfo;

            InitializeFileSystem();

            // Initialize static data sets
            OrganizationSearch.Refresh();

            // Configure JSON serialization.
            Shift.Common.Json.JsonSettings.Register();

            // Initialize integrations to third-party API services.
            InitIntegrations();

            // Configure internal frameworks.
            ApplicationContext.Initialize(
                TActionSearch.CreateActionNodes(),
                TActionSearch.CreateActionTree(ActionMapType.Navigation),
                TActionSearch.CreateActionTree(ActionMapType.Permission));

            RouteTable.Routes.RouteExistingFiles = true;

            // Configure HTTP request validation and URL routing.
            RoutingConfiguration.Register(RouteTable.Routes);
            GlobalConfiguration.Configure(RoutingConfiguration.Register);

            // Configure inflections for Humanizer (i.e. non-standard plural forms).
            HumanizerVocabulary.Initialize();

            // Set InSite website specific handlers for entities before/after save
            SqlHelper.SetIdentification(() =>
            {
                var service = ServiceLocator.IdentityService.GetCurrentService();
                return service ?? new ServiceIdentity
                {
                    User = CurrentSessionState.Identity?.User?.Identifier ?? UserIdentifiers.Someone,
                    Organization = CurrentSessionState.Identity?.Organization?.Identifier ?? OrganizationIdentifiers.Global
                };
            });

            AddRemoteShares();

            Shift.Sdk.UI.ControlLinkerCache.Init(typeof(Global).Assembly);
        }

        private void InitializeFileSystem()
        {
            var directories = new List<string>
            {
                ServiceLocator.AppSettings.DataFolderEnterprise,
                ServiceLocator.AppSettings.DataFolderShare,

                System.IO.Path.Combine(ServiceLocator.AppSettings.DataFolderEnterprise, "Aggregates"),
            };

            foreach (var directory in directories)
                ValidateDirectory(directory);
        }

        private void ValidateDirectory(string path)
        {
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
        }

        public static void InitIntegrations()
        {
            var organizations = OrganizationSearch.SelectAll()
                .Where(x => x.AccountStatus == AccountStatus.Opened)
                .ToList();

            var organizationIntegrations = new Dictionary<Guid, OrganizationIntegrations>();

            foreach (var organization in organizations)
            {
                if (organization.Integrations != null)
                    organizationIntegrations.Add(organization.OrganizationIdentifier, organization.Integrations);
            }

            IntegrationClient.Init(
                ServiceLocator.AppSettings.Environment.Name,
                organizationIntegrations
            );
        }

        protected void Application_BeginRequest()
        {
            if (IsRequestIgnored(Context))
                return;

            var monitor = new Web.Optimization.MaintenanceMonitor();
            monitor.FilterRequest(Context);

            AppSentry.StartTransaction(Context);

            var environment = ServiceLocator.AppSettings.Environment;

            var subdomainValidator = new SubdomainValidator();

            if (!subdomainValidator.Validate(environment, Request.Url.Host))
                Context.RewritePath(subdomainValidator.ErrorPageUrl);

            PageDownloadTimer.Start();

            try
            {
                if (Context.Request.Url.LocalPath.StartsWith("/ui/lobby/"))
                {
                    Response.Headers.Add("X-Frame-Options", "DENY");
                    Response.Headers.Remove("X-AspNet-Version");
                }
            }
            catch { }
        }

        protected void Application_PostAuthorizeRequest()
        {
            if (IsRequestIgnored(Context))
                return;

            if (Context.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~" + AssetsController.FilesUrl)
                || Context.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~" + ClientController.ClientUrl)
                || Context.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~" + FormsController.FormsUrl)
                )
            {
                Context.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
            }
        }

        protected void Application_AcquireRequestState()
        {
            if (!Request.IsAuthenticated || IsRequestIgnored(Context))
                return;

            var userId = CurrentSessionState.Identity?.User?.UserIdentifier;
            if (userId.HasValue)
                Api.Controllers.SessionsController.UpdateSession(userId.Value, Session);
        }

        protected void Application_PreRequestHandlerExecute()
        {
            var session = HttpContext.Current.Session;
            if (session == null)
                return;

            // Don't move, the session ID must be obtained before the application writes headers to the response
            // Otherwise the app will throw the following exception on app start during API request:
            //      Session state has created a session id, but cannot save it because the response was
            //      already flushed by the application
            // https://insite.atlassian.net/issues/DEV-9353

            var sessionId = session.SessionID;

            if (ServiceLocator.AppSettings.Environment.Name != EnvironmentName.Local)
                return;

            var cookie = new HttpCookie(GetSessionStateCookieName())
            {
                Value = sessionId,
                SameSite = SameSiteMode.None,
                Secure = true,
                HttpOnly = true,
                Domain = ServiceLocator.AppSettings.Security.Domain
            };

            Response.SetCookie(cookie);
        }

        public static string GetSessionStateCookieName()
        {
            try
            {
                var section = (SessionStateSection)ConfigurationManager.GetSection("system.web/sessionState");

                return section?.CookieName;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving session state cookie name from web.config: {ex.Message}", ex);
            }
        }

        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            AntiForgeryHelper.EnsureCookie();

            if (Response.Cookies.Count > 0)
            {
                var sessionStateCookieName = GetSessionStateCookieName();

                var cookieDomain = ServiceLocator.AppSettings.Security.Domain;

                foreach (string cookieName in Response.Cookies)
                {
                    if (cookieName == sessionStateCookieName)
                    {
                        HttpCookie cookie = Response.Cookies[cookieName];

                        if (cookie != null && cookie.Domain != cookieDomain)
                        {
                            cookie.Domain = cookieDomain;
                        }
                    }
                }
            }
        }

        protected void Application_EndRequest()
        {
            if (IsRequestIgnored(Context))
                return;

            AppSentry.FinishTransaction(Context);
        }

        protected void Application_End()
        {
            // Flushes out events before shutting down.
            _sentry?.Dispose();
        }

        protected void Application_Error()
        {
            var error = Server.GetLastError();

            if (error == null)
                return;

            if (AppSentry.IsInvalidWebResourceRequestException(error))
                return;

            if (AppSentry.IsUrlRoutingException(error, out string path))
            {
                Server.ClearError();
                HttpResponseHelper.SendHttp404(path);
                return;
            }

            var action = HandleException(error);

            if (action.Type == ExceptionActionType.Warning)
            {
                AppSentry.SentryWarning(error);
            }
            else if (action.Type != ExceptionActionType.Ignore)
            {
                AppSentry.CaptureException(error);

                if (AppSentry.IsViewStateException(error))
                {
                    Server.ClearError();

                    if (!Request.IsAuthenticated)
                        HttpResponseHelper.Redirect(ServiceLocator.Urls.LoginUrl, true);
                    else if (Request.UrlReferrer != null)
                        HttpResponseHelper.Redirect(Request.UrlReferrer.PathAndQuery, true);
                    else if (Request.Url != null)
                        HttpResponseHelper.Redirect(Request.Url.PathAndQuery, true);
                }
            }

            if (action.RedirectUrl != null || action.ClearError)
                Server.ClearError();

            if (action.RedirectUrl != null)
                HttpResponseHelper.Redirect(action.RedirectUrl, true);
        }

        public static ExceptionAction HandleException(Exception error)
        {
            return ExceptionHandler.GetAction(error, HttpContext.Current?.Request?.Path, _exceptionHandlers);
        }

        private static void AddRemoteShares()
        {
            if (ServiceLocator.AppSettings.Integration.RemoteShares.Enabled)
                RemoteShare.AddShares(ServiceLocator.AppSettings.Integration.RemoteShares.Items);
        }

        public static Actor GetRootSentinel()
            => ServiceLocator.AppSettings.Security.Sentinels.Root;

        public static SecuritySettings GetSecuritySettings()
            => ServiceLocator.AppSettings.Security;
    }
}