using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Cmds.Infrastructure;
using InSite.Persistence;

using Shift.Constant;

namespace InSite.Cmds.Controls.Training.EmployeeAchievements
{
    public partial class AttachmentList : UserControl
    {
        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            List.ItemDataBound += List_ItemDataBound;
        }

        #endregion

        #region Event handlers

        private void List_ItemDataBound(Object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item)
                return;

            var dataItem = e.Item.DataItem;
            var attachmentLink = (HyperLink)e.Item.FindControl("AttachmentLink");

            var type = (string)DataBinder.Eval(dataItem, "UploadType");
            var container = (Guid)DataBinder.Eval(dataItem, "ContainerIdentifier");
            var name = (string)DataBinder.Eval(dataItem, "Name");
            var title = (string)DataBinder.Eval(dataItem, "Title");

            attachmentLink.Text = title;
            attachmentLink.NavigateUrl = type == UploadType.CmdsFile
                ? CmdsUploadProvider.GetFileRelativeUrl(container, name)
                : name;
        }

        #endregion

        #region Public methods

        public bool LoadUploads(DataTable table)
        {
            List.DataSource = table;
            List.DataBind();

            return table.Rows.Count > 0;
        }

        public bool LoadUploads(Guid containerId)
        {
            var list = UploadSearch.Bind(x => x, x => x.ContainerIdentifier == containerId, "Name");

            List.DataSource = list;
            List.DataBind();

            return list.Count > 0;
        }

        #endregion
    }
}