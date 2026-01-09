using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;

using Shift.Sdk.UI;

namespace InSite.UI.Admin.Assets.Files.Controls
{
    public partial class FileTreeViewNode : UserControl
    {
        public void LoadData(List<FileTreeItemModel> items)
        {
            FileRepeater.ItemDataBound += FileRepeater_ItemDataBound;
            FileRepeater.DataSource = items;
            FileRepeater.DataBind();
        }

        private void FileRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var item = (FileTreeItemModel)e.Item.DataItem;

            var childrenNode = (HtmlGenericControl)e.Item.FindControl("ChildrenNode");
            var container = (DynamicControl)e.Item.FindControl("Container");
            BindChildrenNode(childrenNode, container, item);

            var fileLink = (ITextControl)e.Item.FindControl("FileLink");
            BindFileLink(fileLink, childrenNode, item);
        }

        private void BindChildrenNode(HtmlGenericControl childrenNode, DynamicControl container, FileTreeItemModel item)
        {
            if (item.Children.Count == 0)
                return;

            childrenNode.Attributes["class"] = item.IsCollapsed
                ? "collapse"
                : "collapse show";

            var node = (FileTreeViewNode)container.LoadControl("~/UI/Admin/Assets/Files/Controls/FileTreeViewNode.ascx");
            node.LoadData(item.Children);
        }

        private void BindFileLink(ITextControl fileLink, HtmlGenericControl childrenNode, FileTreeItemModel item)
        {
            const string icon = "<i class='me-1 far fa-folder'></i>";

            var fileName = HttpUtility.HtmlEncode(item.FileName);
            var text = $"{icon} <span>{fileName}</span>";
            var data = fileName.Replace("'", "\\'");
            var root = item.IsRoot ? "data-root='1'" : "";

            var attrs = $"data-bs-toggle='collapse' data-folder='{data}' onclick='treeView.showFolder(this);' {root}";
            var collapsed = item.IsCollapsed ? "collapsed" : "";
            var selected = item.IsSelected ? "selected" : "";

            fileLink.Text = item.Children.Count > 0
                ? $"<a href='#{childrenNode.ClientID}' class='widget-link sub {collapsed} {selected}' {attrs}>{text}</a>"
                : $"<a href='#' class='widget-link nosub {selected}' {attrs}>{text}</a>";
        }
    }
}