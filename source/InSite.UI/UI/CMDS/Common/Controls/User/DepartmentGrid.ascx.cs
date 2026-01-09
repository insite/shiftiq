using System;
using System.Web.UI;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;

namespace InSite.Cmds.Controls.Contacts.Departments
{
    public partial class DepartmentGrid : UserControl
    {
        private DepartmentFilter Filter
        {
            get => (DepartmentFilter)ViewState[nameof(Filter)];
            set => ViewState[nameof(Filter)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.DataBinding += Grid_DataBinding;
        }

        public int LoadData(Guid organizationId, bool showDivision)
        {
            Filter = new DepartmentFilter
            {
                OrganizationIdentifier = organizationId
            };

            Grid.Columns.FindByHeaderText("Division").Visible = showDivision;
            Grid.VirtualItemCount = ContactRepository3.CountDepartments(Filter);
            Grid.DataBind();
            Grid.PageIndex = 0;

            CreatorLink.NavigateUrl = $"/ui/cmds/admin/departments/create?organizationID={organizationId}";

            return Grid.VirtualItemCount;
        }

        private void Grid_DataBinding(object source, EventArgs e)
        {
            if (Filter != null)
                Filter.Paging = Paging.SetPage(Grid.PageIndex + 1, Grid.PageSize);

            var table = ContactRepository3.SelectDepartmentsWithCounts(Filter);

            Grid.DataSource = table;
        }
    }
}