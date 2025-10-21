using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;

using InSite.Common.Web.Infrastructure;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Admin.Courses.Resources.Controls
{
    public partial class EditDetailContent : UserControl
    {
        private interface IJsonResult
        {
            string Type { get; }
        }

        private class UploadSuccessResult : IJsonResult
        {
            public UploadSuccessResult(string path, string name, bool isImage)
            {
                Type = "OK";
                Path = path;
                Name = name;
                IsImage = isImage;
            }

            public string Path { get; }
            public string Name { get; }
            public bool IsImage { get; }
            public string Type { get; }
        }

        private class UploadErrorResult : IJsonResult
        {
            public UploadErrorResult()
            {
                Type = "ERROR";
                Messages = new List<string>();
            }

            public List<string> Messages { get; }
            public string Type { get; }
        }

        public string UploadFolderPath
        {
            get => (string)ViewState[nameof(UploadFolderPath)];
            set => ViewState[nameof(UploadFolderPath)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            UploadFileButton.Click += UploadFileButton_Click;
        }

        private void UploadFileButton_Click(object sender, EventArgs e)
        {
            var result = Page.IsValid && !string.IsNullOrEmpty(UploadFolderPath)
                ? GetUploadResult()
                : GetValidationErrors();

            Response.Clear();
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(result));
            Response.End();
        }

        private IJsonResult GetValidationErrors()
        {
            var result = new UploadErrorResult();

            if (string.IsNullOrEmpty(UploadFolderPath))
            {
                result.Messages.Add("UploadFolderPath is empty");
            }
            else
            {
                foreach (IValidator validator in Page.Validators)
                {
                    if (validator.IsValid)
                        continue;

                    result.Messages.Add(validator.ErrorMessage);
                }
            }

            return result;
        }

        private IJsonResult GetUploadResult()
        {
            try
            {
                var file = FileUpload.PostedFile;
                var fileName = StringHelper.Sanitize(Path.GetFileNameWithoutExtension(file.FileName), '-');
                var fileExt = Path.GetExtension(file.FileName);

                var document = FileHelper.Provider.Save(
                    CurrentSessionState.Identity.Organization.Identifier,
                    UploadFolderPath + "/" + fileName + fileExt,
                    file.InputStream);
                var url = Common.Web.HttpRequestHelper.CurrentRootUrlFiles + UploadFolderPath;

                return new UploadSuccessResult(
                    url,
                    document.Name,
                    !string.IsNullOrEmpty(document.Type) && FileExtension.IsImage(document.Type)
                );
            }
            catch (ApplicationError kex)
            {
                var result = new UploadErrorResult();
                result.Messages.Add(kex.Message);
                return result;
            }
            catch (Exception ex)
            {
                AppSentry.SentryError(ex);

                var result = new UploadErrorResult();
                result.Messages.Add("An error occurred on the server side");
                return result;
            }
        }
    }
}