using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Cmds.Infrastructure;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Custom.CMDS.User.Progressions.Controls
{
    public partial class EmployeeAchievementDownloadGrid : UserControl
    {
        #region Properties

        protected Guid ContainerGuid
        {
            get { return (Guid)ViewState[nameof(ContainerGuid)]; }
            set { ViewState[nameof(ContainerGuid)] = value; }
        }

        protected string ContainerType
        {
            get { return (string)ViewState[nameof(ContainerType)]; }
            set { ViewState[nameof(ContainerType)] = value; }
        }

        public bool AllowEdit
        {
            get { return ViewState[nameof(AllowEdit)] != null && (bool)ViewState[nameof(AllowEdit)]; }
            set { ViewState[nameof(AllowEdit)] = value; }
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Repeater.ItemCommand += Repeater_ItemCommand;

            RefreshDownloadsButton.Click += RefreshDownloadsButton_Click;

            AttachFileButton.Click += AttachFileButton_Click;
        }

        private void AttachFileButton_Click(object sender, EventArgs e)
        {
            UploadFileError.Visible = false;

            if (string.IsNullOrEmpty(File.FileName))
                return;

            try
            {
                CmdsUploadProvider.Current.Update(
                    ContainerType,
                    ContainerGuid,
                    CmdsUploadProvider.AdjustFileName(System.IO.Path.GetFileName(File.FileName)),
                    GetInputValues);

                // Nov 7, 2023 - Daniel: This line causes an uncaught type error in JavaScript. See DEV-7604.
                // Page.ClientScript.RegisterStartupScript(GetType(), "Close", "closeFileUploader();", true);

                LoadData();
            }
            catch (ApplicationError ex)
            {
                UploadFileError.Text = ex.Message;
                UploadFileError.Visible = true;
            }
        }

        private void GetInputValues(ICmdsUploadModel entity)
        {
            entity.Title = Title.Text;
            entity.Description = Description.Text;

            if (File.Visible && File.HasFile)
                entity.Write(File.FileContent);
        }

        private void Repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                LoadData();
            }
        }

        #endregion

        #region Event handlers

        private void Repeater_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            if (StringHelper.Equals(e.CommandName, "DeleteFile"))
            {
                if (Guid.TryParse((string)e.CommandArgument, out var uploadId))
                    CmdsUploadProvider.Current.Delete(uploadId);

                LoadData();
            }
        }

        private void RefreshDownloadsButton_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        #endregion

        #region Public methods

        public void LoadData(Guid containerGuid, string containerType)
        {
            ContainerGuid = containerGuid;
            ContainerType = containerType;

            UploadFileButton.OnClientClick = $"showFileUploader('', '{ContainerGuid}'); return false;";

            LoadData();
        }

        #endregion

        #region Helper methods

        private void LoadData()
        {
            var uploads = UploadSearch.Bind(
                x => new
                {
                    x.UploadIdentifier,
                    x.ContainerIdentifier,
                    x.Name,
                    x.Title,
                    x.Description,
                    x.ContentSize
                },
                x => x.ContainerIdentifier == ContainerGuid && x.UploadType == UploadType.CmdsFile,
                "Title");

            Repeater.DataSource = uploads;
            Repeater.DataBind();

            Repeater.Visible = uploads.Count > 0;
        }

        #endregion
    }
}