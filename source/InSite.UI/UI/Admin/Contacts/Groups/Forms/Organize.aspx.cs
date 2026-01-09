using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Contacts.Read;
using InSite.Application.Groups.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Contacts.Groups.Forms
{
    public partial class Organize : AdminBasePage, IHasParentLinkParameters
    {
        #region Constants

        private const string EditUrl = "/ui/admin/contacts/groups/edit";
        private const string SearchUrl = "/ui/admin/contacts/groups/search";

        private const int MinItemsPerColumn = 10;

        #endregion

        #region Properties

        private Guid GroupIdentifier
        {
            get => (Guid)ViewState[nameof(GroupIdentifier)];
            set => ViewState[nameof(GroupIdentifier)] = value;
        }

        private List<Tuple<Guid, bool>> SupergroupState
        {
            get => (List<Tuple<Guid, bool>>)ViewState[nameof(SupergroupState)];
            set => ViewState[nameof(SupergroupState)] = value;
        }

        #endregion

        #region Fields

        private int _itemsPerColumn = 0;
        private int _counter = 0;
        private string _prevGroupType = null;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SupergroupRepeater.DataBinding += SupergroupRepeater_DataBinding;
            SupergroupRepeater.ItemDataBound += SupergroupRepeater_ItemDataBound;

            SaveButton.Click += SaveButton_Click;
        }

        #endregion

        #region Loading

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!CanEdit)
                RedirectToFinder();

            Open();
        }

        #endregion

        #region Event handlers

        private void SaveButton_Click(object sender, EventArgs e) => Save();

        private void SupergroupRepeater_DataBinding(object sender, EventArgs e)
        {
            var items = ServiceLocator.GroupSearch.GetSupergroups(Organization.OrganizationIdentifier, GroupIdentifier);

            var relationships = ServiceLocator.GroupSearch.GetGroupDescendentRelationships(GroupIdentifier);
            if (relationships.Length > 0)
            {
                var disabledGroups = relationships.Select(x => x.ChildGroupIdentifier).ToHashSet();
                foreach (var item in items)
                    item.Enabled = !disabledGroups.Contains(item.GroupIdentifier);
            }

            _counter = 0;

            SupergroupState = new List<Tuple<Guid, bool>>();

            if (items.Count > MinItemsPerColumn * 4)
                _itemsPerColumn = (int)Math.Ceiling(items.Count / 3D);
            else if (items.Count > MinItemsPerColumn * 2)
                _itemsPerColumn = (int)Math.Ceiling(items.Count / 2D);

            SupergroupRepeater.DataSource = items;
        }

        private void SupergroupRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dataItem = (SupergroupDataItem)e.Item.DataItem;

            SupergroupState.Add(new Tuple<Guid, bool>(dataItem.GroupIdentifier, dataItem.Selected));
        }

        #endregion

        #region Database operations

        private void Open()
        {
            if (!Guid.TryParse(Request["group"], out var groupId))
                RedirectToFinder();

            var group = ServiceLocator.GroupSearch.GetGroup(groupId);
            if (group == null || group.OrganizationIdentifier != Organization.OrganizationIdentifier)
                RedirectToFinder();

            GroupIdentifier = group.GroupIdentifier;

            PageHelper.AutoBindHeader(
                Page, null,
                $"{group.GroupName} <span class='form-text'>{group.GroupType}</span>");

            CancelButton.NavigateUrl = GetEditorUrl();

            SupergroupRepeater.DataBind();
        }

        private void Save()
        {
            var insertParents = new List<Guid>();
            var removeParents = new List<Guid>();

            for (var i = 0; i < SupergroupRepeater.Items.Count; i++)
            {
                var item = SupergroupRepeater.Items[i];
                if (!IsContentItem(item))
                    continue;

                var stateItem = SupergroupState[i];
                var isSelected = (ICheckBox)item.FindControl("IsSelected");

                if (isSelected.Checked != stateItem.Item2)
                {
                    if (isSelected.Checked)
                    {
                        if (ServiceLocator.GroupSearch.CausesCycle(GroupIdentifier, stateItem.Item1))
                        {
                            ScreenStatus.AddMessage(
                                AlertType.Error,
                                $"The <strong>{isSelected.Text}</strong> group is not valid selection because this relationship creates dependency cycle in hierarchy.");
                            return;
                        }

                        insertParents.Add(stateItem.Item1);
                    }
                    else
                        removeParents.Add(stateItem.Item1);
                }
            }

            if (removeParents.Count > 0)
                DeleteConnections(removeParents);

            if (insertParents.Count > 0)
                AddConnections(insertParents);

            RedirectToEditor();
        }

        private void DeleteConnections(List<Guid> parents)
        {
            var commands = new List<Command>();
            foreach (var parent in parents)
                commands.Add(new DisconnectGroup(GroupIdentifier, parent));

            ServiceLocator.SendCommands(commands);
        }

        private void AddConnections(List<Guid> parents)
        {
            var commands = new List<Command>();
            foreach (var parent in parents)
                commands.Add(new ConnectGroup(GroupIdentifier, parent));

            ServiceLocator.SendCommands(commands);
        }

        #endregion

        #region Methods (navigation)

        private void RedirectToFinder() =>
            HttpResponseHelper.Redirect(SearchUrl, true);

        private void RedirectToEditor()
        {
            var url = GetEditorUrl();

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetEditorUrl()
            => EditUrl + $"?contact={GroupIdentifier}";

        #endregion

        #region Helper methods

        protected string TryRenderHeader(object o)
        {
            var item = (SupergroupDataItem)o;
            var result = string.Empty;
            var isSeparator = false;

            if (_itemsPerColumn >= MinItemsPerColumn)
            {
                _counter++;

                if (_counter > _itemsPerColumn)
                {
                    _counter = 1;
                    isSeparator = true;

                    result = "</div><div class='col-md-4'>";
                }
            }

            if (_prevGroupType == null || _prevGroupType != item.GroupType)
            {
                _prevGroupType = item.GroupType;

                result += $"<h6 class='mt-3'>{item.GroupType}</h6>";
            }
            else if (isSeparator)
            {
                result += $"<h6 class='d-none d-md-block mt-3'>{item.GroupType}</h6>";
            }

            return result;
        }

        #endregion

        #region IHasParentLinkParameters

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/edit")
                ? $"contact={GroupIdentifier}"
                : null;
        }

        #endregion
    }
}