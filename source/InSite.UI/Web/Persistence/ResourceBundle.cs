using System;
using System.Net;
using System.Web;

using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.Web.Persistence
{
    public class ResourceBundle : IHttpHandler
    {
        public const string Path = "/ResourceBundle.axd";

        public bool IsReusable => true;

        public void ProcessRequest(HttpContext context)
        {
            var name = context.Request["r"];
            if (name.IsEmpty())
            {
                FileNotFound();
                return;
            }

            var file = ResourceHelper.GetBundleFile(name);
            if (file == null)
            {
                FileNotFound();
                return;
            }

            var response = context.Response;

            response.Clear();
            response.ClearHeaders();
            response.ClearContent();

            var cache = context.Response.Cache;
            cache.SetCacheability(HttpCacheability.Public);
            cache.VaryByParams["r"] = true;
            cache.SetOmitVaryStar(omit: true);
            cache.SetExpires(DateTime.Now.AddDays(365));
            cache.SetValidUntilExpires(validUntilExpires: true);
            cache.SetLastModified(file.Timestamp);

            response.ContentType = file.Type;

            response.BinaryWrite(file.Body);

            response.Flush();

            void FileNotFound()
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                context.Response.StatusDescription = HttpStatusCode.NotFound.GetStatusDescription();
            }
        }
    }
}