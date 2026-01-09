using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.Infrastructure;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Constant;
using Shift.Common.Events;

namespace InSite.Admin.Sites.Pages.Controls.ContentBlocks
{
    public partial class Image : BaseUserControl
    {
        #region Events

        public event AlertHandler Alert;
        private void OnAlert(AlertType type, string message) =>
            Alert?.Invoke(this, new AlertArgs(type, message));

        #endregion

        #region Properties

        public string UploadPath
        {
            get => (string)ViewState[nameof(UploadPath)];
            set => ViewState[nameof(UploadPath)] = value;
        }

        public string ImageAlt
        {
            get => ImgAlt.Text;
            set => ImgAlt.Text = value;
        }

        public string ImageUrl
        {
            get => ImgUrl.Text;
            set => ImgUrl.Text = value;
        }

        public bool IsBrowsable
        {
            get => ViewState[nameof(IsBrowsable)] is bool value ? value : true;
            set => ViewState[nameof(IsBrowsable)] = value;
        }

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CommonScript.ContentKey = typeof(Image).FullName;

            FileUploadButton.Click += FileUploadButton_Click;

            FileUploadButton.ValidationGroup = UniqueID;
            FileUploadValidator.ValidationGroup = UniqueID;
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (string.IsNullOrEmpty(UploadPath))
                throw new ApplicationError("UploadPath is null");

            FileUploadButton.Visible = IsBrowsable;

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(Image),
                "init_" + ClientID,
                $"contentBlocks.image.init({HttpUtility.JavaScriptStringEncode(FileUploadButton.ClientID, true)},{HttpUtility.JavaScriptStringEncode(FileUpload.ClientID, true)});",
                true);

            base.OnPreRender(e);
        }

        private void FileUploadButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            try
            {
                var file = FileUpload.PostedFile;
                var fileName = StringHelper.Sanitize(Path.GetFileNameWithoutExtension(file.FileName), '-');
                var fileExt = Path.GetExtension(file.FileName);
                var fileModel = FileHelper.Provider.Save(Organization.OrganizationIdentifier, UploadPath + "/" + fileName + fileExt, file.InputStream);

                ImgUrl.Text = FileHelper.GetUrl(fileModel.Path);
            }
            catch (ApplicationError err)
            {
                OnAlert(AlertType.Error, err.Message);
            }
        }
    }
}