using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Cmds.Infrastructure;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Constant;

namespace InSite.Cmds.Controls.Training.Achievements
{
    public partial class AchievementDownloadList : UserControl
    {
        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            List.ItemDataBound += List_ItemDataBound;
        }

        #endregion

        #region Event handlers

        private void List_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item)
                return;

            var row = (DataRowView)e.Item.DataItem;
            var attachmentLink = (HyperLink)e.Item.FindControl("AttachmentLink");

            var type = (string)row["UploadType"];
            var container = (Guid)row["ContainerIdentifier"];
            var name = (string)row["Name"];
            var title = (string)row["Title"];

            attachmentLink.NavigateUrl = type == UploadType.CmdsFile
                ? CmdsUploadProvider.GetFileRelativeUrl(container, name)
                : name;

            attachmentLink.Text = title;

            var attachmentInfo = (Literal)e.Item.FindControl("AttachmentInfo");

            var uploaded = (DateTimeOffset)row["uploaded"];
            var uploaderID = (Guid)row["uploader"];

            var uploaderName = UserSearch.GetFullName(uploaderID);
            attachmentInfo.Text = $"Uploaded by {uploaderName} {uploaded.Humanize()}";
        }

        #endregion

        #region Public methods

        public bool LoadUploads(Guid achievementIdentifier)
        {
            var uploads = UploadRepository.SelectAllAchievementUploads(achievementIdentifier, null);

            List.DataSource = uploads;
            List.DataBind();

            return uploads.Rows.Count > 0;
        }

        #endregion
    }
}