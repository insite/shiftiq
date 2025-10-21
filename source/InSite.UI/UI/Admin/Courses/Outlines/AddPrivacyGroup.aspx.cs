using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

using CheckBox = System.Web.UI.WebControls.CheckBox;

namespace InSite.UI.Admin.Courses.Outlines
{
    public partial class AddPrivacyGroup : AdminBasePage
    {
        private Guid ContainerIdentifier => Guid.TryParse(Request.QueryString["container"], out var container) ? container : Guid.Empty;
        private string ContainerType => Request.QueryString["type"];

        private bool IsSearched
        {
            get => ViewState[nameof(IsSearched)] as bool? ?? false;
            set => ViewState[nameof(IsSearched)] = value;
        }

        private HashSet<Guid> SelectedGroups => (HashSet<Guid>)(ViewState[nameof(SelectedGroups)]
            ?? (ViewState[nameof(SelectedGroups)] = new HashSet<Guid>()));

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AddButton.Click += AddButton_Click;

            FilterButton.Click += FilterButton_Click;
            ClearButton.Click += ClearButton_Click;

            NewGroups.AutoBinding = false;
            NewGroups.EnablePaging = false;
            NewGroups.RowCreated += NewGroups_ItemCreated;
            NewGroups.DataBinding += NewGroups_DataBinding;
            NewGroups.RowDataBound += NewGroups_ItemDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            var name = $"{ContainerType} {ContainerIdentifier}";

            if (ContainerType == "Course")
                name = "Course: " + CourseSearch.SelectCourse(ContainerIdentifier)?.CourseName;
            else if (ContainerType == "Unit")
                name = "Unit: " + CourseSearch.SelectUnit(ContainerIdentifier)?.UnitName;
            else if (ContainerType == "Module")
                name = "Module: " + CourseSearch.SelectModule(ContainerIdentifier)?.ModuleName;
            else if (ContainerType == "Activity")
                name = "Activity: " + CourseSearch.SelectActivity(ContainerIdentifier)?.ActivityName;

            ContainerName.InnerHtml = name;

            NewGroups.DataBind();
        }

        private void NewGroups_ItemCreated(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var isSelected = (CheckBox)e.Row.FindControl("IsSelected");
            isSelected.CheckedChanged += IsSelected_CheckedChanged;
        }

        private void NewGroups_DataBinding(object sender, EventArgs e)
        {
            NewGroups.DataSource = GetNewGroupsData();
        }

        private void NewGroups_ItemDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var id = (Guid)DataBinder.Eval(e.Row.DataItem, "GroupIdentifier");

            var isSelected = (CheckBox)e.Row.FindControl("IsSelected");
            isSelected.Checked = SelectedGroups.Contains(id);
        }

        private void IsSelected_CheckedChanged(object sender, EventArgs e)
        {
            var chk = (CheckBox)sender;
            var row = (GridViewRow)chk.NamingContainer;
            var grid = (Grid)row.NamingContainer;
            var id = grid.GetDataKey<Guid>(row);

            if (!chk.Checked && SelectedGroups.Contains(id))
                SelectedGroups.Remove(id);
            else if (chk.Checked && !SelectedGroups.Contains(id))
                SelectedGroups.Add(id);

        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (SelectedGroups.Count == 0)
                return;

            foreach (var groupIdentifier in SelectedGroups)
                ServiceLocator.ContentStore.InsertPrivacyGroup(ContainerIdentifier, ContainerType, groupIdentifier);

            Page.ClientScript.RegisterClientScriptBlock(
                GetType(),
                "Close",
                "modalManager.closeModal(true);",
                true);
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            IsSearched = true;

            SelectedGroups.Clear();

            NewGroups.DataBind();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            IsSearched = false;

            SearchText.Text = null;

            SelectedGroups.Clear();

            FoundGroupText.Text = null;

            NewGroups.DataBind();
        }

        private List<QGroup> GetNewGroupsData()
        {
            List<QGroup> result = null;

            if (IsSearched)
            {
                if (ContainerIdentifier != Guid.Empty)
                {
                    var filter = new QGroupFilter
                    {
                        OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier,
                        GroupNameLike = SearchText.Text.Trim(),
                        GroupType = GroupType.Value,
                        ExcludeContainerIdentifier = ContainerIdentifier
                    };

                    result = ServiceLocator.GroupSearch.GetGroups(filter);
                }

                var resultCount = result != null ? result.Count : 0;

                FoundGroupText.Text = resultCount > 0
                    ? $"{resultCount:n0} groups match your criteria"
                    : "No groups match your criteria";
            }

            var hasItems = result.IsNotEmpty();

            NewGroups.Visible = hasItems;
            AddButton.Visible = hasItems;

            return result;
        }
    }
}