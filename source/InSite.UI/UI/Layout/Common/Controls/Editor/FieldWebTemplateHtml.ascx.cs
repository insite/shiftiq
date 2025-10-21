using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;

using InSite.Common.Web.Infrastructure;
using InSite.Common.Web.UI;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.UI.Layout.Common.Controls.Editor
{
    public partial class FieldWebTemplateHtml : BaseUserControl
    {
        #region Classes

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

        #endregion

        #region Properties

        public string Value
        {
            get => HtmlInput.Text;
            set => HtmlInput.Text = value;
        }

        public string UploadPath
        {
            get => (string)ViewState[nameof(UploadPath)];
            set => ViewState[nameof(UploadPath)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CommonScript.ContentKey = GetType().FullName;

            UploadFileButton.Click += UploadFileButton_Click;
            UploadFileButton.ValidationGroup = GetFileUploadValidationGroup();
            FileUploadRequiredValidator.ValidationGroup = GetFileUploadValidationGroup();
            FileUploadExtensionValidator.ValidationGroup = GetFileUploadValidationGroup();
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (string.IsNullOrEmpty(UploadPath))
                throw new ApplicationError("UploadPath is null");

            ScriptManager.RegisterStartupScript(Page, GetType(), "init_" + ClientID, $"webTemplateHtml.init('#{HtmlInput.ClientID}');", true);

            base.OnPreRender(e);
        }

        #endregion

        #region Event handlers

        private void UploadFileButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(UploadPath))
                return;

            IJsonResult result = new UploadErrorResult();

            if (!Page.IsValid)
            {
                var error = (UploadErrorResult)result;

                foreach (IValidator validator in Page.Validators)
                {
                    if (validator.IsValid)
                        continue;

                    error.Messages.Add(validator.ErrorMessage);
                }
            }
            else
            {
                try
                {
                    var file = FileUpload.PostedFile;
                    var fileName = StringHelper.Sanitize(Path.GetFileNameWithoutExtension(file.FileName), '-');
                    var fileExt = Path.GetExtension(file.FileName);
                    var fileModel = FileHelper.Provider.Save(Organization.Identifier, UploadPath + "/" + fileName + fileExt, file.InputStream);

                    result = new UploadSuccessResult(
                        InSite.Common.Web.HttpRequestHelper.CurrentRootUrlFiles + UploadPath,
                        fileModel.Name,
                        !string.IsNullOrEmpty(fileModel.Type) && FileExtension.IsImage(fileModel.Type));
                }
                catch (ApplicationError kex)
                {
                    ((UploadErrorResult)result).Messages.Add(kex.Message);
                }
                catch (Exception ex)
                {
                    AppSentry.SentryError(ex);

                    ((UploadErrorResult)result).Messages.Add("An error occurred on the server side");
                }
            }

            Response.Clear();
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(result));
            Response.End();
        }

        #endregion

        #region Methods (helpers)

        private string GetFileUploadValidationGroup() => $"{ClientID}_UploadFile";

        #endregion
    }
}