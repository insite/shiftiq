using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

using InSite.Common.Web;
using InSite.Common.Web.Infrastructure;
using InSite.Domain.Organizations;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Web.Persistence
{
    public class DownloadFile : IHttpHandler
    {
        #region Classes

        protected class ETagCollection
        {
            #region Properties

            public bool IsEmpty => _tags.Count == 0;

            #endregion

            #region Fields

            private readonly List<ETag> _tags;

            #endregion

            #region Construction

            public ETagCollection(bool isAny = false)
            {
                _tags = isAny ? null : new List<ETag>();
            }

            #endregion

            #region Methods

            internal void Add(ETag eTag)
            {
                _tags.Add(eTag);
            }

            internal bool Contains(string tag)
            {
                if (_tags == null)
                    return !string.IsNullOrEmpty(tag);

                return _tags.Any(x => x.Value == tag);
            }

            #endregion
        }

        protected class ETag
        {
            #region Properties

            public string Value { get; private set; }
            public bool IsWeak { get; private set; }

            #endregion

            #region Construction

            private ETag()
            {

            }

            #endregion

            #region Initialization

            public static ETagCollection Parse(string input)
            {
                if (string.IsNullOrEmpty(input))
                    return null;

                if (input == "*")
                    return new ETagCollection(true);

                var result = new ETagCollection();
                var index = 0;
                var eTag = new ETag();
                var isError = false;

                while (!isError && index < input.Length)
                {
                    var ch = input[index];

                    if (ch == ' ')
                    {
                        index++;
                    }
                    else if (ch == '"')
                    {
                        if (eTag != null)
                        {
                            eTag.Value = ReadValue(input, ref index);

                            if (!string.IsNullOrEmpty(eTag.Value))
                            {
                                result.Add(eTag);

                                eTag = null;

                                index++;
                            }
                            else
                                isError = true;
                        }
                        else
                            isError = true;
                    }
                    else if (ch == ',')
                    {
                        if (eTag == null)
                        {
                            eTag = new ETag();

                            index++;
                        }
                        else
                            isError = true;
                    }
                    else
                    {
                        var prefix = ReadPrefix(input, ref index);
                        if (prefix == "W/" && eTag != null)
                            eTag.IsWeak = true;
                        else
                            isError = true;
                    }
                }

                return isError || result.IsEmpty ? null : result;
            }

            private static string ReadPrefix(string input, ref int index)
            {
                var result = new StringBuilder();

                while (index < input.Length)
                {
                    var ch = input[index];
                    if (ch == '"')
                        break;

                    result.Append(ch);

                    index++;
                }

                return result.ToString();
            }

            private static string ReadValue(string input, ref int index)
            {
                var result = new StringBuilder();

                while (true)
                {
                    index++;

                    if (index == input.Length)
                        return null;

                    var ch = input[index];
                    if (ch == '"')
                        break;

                    result.Append(ch);
                }

                return result.ToString();
            }

            #endregion
        }

        #endregion

        public bool IsReusable => true;

        protected FileProvider FileProvider => FileHelper.Provider;

        public void ProcessRequest(HttpContext http)
        {
            var organization = OrganizationSearch.Select(UrlHelper.GetOrganizationCode(http.Request.Url));
            var path = GetPath(http);

            if (IsAuthorized(organization, path))
                ProcessRequest(new[] { organization.OrganizationIdentifier, Guid.Empty }, path, http);
            else
                OnFileNotFound(http.Response);
        }

        protected void ProcessRequest(Guid[] organizations, string path, HttpContext context)
        {
            if (TryProcessRequest1(context))
                return;

            if (TryProcessRequest2(context))
                return;

            FileDescriptor descriptor = null;

            foreach (var organization in organizations)
            {
                descriptor = GetDescriptor(organization, path);
                if (descriptor != null)
                    break;
            }

            if (descriptor == null)
                OnFileNotFound(context.Response);
            else if (!IsModified(context.Request, descriptor))
                OnFileNotModified(context.Response, descriptor);
            else if (!IsAuthorized(context, descriptor))
                OnFileAccessDenied(context.Response, descriptor);
            else if (OnFileTransmit(context, descriptor))
                OnSetupCacheSettings(context.Response.Cache, descriptor);
            else
                OnFileNotFound(context.Response);
        }

        private bool TryProcessRequest1(HttpContext http)
        {
            if (!http.Request.RawUrl.StartsWith("/in-content/"))
                return false;

            string path = http.Request.QueryString["path"];

            if (path == null)
                path = http.Request["path"];

            var physicalPath = GetPhysicalPath(path);

            if (!string.IsNullOrEmpty(physicalPath))
            {
                using (var stream = Read(physicalPath))
                {
                    if (stream != Stream.Null)
                    {
                        var response = http.Response;

                        response.Clear();
                        response.ClearHeaders();
                        response.ClearContent();

                        response.ContentType = Shift.Common.MimeMapping.GetContentType(physicalPath);

                        response.AddHeader("Content-Length", stream.Length.ToString());

                        stream.CopyTo(response.OutputStream);

                        return true;
                    }
                }
            }

            return false;

            string GetPhysicalPath(string relativePath)
            {
                if (string.IsNullOrEmpty(relativePath))
                    return null;

                if (relativePath.ToLower().StartsWith("users"))
                {
                    var userId = UrlHelper.GetUserId(relativePath);
                    var user = UserSearch.Select(userId);
                    if (user != null)
                    {
                        return Path.Combine(
                            ServiceLocator.FilePaths.GetPhysicalPathToShareFolder("Files", "Library", "Accounts"),
                            Path.Combine(relativePath.Split('/', '\\')));
                    }
                }
                else
                {
                    var code = UrlHelper.GetOrganizationCode(HttpContext.Current.Request.Url);
                    var organization = OrganizationSearch.Select(code);
                    if (organization != null)
                    {
                        return Path.Combine(
                            ServiceLocator.FilePaths.GetPhysicalPathToShareFolder("Files", "Tenants", organization.Code),
                            Path.Combine(relativePath.Split('/', '\\')));
                    }
                }

                return null;
            }

            Stream Read(string physical)
            {
                if (File.Exists(physical))
                    return new FileStream(physical, FileMode.Open, FileAccess.Read, FileShare.Read);

                return Stream.Null;
            }
        }

        private bool TryProcessRequest2(HttpContext http)
        {
            if (!http.Request.RawUrl.StartsWith("/cmds/uploads/"))
                return false;

            var request = http.Request;
            var response = http.Response;

            response.Clear();
            response.ClearHeaders();
            response.ClearContent();

            Cmds.Infrastructure.CmdsUploadInfo info = null;

            string id = http.Request.QueryString["id"];

            string name = http.Request.QueryString["name"];

            info = Cmds.Infrastructure.CmdsUploadProvider.Current.GetInfo(id, name);

            if (info == null)
                OnFileNotFound(response);
            else if (!EvalCondition(request, info))
                OnFileNotModified(response);
            else if (OnFileTransmit(response, info))
            {
                if (request["download"] == "1")
                    response.ContentType = "application/octet-stream";

                OnSetupCacheSettings(response, info);
            }
            else
                OnFileNotFound(response);

            return true;

            bool EvalCondition(HttpRequest r, Cmds.Infrastructure.CmdsUploadInfo i)
            {
                var clientDate = ParseDate(r.Headers["If-Modified-Since"]);
                if (clientDate.HasValue)
                    return i.Timestamp.UtcDateTime > clientDate.Value;

                return true;
            }

            void OnFileNotModified(HttpResponse r)
            {
                r.StatusCode = (int)HttpStatusCode.NotModified;
                r.StatusDescription = HttpStatusCode.NotModified.GetStatusDescription();
                r.End();
            }

            bool OnFileTransmit(HttpResponse r, Cmds.Infrastructure.CmdsUploadInfo i)
            {
                using (var stream = Cmds.Infrastructure.CmdsUploadProvider.Current.Read(i))
                {
                    if (stream == Stream.Null)
                        return false;

                    r.ContentType = i.MimeType;

                    r.AddHeader("Content-Length", stream.Length.ToString());
                    r.AddHeader("Last-Modified", $"{i.Timestamp.UtcDateTime:ddd, dd MMM yyyy HH:mm:ss} GMT");

                    stream.CopyTo(r.OutputStream);

                    return true;
                }
            }

            void OnSetupCacheSettings(HttpResponse r, Cmds.Infrastructure.CmdsUploadInfo i)
            {
                if (i.MimeType.StartsWith("application/"))
                {
                    HttpResponseHelper.SetNoCache();
                }
                else
                {
                    r.Cache.SetCacheability(HttpCacheability.Private);
                }
            }
        }

        protected virtual FileDescriptor GetDescriptor(Guid organization, string path) =>
            FileProvider.GetDescriptor(organization, path);

        private string GetPath(HttpContext http)
        {
            string path = http.Request.QueryString["path"];

            if (path == null)
                path = http.Request["path"];

            if (path == null)
            {
                var id = http.Request.QueryString["id"];
                var name = http.Request.QueryString["name"];
                if (id != null && name != null)
                    return $"{id}/{name}";
            }

            if (path != null && !path.StartsWith("/"))
                path = "/" + path;

            return path;
        }

        protected bool OnFileTransmit(HttpContext context, FileDescriptor descriptor)
        {
            var isValid = TryOnFileTransmit(context, descriptor);

            if (isValid && context.Request["download"] == "1")
                context.Response.ContentType = "application/octet-stream";

            return isValid;
        }

        protected bool TryOnFileTransmit(HttpContext context, FileDescriptor descriptor)
        {
            using (var stream = FileProvider.Read(descriptor))
            {
                if (stream == Stream.Null)
                    return false;

                var response = context.Response;

                response.Clear();
                response.ClearHeaders();
                response.ClearContent();

                response.ContentType = descriptor.MimeType;

                response.AddHeader("Content-Length", stream.Length.ToString());
                response.AddHeader("ETag", $"\"{descriptor.Fingerprint}\"");
                response.AddHeader("Last-Modified", $"{descriptor.Posted.UtcDateTime:ddd, dd MMM yyyy HH:mm:ss} GMT");

                stream.CopyTo(response.OutputStream);

                return true;
            }
        }

        protected virtual void OnSetupCacheSettings(HttpCachePolicy cache, FileDescriptor descriptor)
        {
            if (descriptor.MimeType.StartsWith("application/"))
            {
                HttpResponseHelper.SetNoCache();
            }
            else
            {
                cache.SetCacheability(HttpCacheability.Private);
                cache.SetMaxAge(new TimeSpan(0, 2, 0));
            }
        }

        protected virtual void OnFileNotFound(HttpResponse response)
        {
            response.StatusCode = (int)HttpStatusCode.NotFound;
            response.StatusDescription = HttpStatusCode.NotFound.GetStatusDescription();
        }

        protected virtual void OnFileNotModified(HttpResponse response, FileDescriptor descriptor)
        {
            response.StatusCode = (int)HttpStatusCode.NotModified;
            response.StatusDescription = HttpStatusCode.NotModified.GetStatusDescription();
            response.AddHeader("ETag", $"\"{descriptor.Fingerprint}\"");
        }

        protected virtual void OnFileAccessDenied(HttpResponse response, FileDescriptor descriptor)
        {
            response.StatusCode = (int)HttpStatusCode.Forbidden;
            response.StatusDescription = HttpStatusCode.Forbidden.GetStatusDescription();
        }

        protected virtual bool IsModified(HttpRequest request, FileDescriptor descriptor)
        {
            var ifNoneMatch = ETag.Parse(request.Headers["If-None-Match"]);
            if (ifNoneMatch != null)
                return !ifNoneMatch.Contains(descriptor.Fingerprint);

            var clientDate = ParseDate(request.Headers["If-Modified-Since"]);
            if (clientDate.HasValue)
                return descriptor.Posted.UtcDateTime > clientDate.Value;

            return true;
        }

        private static DateTime? ParseDate(string value)
        {
            return DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
                ? date.UtcDateTime
                : (DateTime?)null;
        }

        private bool IsAuthorized(OrganizationState organization, string path)
        {
            return organization != null && !string.IsNullOrEmpty(path);
        }

        private bool IsAuthorized(HttpContext context, FileDescriptor descriptor)
        {
            if (descriptor.Access == FileAccessType.Public || descriptor.Access == FileAccessType.Platform)
                return true;

            if (descriptor.Access == FileAccessType.Tenant)
                return TryGetOrganization(out OrganizationState organization) && descriptor.OrganizationIdentifier == organization.OrganizationIdentifier;

            var token = CookieTokenModule.Current;
            if (token == null || string.IsNullOrEmpty(token.UserEmail) || !CookieTokenModule.IsActive(token))
                return false;

            return true;

            bool TryGetOrganization(out OrganizationState organization)
            {
                var code = UrlHelper.GetOrganizationCode(context.Request.Url);

                organization = OrganizationSearch.Select(code);

                return organization != null;
            }
        }
    }
}