using System;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;

using Paging = Shift.Common.Paging;

namespace InSite.Cmds.Controls.Contacts.Persons
{
    public partial class PersonGrid : UserControl
    {
        public delegate void DeletingHandler(object sender, int personID);

        public event DeletingHandler Deleting;
        private void OnDeleting(int personID) =>
            Deleting?.Invoke(this, personID);

        private CmdsPersonFilter Filter
        {
            get => (CmdsPersonFilter)ViewState[nameof(Filter)];
            set => ViewState[nameof(Filter)] = value;
        }

        public int RowCount => Grid.VirtualItemCount;

        public bool HasRows => RowCount > 0;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.DataBinding += Grid_DataBinding;
            Grid.RowCommand += Grid_RowCommand;

            RoleTypeSelector.AutoPostBack = true;
            RoleTypeSelector.SelectedIndexChanged += (s, a) => ReloadGrid();
        }

        private void Grid_DataBinding(object sender, EventArgs e)
        {
            Filter.Paging = Paging.SetPage(Grid.PageIndex + 1, Grid.PageSize);

            var table = ContactRepository3.SelectSearchResults(Filter, CurrentSessionState.Identity.Organization.Identifier);

            Grid.DataSource = table;
        }

        private void Grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var row = GridViewExtensions.GetRow(e);
                var userIdentifier = Grid.GetDataKey<int>(row);

                OnDeleting(userIdentifier);
            }
        }

        public void LoadData(CmdsPersonFilter filter)
        {
            Filter = filter;

            ReloadGrid();
        }

        public void SetVisibleColumns(string[] visibleColumns)
        {
            foreach (DataControlField column in Grid.Columns)
                column.Visible = false;

            foreach (var column in visibleColumns)
                Grid.Columns.FindByName(column).Visible = true;
        }

        public void ShowFilterPanel()
        {
            FilterPanel.Visible = true;
        }

        private void ReloadGrid()
        {
            PrepareFilter(Filter);

            Grid.VirtualItemCount = ContactRepository3.CountSearchResults(Filter);
            Grid.PageIndex = 0;
            Grid.DataBind();
        }

        private void PrepareFilter(CmdsPersonFilter filter)
        {
            if (!FilterPanel.Visible)
                return;

            var membershpSubTypes = RoleTypeSelector.Items.Cast<ListItem>()
                .Where(x => x.Selected)
                .Select(x => x.Value)
                .ToArray();

            filter.RoleType = membershpSubTypes.Length > 0
                ? membershpSubTypes
                : new[] { "none" };
        }

        protected static string GetCompanyName(Guid userKey)
        {
            var table = ContactRepository3.SelectPersonOrganizations(userKey);

            if (table.Rows.Count == 0)
                return "N/A";

            return table.Rows.Count == 1 ? (string)table.Rows[0]["CompanyTitle"] : "(Multiple Organizations)";
        }
    }
}