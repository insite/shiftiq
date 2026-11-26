using System;
using System.Linq;
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
            else if (StringHelper.Equals(e.CommandName, "DeleteStoredFile") && AllowEdit)
            {
                if (Guid.TryParse((string)e.CommandArgument, out var fileId))
                    DeleteStoredFile(fileId);

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

            var items = uploads
                .Select(x => new DownloadItem
                {
                    Identifier = x.UploadIdentifier,
                    DeleteCommand = "DeleteFile",
                    Url = CmdsUploadProvider.GetFileRelativeUrl(x.ContainerIdentifier, x.Name),
                    Title = x.Title,
                    Description = x.Description,
                    ContentSize = x.ContentSize ?? 0
                })
                .ToList();

            // Files attached to the credential through the new file storage, such as a
            // certificate a learner uploaded for review. A renewed credential keeps the
            // certificates of its earlier cycles, so each is listed in the order it was uploaded
            // and dated, and a reviewer can tell the current certificate from the record.
            var storedFiles = ServiceLocator.FileSearch.GetModels(null, ContainerGuid, null, false);

            var timeZone = CurrentSessionState.Identity.User.TimeZone;

            items.AddRange(storedFiles
                .OrderBy(x => x.Uploaded)
                .ThenBy(x => x.FileIdentifier)
                .Select(x => new DownloadItem
                {
                    Identifier = x.FileIdentifier,
                    DeleteCommand = "DeleteStoredFile",
                    Url = ServiceLocator.StorageService.GetFileUrl(x.FileIdentifier, x.FileName, true),
                    Title = x.FileName,
                    Description = "Uploaded "
                        + TimeZones.FormatDateOnly(x.Uploaded, timeZone, null, "{0:yyyy-MM-dd}"),
                    ContentSize = x.FileSize
                }));

            Repeater.DataSource = items;
            Repeater.DataBind();

            Repeater.Visible = items.Count > 0;
        }

        private void DeleteStoredFile(Guid fileId)
        {
            var file = ServiceLocator.FileSearch.GetModels(null, ContainerGuid, null, false)
                .FirstOrDefault(x => x.FileIdentifier == fileId);

            // Only a file attached to this container can be deleted from here.
            if (file == null)
                return;

            ServiceLocator.StorageService.Delete(fileId);
        }

        private class DownloadItem
        {
            public Guid Identifier { get; set; }
            public string DeleteCommand { get; set; }
            public string Url { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public int ContentSize { get; set; }
        }

        #endregion
    }
}