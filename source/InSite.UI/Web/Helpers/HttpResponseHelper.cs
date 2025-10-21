using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

using Shift.Common;

namespace InSite.Common.Web
{
    public static class HttpResponseHelper
    {
        private static class Url
        {
            public const string Error400 = "/400";
            public const string Error403 = "/403";
            public const string Error404 = "/404";
            public const string Error410 = "/410";
            public const string Error500 = "/500";
        }

        public static string SignOutUrl { get; set; }

        public static string BuildUrl(string path, string parameters)
        {
            return parameters.IsNotEmpty()
                ? path.Contains("?") ? path + "&" + parameters : path + "?" + parameters
                : path;
        }

        /// <summary>
        /// This method creates an HTML form with hidden fields and submits a POST request to a URL.
        /// </summary>
        public static string CreatePostForm(string url, NameValueCollection data)
        {
            const string id = "AutoPostForm";

            var sb = new StringBuilder();
            sb.AppendFormat($"<form id='{id}' name='{id}' action='{url}' method='POST'>");
            foreach (string key in data)
            {
                sb.AppendFormat($"<input type='hidden' name='{key}' value='{data[key]}'>");
            }
            sb.Append("</form>");

            var script = new StringBuilder();
            script.Append("<script language='javascript'>");
            script.AppendFormat($"var v{id} = document.{id};");
            script.AppendFormat($"v{id}.submit();");
            script.Append("</script>");

            return sb + script.ToString();
        }

        public static void Redirect(string url, string parameters)
            => Redirect(BuildUrl(url, parameters));

        public static void Redirect(WebUrl url, bool endResponse) => Redirect(url.ToString(), endResponse);

        public static void Redirect(string url, bool endResponse) => HttpContext.Current.Response.Redirect(url, endResponse);

        public static void Redirect(WebUrl url) => Redirect(url.ToString());

        public static void Redirect(string url)
        {
            HttpContext.Current.Response.Redirect(url);
        }

        public static void SendFile(this HttpResponse response, string name, string ext, byte[] data, string mime = null, bool sanitize = true)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (string.IsNullOrEmpty(ext))
                throw new ArgumentNullException(nameof(ext));

            SendFile(response, $"{name}.{ext}", data, mime, sanitize);
        }

        public static void SendFile(this HttpResponse response, string filename, byte[] data, string mime = null, bool sanitize = true)
        {
            if (data.IsEmpty())
                throw new ArgumentNullException(nameof(data));

            SendFile(response, filename, s => s.Write(data, 0, data.Length), data.Length, mime, sanitize);
        }

        public static void SendFile(this HttpResponse response, string filename, Action<Stream> write, int? length = null, string mime = null, bool sanitizeFilename = true)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException(nameof(filename));

            if (sanitizeFilename)
                filename = StringHelper.Sanitize(Path.GetFileNameWithoutExtension(filename), '-') + Path.GetExtension(filename);

            if (string.IsNullOrEmpty(mime))
                mime = Shift.Common.MimeMapping.GetContentType(filename);

            response.Clear();
            response.ClearHeaders();
            response.ClearContent();

            response.AddHeader("Content-Disposition", $"attachment; filename={filename}");

            if (length.HasValue)
                response.AddHeader("Content-Length", length.Value.ToString());

            response.ContentType = mime;

            write(response.OutputStream);

            response.End();
        }

        public static void SendFile(this HttpResponse response, string filename, Stream stream, int? length = null, string mime = null, bool sanitizeFilename = true)
        {
            try
            {
                if (string.IsNullOrEmpty(filename))
                    throw new ArgumentNullException(nameof(filename));

                if (sanitizeFilename)
                    filename = StringHelper.Sanitize(Path.GetFileNameWithoutExtension(filename), '-') + Path.GetExtension(filename);

                if (string.IsNullOrEmpty(mime))
                    mime = Shift.Common.MimeMapping.GetContentType(filename);

                response.Clear();
                response.ClearHeaders();
                response.ClearContent();

                response.AddHeader("Content-Disposition", $"attachment; filename={filename}");

                if (length.HasValue)
                    response.AddHeader("Content-Length", length.Value.ToString());

                response.ContentType = mime;

                stream.CopyTo(response.OutputStream);

                response.End();
            }
            finally
            {
                stream.Close();
            }
        }

        public static string GetHelpUrl(HttpStatusCode code)
        {
            if (code == HttpStatusCode.Forbidden)
                return Url.Error403;

            return "/";
        }

        public static void SendFile(this HttpResponse response, string filename, string path, string mime = null)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            var info = new FileInfo(path);
            if (!info.Exists)
                throw new ApplicationError("File not found: " + path);
            else if (info.Length == 0)
                throw new ApplicationError("File is empty: " + path);

            if (string.IsNullOrEmpty(filename))
                filename = info.Name;

            filename = StringHelper.Sanitize(Path.GetFileNameWithoutExtension(filename), '-') + Path.GetExtension(filename);

            response.Clear();
            response.ClearHeaders();
            response.ClearContent();

            response.AddHeader("Content-Disposition", $"attachment; filename={filename}");
            response.AddHeader("Content-Length", info.Length.ToString());
            response.ContentType = string.IsNullOrEmpty(mime) ? Shift.Common.MimeMapping.GetContentType(filename) : mime;

            response.WriteFile(path);
            response.End();
        }

        public static void SendHttp(HttpStatusCode status)
            => SendHttp(HttpContext.Current.Response, status);

        public static void SendHttp(HttpResponse response, HttpStatusCode status)
        {
            response.Clear();
            response.ClearHeaders();
            response.ClearContent();

            response.StatusCode = (int)status;
            response.StatusDescription = status.GetStatusDescription();
            response.StatusDescription = status.GetStatusDescription();
            response.End();
        }

        public static void SendHttp400(bool redirect = true)
            => SendHttp400(HttpContext.Current.Response, HttpContext.Current.Request.RawUrl, null, redirect);

        public static void SendHttp400(string message, bool redirect = true)
            => SendHttp400(HttpContext.Current.Response, HttpContext.Current.Request.RawUrl, message, redirect);

        public static void SendHttp400(HttpResponse response, string path, string message, bool redirect = true)
        {
            if (HttpContext.Current.Request.Url.AbsolutePath == Url.Error400)
                return;

            response.Clear();
            response.ClearHeaders();
            response.ClearContent();

            response.StatusCode = (int)HttpStatusCode.BadRequest;
            response.StatusDescription = HttpStatusCode.BadRequest.GetStatusDescription();

            if (redirect)
            {
                var parameters = new NameValueCollection();

                if (path != null)
                    parameters.Add("path", path);

                if (message != null)
                    parameters.Add("message", StringHelper.EncodeBase64Url(message));

                var errorUrl = Url.Error400 + ToQueryString(parameters);

                response.Redirect(errorUrl);
            }

            response.End();
        }

        public static void SendHttp403(bool redirect = true)
            => SendHttp403(HttpContext.Current.Response, HttpContext.Current.Request.RawUrl, null, redirect);

        public static void SendHttp403(HttpResponse response, string path, string message, bool redirect = true)
        {
            if (HttpContext.Current.Request.Url.AbsolutePath == Url.Error403)
                return;

            response.Clear();
            response.ClearHeaders();
            response.ClearContent();

            response.StatusCode = (int)HttpStatusCode.Forbidden;
            response.StatusDescription = HttpStatusCode.Forbidden.GetStatusDescription();

            if (redirect)
            {
                var parameters = new NameValueCollection();

                if (path != null)
                    parameters.Add("path", path);

                if (message != null)
                    parameters.Add("message", StringHelper.EncodeBase64Url(message));

                var errorUrl = Url.Error403 + ToQueryString(parameters);

                response.Redirect(errorUrl);
            }

            response.End();
        }

        public static void SendHttp404(bool redirect = true)
            => SendHttp404(HttpContext.Current.Response, HttpContext.Current.Request.RawUrl, redirect);

        public static void SendHttp404(string path, bool redirect = true)
            => SendHttp404(HttpContext.Current.Response, path, redirect);

        public static void SendHttp404(HttpResponse response, string path, bool redirect = true)
        {
            if (HttpContext.Current.Request.Url.AbsolutePath == Url.Error404 || HttpContext.Current.Request.Url.AbsolutePath == Url.Error410)
                return;

            response.Clear();
            response.ClearHeaders();
            response.ClearContent();

            // Authenticated requests for pages that do not exist need a non-404 response so that heuristics in IP Ban
            // can be improved. Authenticated requests are (generally) more trust-worthy than unauthenticated requests,
            // and this needs to be taken into account when automatically blocking IP addresses when there is a
            // sequence of too many page-not-found errors.

            if (HttpContext.Current.Request.IsAuthenticated)
            {
                response.StatusCode = (int)HttpStatusCode.Gone;
                response.StatusDescription = HttpStatusCode.Gone.GetStatusDescription();
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.StatusDescription = HttpStatusCode.NotFound.GetStatusDescription();
            }

            if (redirect)
            {
                var errorUrl = HttpContext.Current.Request.IsAuthenticated
                    ? Url.Error410
                    : Url.Error404;

                if (path != null)
                    errorUrl += "?path=" + path;

                response.Redirect(errorUrl);
            }

            response.End();
        }

        public static void SetNoCache()
        {
            var response = HttpContext.Current.Response;

            response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.Cache.SetNoStore();
        }

        public static string ToQueryString(NameValueCollection collection)
        {
            if (collection.Count == 0)
                return string.Empty;

            var array = (from key in collection.AllKeys
                         from value in collection.GetValues(key)
                         select string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value))).ToArray();

            return "?" + string.Join("&", array);
        }
    }
}
