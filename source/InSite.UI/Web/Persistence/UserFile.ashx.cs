using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Web;
using System.Web.SessionState;

using InSite.Common.Web.Infrastructure;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Handlers
{
    public class UserFile : IHttpHandler, IRequiresSessionState
    {
        public bool IsReusable => true;

        public void ProcessRequest(HttpContext context)
        {
            var fileDate = GetTimestamp(context.Request.QueryString["d"]);

            if (!fileDate.HasValue)
                OnFileNotFound(context.Response);

            if (IsNotModified(context.Request, fileDate.Value))
                OnFileNotModified(context.Response);

            var descriptor = GetFileDescriptor(context.Request, fileDate.Value);
            if (descriptor == null || !OnFileTransmit(context.Response, descriptor))
                OnFileNotFound(context.Response);
        }

        private static void OnFileNotFound(HttpResponse response)
        {
            response.StatusCode = (int)HttpStatusCode.NotFound;
            response.StatusDescription = HttpStatusCode.NotFound.GetStatusDescription();
            response.End();
        }

        private static void OnFileNotModified(HttpResponse response)
        {
            response.StatusCode = (int)HttpStatusCode.NotModified;
            response.StatusDescription = HttpStatusCode.NotModified.GetStatusDescription();
            response.End();
        }

        private static bool OnFileTransmit(HttpResponse response, FileDescriptor descriptor)
        {
            using (var stream = FileHelper.Provider.Read(descriptor))
            {
                if (stream == Stream.Null)
                    return false;

                response.Clear();
                response.ClearHeaders();
                response.ClearContent();

                response.AddHeader("Content-Type", descriptor.MimeType);
                response.AddHeader("Content-Length", stream.Length.ToString());
                response.AddHeader("Last-Modified", $"{descriptor.Posted.UtcDateTime:ddd, dd MMM yyyy HH:mm:ss} GMT");
                response.AddHeader("Content-Disposition", $"attachment;filename={descriptor.Name}");

                response.Cache.SetCacheability(HttpCacheability.Private);

                stream.CopyTo(response.OutputStream);

                return true;
            }
        }

        #region Helpers

        private static FileDescriptor GetFileDescriptor(HttpRequest request, DateTime date)
        {
            FileDescriptor descriptor = null;
            var userId = ValueConverter.ToGuidNullable(request.QueryString["u"]);
            var fileName = request.QueryString["f"];

            if (userId.HasValue && !string.IsNullOrEmpty(fileName))
            {
                var m = FileHelper.UserUpload.GetFileDescriptor(userId.Value, fileName, CurrentSessionState.Identity?.Organization);
                if (m != null && m.Posted.UtcDateTime == date)
                    descriptor = m;
            }

            return descriptor;
        }

        private static bool IsNotModified(HttpRequest request, DateTime date)
        {
            var modifiedSince = request.Headers["If-Modified-Since"];

            return !string.IsNullOrEmpty(modifiedSince) && DateTime.ParseExact(modifiedSince, "r", CultureInfo.InvariantCulture) == date;
        }

        private static DateTime? GetTimestamp(string value)
        {
            try
            {
                var ticks = long.Parse(value);
                return new DateTime(ticks);
            }
            catch
            {

            }

            return null;
        }

        #endregion
    }
}