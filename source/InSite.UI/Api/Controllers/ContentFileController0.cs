using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

using Humanizer;

using InSite.Api.Settings;

using Newtonsoft.Json;

using Shift.Common.File;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Api.Controllers
{
    [DisplayName("Content")]
    public class FilesLegacyController : ApiBaseController
    {
        [HttpPost]
        [Route("api/assets/files/legacyupload")]
        [ApiAuthenticationRequirement(ApiAuthenticationType.Request)]
        public HttpResponseMessage LegacyUpload(string container, string id, string surveySession = null)
        {
            var context = HttpContext.Current;

            if (!context.Request.IsAuthenticated && !ValidateSurveySession(surveySession))
                return JsonError(HttpStatusCode.Forbidden);

            HttpFileCollection files = context.Request.Files;

            if (files.Count == 0)
                return JsonError(HttpStatusCode.BadRequest);

            if (!Guid.TryParse(id, out Guid containerId))
                return JsonError(HttpStatusCode.BadRequest);

            var meta = SaveUploadedFiles(container, containerId, files, User.Identity.Name);

            return JsonSuccess(meta);
        }

        private static bool ValidateSurveySession(string surveySession)
        {
            if (string.IsNullOrEmpty(surveySession) || !Guid.TryParse(surveySession, out var id))
                return false;

            var session = ServiceLocator.SurveySearch.GetResponseSession(id);

            return session != null
                && !string.Equals(session.ResponseSessionStatus, ResponseSessionStatus.Completed.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        private static UploadMetadata SaveUploadedFiles(string container, Guid id, HttpFileCollection files, string user)
        {
            var folder = UploadHelper.CreateTempFolderForUser(ServiceLocator.FilePaths, user);

            var byteSize = 0;
            string last = null;

            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFile file = files[i];
                last = UploadHelper.Sanitize(file.FileName);

                byteSize += file.ContentLength;
                file.SaveAs(Path.Combine(folder, last));
            }

            var meta = CreateMetadata(container, id, System.Web.HttpContext.Current.Request, folder, files.Count, byteSize, last);

            return meta;
        }

        public static UploadMetadata SaveUploadedFile(string container, Guid id, HttpPostedFile file, string user)
        {
            var folder = UploadHelper.CreateTempFolderForUser(ServiceLocator.FilePaths, user);

            var fileName = UploadHelper.Sanitize(file.FileName);
            var byteSize = file.ContentLength;
            file.SaveAs(Path.Combine(folder, fileName));

            var last = fileName;
            var meta = CreateMetadata(container, id, System.Web.HttpContext.Current.Request, folder, 1, byteSize, last);

            var path = ServiceLocator.FilePaths.GetPhysicalPathToShareFolder("Files", "Temp", meta.OrganizationCode, meta.ContainerType.Pluralize(), meta.ContainerIdentifier.ToString());
            UploadHelper.CopyUploadedFiles(meta, path, true);

            return meta;
        }

        private static UploadMetadata CreateMetadata(string container, Guid id, HttpRequest request, string folder, int fileCount, int fileSize, string file)
        {
            var organization = CurrentSessionState.Identity.Organization;
            var user = CurrentSessionState.Identity.User;

            var meta = new UploadMetadata
            {
                ContainerType = container,
                ContainerIdentifier = id,

                FileCount = fileCount,
                FileName = file,
                FilePath = Path.Combine(folder, file),
                FileSize = fileSize.Bytes().Humanize("MB"),
                FileType = Path.GetExtension(file),

                OrganizationCode = organization.Code,
                OrganizationIdentifier = organization.Identifier,

                UploadDate = DateTime.Today.ToString("MMM d, yyyy"),
                UploadFolder = folder,
                UploadUrl = request.RawUrl,
                UploadUrlReferrer = request.UrlReferrer.PathAndQuery,

                UserName = user?.Email,
                UserIdentifier = user?.UserIdentifier ?? Shift.Constant.UserIdentifiers.Someone
            };

            var json = JsonConvert.SerializeObject(meta, Formatting.Indented);
            File.WriteAllText(Path.Combine(folder, UploadHelper.MetadataFileName), json);

            return meta;
        }
    }
}