using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

using InSite.Api.Settings;
using InSite.Application.Files.Read;

using Shift.Common;
using Shift.Common.File;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Api.Controllers
{
    [DisplayName("Content")]
    public class AssetsController : ApiBaseController
    {
        public const string FilesUrl = "/api/assets/files";

        private static readonly TimeSpan PublicFileMaxAge = TimeSpan.FromHours(1);

        [HttpGet]
        [Route("api/assets/files/{fileIdentifier}/{fileName}")]
        [AllowAnonymous]
        public HttpResponseMessage Get(Guid fileIdentifier, string fileName, string download = null)
        {
            var (status, file) = ServiceLocator.StorageService.GetFileAndAuthorize(CurrentSessionState.Identity, fileIdentifier);

            switch (status)
            {
                case FileGrantStatus.NoFile:
                    return JsonError(new { Message = "No File" }, HttpStatusCode.NotFound);
                case FileGrantStatus.Denied:
                    return JsonError(new { Message = "Unauthorized" }, HttpStatusCode.Unauthorized);
                default:
                    break;
            }

            if (!string.Equals(file.FileName, fileName, StringComparison.OrdinalIgnoreCase)
                || !ServiceLocator.StorageService.IsRemoteFilePathValid(file)
                )
            {
                return JsonError(new { Message = "No File" }, HttpStatusCode.NotFound);
            }

            if (Request.Headers.IfModifiedSince.HasValue && Request.Headers.IfModifiedSince >= file.Uploaded.AddSeconds(-1))
            {
                var response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotModified,
                    RequestMessage = Request
                };

                SetFileCacheControl(response, file);

                return response;
            }

            return SendFile(fileIdentifier, download == "1");
        }

        [HttpPost]
        [Route("api/assets/files")]
        [AllowAnonymous]
        public HttpResponseMessage UploadTempFile(string surveySession = null)
        {
            if (CurrentSessionState.Identity?.Organization == null
                || (!CurrentSessionState.Identity.IsAuthenticated
                    && !ValidateSurveySession(surveySession)
                    )
                )
            {
                return JsonError(new { message = "Unauthorized" }, HttpStatusCode.Unauthorized);
            }

            var files = HttpContext.Current.Request.Files;
            if (files.Count == 0)
                return JsonError(new { message = "No Files" }, HttpStatusCode.BadRequest);

            var userId = CurrentSessionState.Identity.User?.Identifier ?? UserIdentifiers.Someone;
            var organizationId = CurrentSessionState.Identity.Organization.Identifier;
            var result = new List<UploadFileInfo>();

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];

                var model = ServiceLocator.StorageService.Create(
                    file.InputStream,
                    file.FileName,
                    organizationId,
                    userId,
                    ObjectIdentifiers.Temporary,
                    FileObjectType.Temporary,
                    new FileProperties { DocumentName = file.FileName },
                    null
                );

                result.Add(new UploadFileInfo
                {
                    FileIdentifier = model.FileIdentifier,
                    FileName = model.Properties.DocumentName,
                    FileSize = model.FileSize
                });
            }

            return JsonSuccess(result);
        }

        [HttpPost]
        [Route("api/assets/files-timers/elapse")]
        [ApiAuthenticationRequirement(ApiAuthenticationType.Jwt)]
        public HttpResponseMessage Elapse()
        {
            var root = Global.GetRootSentinel();
            var identity = HttpContext.Current.GetIdentity();
            if (identity.User.Identifier != root.Identifier)
                return JsonError("Only the root developer account is permitted to invoke this API method.");

            try
            {
                var fileCount = ServiceLocator.StorageService.DeleteExpiredFiles();
                return JsonSuccess($"{fileCount} expired files deleted");
            }
            catch (Exception ex)
            {
                return JsonError(ex.Message);
            }
        }

        private HttpResponseMessage SendFile(Guid fileIdentifier, bool download)
        {
            FileStorageModel file;
            Stream stream = null;

            try
            {
                (file, stream) = ServiceLocator.StorageService.GetFileStream(fileIdentifier);
            }
            catch (ReadFileStreamFailedException fe) when (fe.InnerException is HttpRequestException)
            {
                return JsonError(new { Message = "The file cannot be read from the remote host" }, HttpStatusCode.NotFound);
            }
            catch (FileNotFoundException)
            {
                file = null;
            }
            catch (DirectoryNotFoundException)
            {
                file = null;
            }

            if (file == null)
                return JsonError(new { Message = "No File" }, HttpStatusCode.NotFound);

            if (file.FileSize == 0)
                stream.Close();

            var response = new HttpResponseMessage
            {
                Content = file.FileSize == 0 ? (HttpContent)new StringContent("") : new StreamContent(stream, file.FileSize),
                StatusCode = HttpStatusCode.OK,
                RequestMessage = Request
            };

            response.Content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            response.Content.Headers.LastModified = file.Uploaded;

            SetFileCacheControl(response, file);

            if (download)
            {
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = file.FileName
                };
            }

            return response;
        }

        private static bool ValidateSurveySession(string surveySession)
        {
            if (string.IsNullOrEmpty(surveySession) || !Guid.TryParse(surveySession, out var id))
                return false;

            var session = ServiceLocator.SurveySearch.GetResponseSession(id);

            return session != null
                && !string.Equals(session.ResponseSessionStatus, ResponseSessionStatus.Completed.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        private static void SetFileCacheControl(HttpResponseMessage response, FileStorageModel file)
        {
            var isPublic = file.Claims.NullIfEmpty() == null;

            if (isPublic)
            {
                response.Headers.CacheControl = new CacheControlHeaderValue
                {
                    Public = true,
                    MaxAge = PublicFileMaxAge
                };
                response.Headers.Pragma.Clear();
            }
            else
            {
                response.Headers.CacheControl = new CacheControlHeaderValue
                {
                    Private = true,
                    NoCache = true
                };
            }
        }
    }
}