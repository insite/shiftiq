using System;
using System.IO;
using System.Net;
using System.Web.UI;

using Humanizer;

using InSite.Admin.Assets.Uploads.Models;
using InSite.Common.Web;
using InSite.Common.Web.Infrastructure;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.Json;

using JsonResult = Shift.Common.Json.JsonResult;

namespace InSite.Admin.Assets.Uploads.Forms
{
    public partial class Info : AdminBasePage
    {
        #region Classes

        [JsonObject(MemberSerialization.OptIn)]
        protected class JsonSuccessResult : JsonResult
        {
            #region Construction

            public JsonSuccessResult()
                : base("OK")
            {
            }

            #endregion
        }

        #endregion

        private string Path => Request["path"];

        protected string FileTitle;
        protected string FilePath;

        protected override void OnInit(EventArgs e)
        {
            if (HandleAjaxRequest())
                return;

            if (HandleDownloadRequest())
                return;

            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                Open();
            }
        }

        private void Open()
        {
            var fileInfo = Path.HasValue()
                ? UploadSearch.Bind(Organization.OrganizationIdentifier, Path, x => new
                {
                    x.NavigateUrl,
                    x.Uploader,
                    x.Uploaded,
                    x.ContentSize
                })
                : null;

            if (fileInfo == null)
            {
                HttpResponseHelper.SendHttp404();
                return;
            }

            var path = fileInfo.NavigateUrl;
            var size = fileInfo.ContentSize ?? 0;
            var uploader = fileInfo.Uploader;
            var uploaded = TimeZoneInfo.ConvertTime(fileInfo.Uploaded, User.TimeZone);

            var location = FileModel.GetLocation(Path);
            location = location.Substring(0, location.Length - 1);

            string uploaderName = null;
            if (uploader != Guid.Empty)
                uploaderName = UserSearch.GetFullName(uploader);

            FileTitle = FileModel.GetName(path);
            FilePath = path;
            FileName.Text = FileNameEdit.Text = FileModel.GetNameWithoutExtension(path);
            Folder.Text = FolderEdit.Text = location;
            FileSize.Text = string.Format("{0} ({1:n0} bytes)", size.Bytes().ToString("#.#"), size);
            DownloadURL.NavigateUrl = DownloadURL.Text = !string.IsNullOrEmpty(path) ? $"/files{path}" : null;
            PostedBy.Text = $"Posted {uploaded.Humanize()} by {uploaderName ?? uploader.ToString()}";
        }

        #region Ajax

        private bool HandleAjaxRequest()
        {
            if (!HttpRequestHelper.IsAjaxRequest || !bool.TryParse(Page.Request.Form["IsPageAjax"], out bool isAjax) || !isAjax)
            {
                return false;
            }

            Response.Clear();

            if (!UploadSearch.ExistsByOrganizationIdentifier(Organization.OrganizationIdentifier, Path))
            {
                Response.Write(JsonConvert.SerializeObject(JsonTextError("File not found")));
                return true;
            }

            var action = Page.Request.Form["action"];

            if (action == "edit")
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
                Response.Write(JsonConvert.SerializeObject(new JsonSuccessResult()));
            }
            else if (action == "save-edit")
            {
                var model = FileEditModel.Create(Request, out var validationError);
                if (validationError != null)
                {
                    Response.Write(JsonConvert.SerializeObject(JsonTextError(validationError)));
                }
                else if (string.Compare(Path, model.Path, true) != 0 &&
                    UploadSearch.ExistsByOrganizationIdentifier(Organization.OrganizationIdentifier, model.Path))
                {
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    Response.Write(JsonConvert.SerializeObject(JsonTextError($"The folder '{model.Location}' already contains a file named '{model.Name}'")));
                }
                else
                {
                    FileHelper.Provider.Update(Organization.OrganizationIdentifier, Path, x => x.Path = model.Path);

                    Response.StatusCode = (int)HttpStatusCode.OK;
                    Response.Write(JsonConvert.SerializeObject(new JsonSuccessResult()));
                }
            }
            else if (action == "view")
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
                Response.Write(JsonConvert.SerializeObject(new JsonSuccessResult()));
            }
            else if (action == "delete")
            {
                FileHelper.Provider.Delete(Organization.OrganizationIdentifier, Path);

                Response.StatusCode = (int)HttpStatusCode.OK;
                Response.Write(JsonConvert.SerializeObject(new JsonSuccessResult()));
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.Write(JsonConvert.SerializeObject(JsonTextError("Unexpected action: " + action)));
            }

            Response.End();

            return true;
        }

        private bool HandleDownloadRequest()
        {
            var action = Page.Request.Form["action"];

            if (action != "download")
            {
                return false;
            }

            using (var stream = FileHelper.Provider.Read(Organization.OrganizationIdentifier, Path))
            {
                if (stream != Stream.Null)
                {
                    var data = GetBytes(stream);
                    var filename = FileModel.GetName(Path);
                    var mime = MimeMapping.GetContentType(filename);
                    var length = data.Length;

                    Response.Clear();
                    Response.ClearHeaders();
                    Response.ClearContent();

                    Response.AddHeader("Content-Disposition", $"attachment; filename={filename}");
                    Response.AddHeader("Content-Length", length.ToString());

                    Response.ContentType = mime;

                    Response.BinaryWrite(data);

                    Response.StatusCode = (int)HttpStatusCode.OK;
                    Response.End();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(Page, GetType(), "FileNotFound", "alert('File not found');", true);
                }
            }

            return true;
        }

        private byte[] GetBytes(Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        private JsonResult JsonTextError(string textMessage)
        {
            var html = string.IsNullOrEmpty(textMessage)
                ? string.Empty
                : System.Web.HttpUtility.HtmlEncode(textMessage).Replace("\r", string.Empty).Replace("\n", "<br/>");

            return JsonHtmlError(html);
        }

        private JsonResult JsonHtmlError(string htmlMessage)
        {
            return new JsonErrorResult(htmlMessage);
        }

        #endregion
    }
}