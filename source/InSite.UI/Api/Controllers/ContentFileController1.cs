using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

using InSite.Api.Models;
using InSite.Api.Settings;
using InSite.Common.Web;
using InSite.Common.Web.Infrastructure;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Json;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Api.Controllers
{
    [DisplayName("Content")]
    [RoutePrefix("api/files")]
    public partial class FilesController : ApiBaseController
    {
        [HttpGet]
        [Route("list")]
        public HttpResponseMessage FileList(string path, string type, string name = null, DateTimeOffset? since = null, DateTimeOffset? before = null)
        {
            JsonResult result = null;

            try
            {
                if (path.IsEmpty() || !path.StartsWith("/"))
                    path = "/";

                var root = new FolderModel<JsonFileModel>(path);

                var pathFilter = root.Path;
                if (!pathFilter.EndsWith("/"))
                    pathFilter += '/';

                var files = UploadSearch.Bind(
                    x => new JsonFileModel
                    {
                        Path = x.NavigateUrl,
                        Date = x.Uploaded,
                        Size = x.ContentSize ?? 0
                    }, x => x.UploadType == UploadType.InSiteFile
                         && x.ContainerType == UploadContainerType.Oganization
                         && x.OrganizationIdentifier == CurrentOrganization.OrganizationIdentifier
                         && x.NavigateUrl.StartsWith(pathFilter));

                foreach (var file in files)
                    root.Add(file);

                result = JsonFileListResult.Create(root, type, name, since, before);
            }
            catch (ApplicationError ex)
            {
                result = new JsonErrorResult(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                result = new JsonErrorResult("Access Denied");
            }
            catch (Exception ex)
            {
                result = new JsonErrorResult(ex.ToString());
            }

            return JsonSuccess(result);
        }

        [HttpGet]
        [Route("typelist")]
        public HttpResponseMessage FileTypeList()
        {
            var result = new JsonFileTypeListResult
            {
                Types = UploadSearch.SelectFileTypes(CurrentOrganization.OrganizationIdentifier)
            };

            return JsonSuccess(result);
        }

        [HttpPost]
        [Route("upload")]
        public HttpResponseMessage FileUpload()
        {
            if (string.IsNullOrEmpty(CookieTokenModule.Current.OrganizationCode))
                return JsonError("Unexpected error happened. Refresh the page", HttpStatusCode.InternalServerError);

            var request = HttpContext.Current.Request;
            var path = request["path"];

            var organizationId = CurrentOrganization.OrganizationIdentifier;
            var isValid = request.Files.Count > 0
                          && !string.IsNullOrEmpty(path)
                          && path.StartsWith("/") && path.EndsWith("/");

            if (isValid)
                for (var i = 0; i < request.Files.Count; i++)
                {
                    var file = request.Files[i];
                    if (file.ContentLength == 0)
                        continue;

                    var fileName = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
                    var fileExtension = System.IO.Path.GetExtension(file.FileName);

                    var resultFileName = $"{fileName}{fileExtension}";

                    for (var j = 1; ; j++)
                    {
                        if (!UploadSearch.ExistsByOrganizationIdentifier(organizationId, path + resultFileName))
                            break;

                        resultFileName = $"{fileName} ({j}){fileExtension}";
                    }

                    try
                    {
                        FileHelper.Provider.Save(organizationId, path + resultFileName, file.InputStream, CurrentUser.UserIdentifier);
                    }
                    catch (FileStorage.MaxFileSizeExceededException ex)
                    {
                        return JsonError(ex.Message, HttpStatusCode.BadRequest);
                    }
                }

            return JsonSuccess("OK");
        }
    }
}