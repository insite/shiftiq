using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Custom.CMDS.Common.Controls.Server;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Constant.CMDS;
using Shift.Sdk.UI;

namespace InSite.Cmds.Actions.Contact.Department.Competency
{
    public partial class PrioritizeCompetencies : AdminBasePage, ICmdsUserControl, IHasParentLinkParameters
    {
        private const string SearchUrl = "/ui/cmds/admin/organizations/search";
        private const string ParenthUrl = "/ui/cmds/admin/departments/edit";

        private Guid DepartmentIdentifier => Guid.TryParse(Request.QueryString["id"], out var value) ? value : Guid.Empty;

        private Guid OrganizationIdentifier
        {
            get => (Guid)(ViewState[nameof(OrganizationIdentifier)] ?? Guid.Empty);
            set => ViewState[nameof(OrganizationIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Levels.RowCreated += Levels_RowCreated;
            Levels.RowDataBound += Levels_RowDataBound;
            Levels.DataBinding += Levels_DataBinding;
            Levels.PageIndexChanging += Levels_PageIndexChanging;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var department = DepartmentSearch.Select(DepartmentIdentifier);
            if (department == null)
                HttpResponseHelper.Redirect(SearchUrl);

            Levels.VirtualItemCount = Persistence.Plugin.CMDS.DepartmentProfileCompetencyRepository2.SelectCompetencyCount(DepartmentIdentifier);

            DataBind();

            var organization = OrganizationSearch.Select(OrganizationIdentifier = department.OrganizationIdentifier);

            PageHelper.AutoBindHeader(this, qualifier: $"{organization.CompanyName} ({department.DepartmentName})");

            CancelButton.NavigateUrl = ParenthUrl + $"?id={department.DepartmentIdentifier}";
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            if (parent.Name.EndsWith("departments/edit"))
                return $"id={DepartmentIdentifier}";

            if (parent.Name.EndsWith("organizations/edit"))
                return $"id={OrganizationIdentifier}";

            return null;
        }

        private void Levels_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            var isTimeSensitive = (System.Web.UI.WebControls.CheckBox)e.Row.FindControl("IsTimeSensitive");
            isTimeSensitive.PreRender += IsTimeSensitive_PreRender;
        }

        private void Levels_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            var row = (DataRowView)e.Row.DataItem;

            var priority = (CompetencyPrioritySelector2)e.Row.FindControl("Priority");
            var criticalIcon = (Icon)e.Row.FindControl("CriticalIcon");
            var validForUnit = (ValidForUnitSelector2)e.Row.FindControl("ValidForUnit");
            var validForContainer = (HtmlGenericControl)e.Row.FindControl("ValidForContainer");

            validForContainer.Attributes["class"] = (bool)row["IsTimeSensitive"] ? "" : "d-none";

            validForUnit.EnsureDataBound();

            var validForUnitItem = validForUnit.FindOptionByValue(row["ValidForUnit"] as string);
            if (validForUnitItem != null)
                validForUnitItem.Selected = true;

            priority.EnsureDataBound();
            priority.Value = row["Criticality"] as string;

            criticalIcon.Style["display"] = priority.Value == "Critical" ? "" : "none";
        }

        private void Levels_DataBinding(object sender, EventArgs e)
        {
            Levels.VirtualItemCount = Persistence.Plugin.CMDS.DepartmentProfileCompetencyRepository2.SelectCompetencyCount(DepartmentIdentifier);
            Levels.DataSource = Persistence.Plugin.CMDS.DepartmentProfileCompetencyRepository2.SelectCompetencies(DepartmentIdentifier, Levels.Paging);
        }

        private void Levels_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            SaveData();

            Levels.PageIndex = e.NewPageIndex;
            Levels.DataBind();
        }

        private void IsTimeSensitive_PreRender(object sender, EventArgs e)
        {
            var isTimeSensitive = (WebControl)sender;
            var item = (GridViewRow)isTimeSensitive.NamingContainer;
            var priority = (CompetencyPrioritySelector2)item.FindControl("Priority");
            var criticalIcon = (Icon)item.FindControl("CriticalIcon");
            var validForContainer = (HtmlGenericControl)item.FindControl("ValidForContainer");

            isTimeSensitive.Attributes["onclick"] = string.Format(
                "isTimeSensitiveChanged('{0}','{1}','{2}','{3}');",
                isTimeSensitive.ClientID, priority.ClientID, criticalIcon.ClientID, validForContainer.ClientID);

            priority.Attributes["onchange"] = string.Format("isPriorityChanged('{0}','{1}');",
                priority.ClientID, criticalIcon.ClientID);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveData();

            Levels.DataBind();

            ScreenStatus.AddMessage(AlertType.Success, "Your changes have been saved.");
        }

        private void SaveData()
        {
            var list = new List<DepartmentProfileCompetency>();

            foreach (GridViewRow item in Levels.Rows)
            {
                var isTimeSensitiveCtrl = (ICheckBoxControl)item.FindControl("IsTimeSensitive");
                var priorityCtrl = (CompetencyPrioritySelector2)item.FindControl("Priority");
                var competencyStandardIdentifierCtrl = (Label)item.FindControl("CompetencyStandardIdentifier");
                var profileStandardIdentifierCtrl = (Label)item.FindControl("ProfileStandardIdentifier");
                var validForCountCtrl = (ITextBox)item.FindControl("ValidForCount");
                var validForUnitCtrl = (ValidForUnitSelector2)item.FindControl("ValidForUnit");

                var isTimeSensitive = isTimeSensitiveCtrl.Checked;
                var priority = priorityCtrl.Value;
                var competencyStandardIdentifier = Guid.Parse(competencyStandardIdentifierCtrl.Text);
                var profileStandardIdentifier = Guid.Parse(profileStandardIdentifierCtrl.Text);
                var validForUnit = isTimeSensitive ? validForUnitCtrl.Value : null;
                var validForCount = isTimeSensitive && int.TryParse(validForCountCtrl.Text, NumberStyles.Any,
                                        Cultures.Default, out int validForCountTemp)
                    ? validForCountTemp
                    : (int?)null;

                var entity = DepartmentProfileCompetencySearch.SelectFirst(x =>
                    x.DepartmentIdentifier == DepartmentIdentifier && x.ProfileStandardIdentifier == profileStandardIdentifier &&
                    x.CompetencyStandardIdentifier == competencyStandardIdentifier);
                entity.IsCritical = priority == "Critical";

                if (isTimeSensitive)
                    entity.LifetimeMonths = validForUnit == ValidForUnits.Years ? 12 * validForCount : validForCount;
                else
                    entity.LifetimeMonths = null;

                list.Add(entity);
            }

            DepartmentProfileCompetencyStore.InsertUpdateDelete(null, list, null);
        }
    }
}