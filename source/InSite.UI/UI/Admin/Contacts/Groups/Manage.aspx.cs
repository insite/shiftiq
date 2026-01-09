using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Admin.Contacts.Groups.Models;
using InSite.UI.Layout.Admin;

using Shift.Constant;

namespace InSite.UI.Admin.Contacts.Groups
{
    public partial class Manage : AdminBasePage
    {
        #region Classes

        private class TreeViewDataItem
        {
            public Guid ContactID { get; set; }
            public Guid Thumbprint { get; set; }
            public string Name { get; set; }
            public string Icon { get; set; }
            public string Subtype { get; set; }
            public string Abbreviation { get; set; }
            public int MemberCount { get; set; }
            public int PermissionCount { get; set; }

            public GroupOutlineModel.GroupModel Previous { get; set; }
            public GroupOutlineModel.GroupModel Parent { get; set; }

            public string HtmlPrefix { get; set; }
            public string HtmlPostfix { get; set; }
        }

        #endregion

        #region Inialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SearchButton.Click += SearchButton_Click;
            ClearButton.Click += ClearButton_Click;

            TreeViewRepeater.ItemCommand += TreeViewRepeater_ItemCommand;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(Page);

            OutlineTab.Visible = false;
            CriteriaTab.IsSelected = true;
        }

        #endregion

        #region Event handlers

        private void SearchButton_Click(object sender, EventArgs e)
        {
            LoadData();

            OutlineTab.Visible = true;
            OutlineTab.IsSelected = true;
            StateValue.Value = null;
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            FilterKeyword.Text = null;

            OutlineTab.Visible = false;
            CriteriaTab.IsSelected = true;
            StateValue.Value = null;
        }

        private void TreeViewRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Indent")
            {
                var parts = ((string)e.CommandArgument).Split(new[] { ':' });
                var groupId = Guid.Parse(parts[0]);
                var previousIdentifier = Guid.Parse(parts[1]);

                GroupOutlineModel.Indent(groupId, previousIdentifier);

                LoadData();
            }
            else if (e.CommandName == "Outdent")
            {
                var contactID = Guid.Parse((string)e.CommandArgument);

                GroupOutlineModel.Outdent(contactID);

                LoadData();
            }
        }

        #endregion

        #region Load data

        private void LoadData()
        {
            var dataSource = new List<TreeViewDataItem>();
            var model = GroupOutlineModel.Create(Organization, FilterKeyword.Text);

            var root = model.Root;

            var prevNode = AddNode(root, null, dataSource);

            if (dataSource.Count > 0)
            {
                var lastDataItem = dataSource[dataSource.Count - 1];

                if (prevNode != root)
                    lastDataItem.HtmlPostfix += BuildTreeEnd(GetDepth(prevNode) - GetDepth(root));

                lastDataItem.HtmlPostfix += "</ul>";
            }

            TreeViewRepeater.DataSource = dataSource;
            TreeViewRepeater.DataBind();
        }

        private GroupOutlineModel.GroupModel AddNode(GroupOutlineModel.GroupModel node, GroupOutlineModel.GroupModel prevNode, List<TreeViewDataItem> dataSource)
        {
            if (node.Parent != null)
            {
                var dataItem = new TreeViewDataItem
                {
                    ContactID = node.Identifier,
                    Thumbprint = node.Thumbprint,
                    Name = node.Name,
                    Icon = GroupTypes.GetIcon(node.Subtype),
                    Subtype = node.Subtype,
                    Abbreviation = node.Abbreviation,
                    MemberCount = node.MemberCount,
                    PermissionCount = node.PermissionCount,
                    Parent = node.Parent,
                    Previous = node.Previous
                };

                if (prevNode == null)
                    dataItem.HtmlPrefix = $"<ul id='{TreeViewRepeater.ClientID}' data-init='code' class='tree-view'><li data-key='{dataItem.ContactID}'>";
                else if (prevNode == node.Parent)
                    dataItem.HtmlPrefix = $"<ul class='tree-view'><li data-key='{dataItem.ContactID}'>";
                else if (prevNode.Parent == node.Parent)
                    dataItem.HtmlPrefix = $"<li data-key='{dataItem.ContactID}'>";
                else
                    dataItem.HtmlPrefix = BuildTreeEnd(GetDepth(prevNode) - GetDepth(node)) + $"<li data-key='{dataItem.ContactID}'>";

                if (prevNode != null && node.Children.Count == 0)
                    dataItem.HtmlPostfix = "</li>";

                dataSource.Add(dataItem);

                prevNode = node;
            }

            foreach (var childNode in node.Children)
            {
                var addedNode = AddNode(childNode, prevNode, dataSource);

                if (addedNode != null)
                    prevNode = addedNode;
            }

            return prevNode;
        }

        private int GetDepth(GroupOutlineModel.GroupModel node)
        {
            var i = 0;
            var curNode = node;

            while (curNode != null)
            {
                i++;
                curNode = curNode.Parent;
            }

            return i;
        }

        private string BuildTreeEnd(int levels) => string.Concat(Enumerable.Repeat("</ul></li>", levels));

        #endregion
    }
}