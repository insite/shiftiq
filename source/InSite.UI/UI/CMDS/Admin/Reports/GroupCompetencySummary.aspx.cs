using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Persistence.Plugin.CMDS;
using InSite.UI.CMDS.Common.Controls.User;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Cmds.Actions.Talent.Employee.Competency.Group
{
    public partial class GroupCompetencySummary : AdminBasePage, ICmdsUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ManagersRepeater.DataBinding += ManagersRepeater_DataBinding;
            ManagersRepeater.ItemDataBound += ManagersRepeater_ItemDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            var permissionName = PermissionNames.Custom_CMDS_Workers;
            var hasPermissions = Identity.IsGranted(permissionName, DataAccess.Delete) ||
                Identity.IsGranted(permissionName, DataAccess.Configure) ||
                Identity.IsGranted(permissionName, DataAccess.Read) ||
                Identity.IsGranted(permissionName, DataAccess.Update);

            ReportSection.Visible = hasPermissions;

            if (!hasPermissions)
            {
                ScreenStatus.AddMessage(AlertType.Error, "You do not have enough permissions. Please contact system administrator.");
                return;
            }

            ManagersRepeater.DataBind();
        }

        private void ManagersRepeater_DataBinding(object sender, EventArgs e)
        {
            ManagersRepeater.DataSource =
                ContactRepository3.SelectManagersForGroupCompetencySummaryViewer(Organization.Identifier);
        }

        private void ManagersRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var data = e.Item.DataItem as DataRowView;

            var relatedEmployees = (Repeater)e.Item.FindControl("RelatedEmployees");
            relatedEmployees.ItemDataBound += RelatedEmployees_ItemDataBound1;
            relatedEmployees.DataSource =
                ContactRepository3.SelectManagerWorkersForGroupCompetencySummaryViewer((Guid)data["UserIdentifier"], Organization.Identifier);
            relatedEmployees.DataBind();

            e.Item.Visible = relatedEmployees.Items.Count > 0;
        }

        private void RelatedEmployees_ItemDataBound1(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var row = e.Item.DataItem as DataRowView;

            var workersCount = (int)row["WorkersCount"];

            var employeeCountOutput = (ITextControl)e.Item.FindControl("EmployeeCountOutput");
            employeeCountOutput.Text = workersCount == 1
                ? "1 Worker"
                : string.Format("{0} Workers", workersCount);

            var groupCompetencySummary = (CompetencySummary)e.Item.FindControl("GroupCompetencySummary");
            groupCompetencySummary.LoadData((Guid)row["UserIdentifier"], Organization.Identifier, CompetencySummaryType.ManagerGroup);
        }
    }
}