using System.Net;
using System.Web;

using Shift.Common;

namespace InSite.Web.Persistence
{
    public class AppStatus : IHttpHandler
    {
        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            var request = context.Request;

            var path = context.Request.RawUrl;

            if (StringHelper.EqualsAny(path, new[] { "/ui/health", "/ui/status" }))
                Health(request, context.Response);

            else if (StringHelper.EqualsAny(path, new[] { "/ui/version" }))
                Version(request, context.Response);

            else
                NotFound(context.Response);
        }

        private static void Health(HttpRequest request, HttpResponse response)
        {
            var assembly = typeof(AppStatus).Assembly.GetName();

            response.StatusCode = (int)HttpStatusCode.OK;

            response.Write($"{assembly.Name} version {assembly.Version} is online. The {ServiceLocator.AppSettings.Environment.Name} environment says hello to you at {request.UserHostAddress}.");
        }

        private static void Version(HttpRequest request, HttpResponse response)
        {
            var version = typeof(AppStatus).Assembly.GetName().Version;

            response.StatusCode = (int)HttpStatusCode.OK;

            response.Write(version.ToString());
        }

        private static void NotFound(HttpResponse response)
        {
            response.StatusCode = (int)HttpStatusCode.NotFound;

            response.Write("Not Found");
        }
    }
}