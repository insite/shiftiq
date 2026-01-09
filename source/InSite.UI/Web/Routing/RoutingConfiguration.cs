using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ExceptionHandling;
using System.Web.Routing;

using InSite.Api.Settings;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Web.Routing
{
    public static class RoutingConfiguration
    {
        private static RouteCollection Routes { get; set; }
        private static Dictionary<string, string> RouteNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public static void Register(RouteCollection routes)
        {
            Routes = routes;

            RegisterContacts();
            RegisterCourses();
            RegisterLobby();
            RegisterPortals();
            RegisterPrograms();
            RegisterSites();
            RegisterSurveys();

            // Perform dynamic registration of ASPX web forms defined in the database after all other routes are mapped.

            RegisterActions();
        }

        private static void Register(string name, string url, string aspx)
        {
            if (RouteNames.ContainsKey(name))
                return;

            RouteNames.Add(name, url);
            if (aspx.StartsWith("/"))
                aspx = "~" + aspx;
            Routes.MapPageRoute(name, url, aspx);
        }

        private static void RegisterActions()
        {
            var actions = TActionSearch.Search(x => x.ControllerPath != null && x.ControllerPath.EndsWith(".aspx"));
            foreach (var action in actions)
                Register(action.ActionUrl, action.ActionUrl, action.ControllerPath);
        }

        private static void RegisterContacts()
        {
            Register("Contacts/Groups/Join", "groups/join/{group}", "~/UI/Portal/contacts/groups/join.aspx");
        }

        private static void RegisterCourses()
        {
            Register("LTI", "lti/{*path}", "~/UI/Lobby/LaunchLti.aspx");
            Register($"Portal/Course", "ui/portal/learning/course/{course}", $"~/UI/Portal/Learning/Course.aspx");
            Register($"Portal/Course/Scorm", "ui/portal/integrations/scorm/launch/{activity}", $"~/UI/Portal/integrations/scorm/launch.aspx");
            Register($"Lobby/Scorm/Exit", "ui/lobby/scorm/{activity}/{registration}/exit", $"~/UI/Lobby/ScormExit.aspx");
        }

        private static void RegisterPrograms()
        {
            Register($"Portal/Learning/Program", "ui/portal/learning/program/{slug}", $"~/UI/Portal/Learning/Program.aspx");
            Register($"Portal/Learning/Program/Task", "ui/portal/records/program/{slug}/task/{task}/", $"~/UI/Portal/Learning/Task.aspx");
        }

        private static void RegisterLobby()
        {
            foreach (var form in new[] { "register", "request-access", "reset-password", "signin", "signout", "subscribe", "verify-email" })
            {
                var fileName = form.Replace("-", string.Empty);
                Register($"Lobby/{fileName}", $"ui/lobby/{form}", $"~/UI/Lobby/{fileName}.aspx");
            }

            Register($"Lobby/Integration/Lti/Launch", "ui/lobby/integration/lti/launch", "~/UI/Lobby/Integration/Lti/Launch.aspx");
            Register($"Lobby/Messages/Links", "ui/lobby/messages/links/click", "~/UI/Lobby/Messages/Links/Click.aspx");
            Register($"Lobby/Messages/Links (Legacy MVC)", "mvc/messages/links/click", "~/UI/Lobby/Messages/Links/Click.aspx");
        }

        private static void RegisterPortals()
        {
            Register($"Portals/Custom", "portals/{*path}", $"~/UI/Portal/Content/Sites/Page.aspx");
        }

        private static void RegisterSites()
        {
            Register("Sites/Standby", "standby", "~/UI/Lobby/Standby.aspx");
        }
        
        private static void RegisterSurveys()
        {
            Routes.MapPageRoute($"Portal/Surveys/WithIdentification/Launch", "form/{form}/{user}", $"~/UI/Portal/Workflow/Forms/Submit.aspx", false, null, new RouteValueDictionary { { "form", @"^\d+$" } });
            Routes.MapPageRoute($"Portal/Surveys/WithoutIdentification/Launch", "form/{form}", $"~/UI/Portal/Workflow/Forms/Submit.aspx", false, null, new RouteValueDictionary { { "form", @"^\d+$" } });

            Register($"Portal/Surveys/Search", "ui/portal/workflow/forms/submit/search", $"~/UI/Portal/Workflow/Forms/Search.aspx");
            Register($"Portal/Surveys/WithIdentification", "ui/portal/workflow/forms/submit/{verb}/{form}/{user}", $"~/UI/Portal/Workflow/Forms/Submit.aspx");
            Register($"Portal/Surveys/WithoutIdentification", "ui/portal/workflow/forms/submit/{verb}/{form}", $"~/UI/Portal/Workflow/Forms/Submit.aspx");
            Register($"Portal/Surveys/WithResponseSession", "ui/portal/workflow/forms/submit/{verb}", $"~/UI/Portal/Workflow/Forms/Submit.aspx");
        }

        public static void Register(HttpConfiguration config)
        {
            // Replace the default exception handling.

            config.Services.Replace(typeof(IExceptionHandler), new ApiExceptionHandler());

            // Add filters and formatters.

            config.Filters.Add(new ApiExceptionFilter());
            config.Filters.Add(new ApiValidationFilter());
            config.Formatters.Add(new ApiJsonFormatter());

            // Enable attribute routing.

            config.MapHttpAttributeRoutes();

            // CORS

            EnableCors(config);

            // Map special-case routes.

            var routes = config.Routes;

            // Assessments
            routes.MapHttpRoute("Assessments: Forms", "api/assessments/forms/setelementvalue", new { controller = "Forms", action = "SetElementValue" });

            // Assets
            routes.MapHttpRoute("Assets: Files", "api/assets/files/legacyupload", new { controller = "Files", action = "LegacyUpload" });

            // Registrations
            routes.MapHttpRoute("Registrations: 1", "api/events/registrations/count", new { controller = "Registrations", action = "Count" });
            routes.MapHttpRoute("Registrations: 2", "api/events/registrations/search", new { controller = "Registrations", action = "Search" });

            // Map default routes for the API.
            routes.MapHttpRoute("Default", "api/{controller}/{action}");

            // React
            routes.MapHttpRoute("React", "client/{*path}", new { controller = "React", action = "GetResource" });
        }

        private static void EnableCors(HttpConfiguration config)
        {
            var attr = ServiceLocator.AppSettings.Environment.Name == EnvironmentName.Local
                ? new EnableCorsAttribute("http://localhost:3000", "*", "*") { SupportsCredentials = true }
                : new EnableCorsAttribute("*", "*", "*");

            config.EnableCors(attr);
        }

        public static string PortalCourseUrl(Guid course, Guid? activity = null)
        {
            const string routeName = "Portal/Course";
            var parameters = new RouteValueDictionary
            {
                { "course", course }
            };
            VirtualPathData vpd = RouteTable.Routes.GetVirtualPath(null, routeName, parameters);
            var url = vpd.VirtualPath;

            if (activity.HasValue)
                url += $"?activity={activity}";

            return url;
        }

        public static string PortalProgramUrl(string slug)
        {
            const string routeName = "Portal/Learning/Program";
            var parameters = new RouteValueDictionary
            {
                { "slug", slug }
            };
            VirtualPathData vpd = RouteTable.Routes.GetVirtualPath(null, routeName, parameters);
            var url = vpd.VirtualPath;

            return url;
        }

        public static string PortalProgramUrl(string slug, Guid task)
        {
            const string routeName = "Portal/Learning/Program";
            var parameters = new RouteValueDictionary
            {
                { "slug", slug },
                { "task", task },
            };
            VirtualPathData vpd = RouteTable.Routes.GetVirtualPath(null, routeName, parameters);
            var url = vpd.VirtualPath;

            return url;
        }

        internal static string GetUrl(RouteBase route)
        {
            if (route is Route r)
                return r.Url;
            return null;
        }
    }
}