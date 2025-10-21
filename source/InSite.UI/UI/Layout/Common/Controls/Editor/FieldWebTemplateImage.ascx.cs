using System;
using System.IO;

using InSite.Common.Web.Infrastructure;
using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.UI.Layout.Common.Controls.Editor
{
    public partial class FieldWebTemplateImage : BaseUserControl
    {
        public string UploadPath
        {
            get => (string)ViewState[nameof(UploadPath)];
            set => ViewState[nameof(UploadPath)] = value;
        }

        public string ImageAltText
        {
            get => ImgAlt.Text;
            set => ImgAlt.Text = value;
        }

        public string ImageUrl
        {
            get => ImgUrl.Text;
            set => ImgUrl.Text = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CommonScript.ContentKey = typeof(FieldWebTemplateImage).FullName;

            FileUploadButton.Click += FileUploadButton_Click;

            FileUploadButton.ValidationGroup = UniqueID;
            FileUploadValidator.ValidationGroup = UniqueID;
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (string.IsNullOrEmpty(UploadPath))
                throw new ApplicationError("UploadPath is null");

            base.OnPreRender(e);
        }

        private void FileUploadButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var file = FileUpload.PostedFile;
            var fileName = StringHelper.Sanitize(Path.GetFileNameWithoutExtension(file.FileName), '-');
            var fileExt = Path.GetExtension(file.FileName);
            var fileModel = FileHelper.Provider.Save(Organization.OrganizationIdentifier, UploadPath + "/" + fileName + fileExt, file.InputStream);

            ImgUrl.Text = FileHelper.GetUrl(fileModel.Path);
        }
    }
}