using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Common.Web.Cmds;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;
using Shift.Constant;

namespace InSite.Cmds.Controls.Talents.EmployeeCompetencies
{
    public partial class EmployeeCompetencySearchResults : SearchResultsGridViewController<EmployeeCompetencyFilter>
    {
        public EmployeeCompetencyEditorType EditorType { get; set; }

        private bool IsCompetenciesToValidate => EditorType == EmployeeCompetencyEditorType.Validation;

        private static readonly string PermissionName = PermissionNames.Custom_CMDS_Workers;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowDataBound += Grid_RowDataBound;
        }

        private void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            var row = (DataRowView)e.Row.DataItem;
            var standard = (Guid)row["CompetencyStandardIdentifier"];
            var user = (Guid)row["UserIdentifier"];
            var validForUnit = row["ValidForUnit"] as string;
            var validForCount = row["ValidForCount"] as int?;
            var priorityName = row["PriorityName"] as string;
            var validationStatus = row["ValidationStatus"] as string;
            var isTimeSensitive = validForCount.HasValue;

            var parameters = user == User.UserIdentifier
                ? string.Format("id={0}", standard)
                : string.Format("id={0}&userID={1}", standard, user);

            var link = (HyperLink)e.Row.FindControl("NumberLink");

            string action;

            switch (EditorType)
            {
                case EmployeeCompetencyEditorType.AdminEditor:
                    action = "/ui/cmds/admin/validations/competencies/edit";
                    break;
                case EmployeeCompetencyEditorType.Validation:
                    action = "/ui/cmds/portal/validations/competencies/validate";
                    break;
                default:
                    action = "/ui/cmds/portal/validations/competencies/edit";
                    break;
            }

            link.NavigateUrl = action + "?" + parameters;

            // Display time-sensitivity

            var spanIsTimeSensitive = (Icon)e.Row.FindControl("IsTimeSensitive");
            var spanIsCritical = (Icon)e.Row.FindControl("IsCritical");
            var spanIsValidated = (Icon)e.Row.FindControl("IsValidated");
            var spanIsNotValidated = (Icon)e.Row.FindControl("IsNotValidated");

            TimeSensitivityHelper.SetTimeSensitiveIcon(isTimeSensitive, validForUnit, validForCount, spanIsTimeSensitive);
            spanIsCritical.Visible = priorityName == "Critical";
            spanIsValidated.Visible = priorityName == "Critical" && (validationStatus == "Validated" || validationStatus == "Not Applicable");
            spanIsNotValidated.Visible = priorityName == "Critical" && !spanIsValidated.Visible;

            // Display the status history for each competency
            var history = (HtmlGenericControl)e.Row.FindControl("ValidationHistory");
            history.Visible = Filter.ShowValidationHistory;
            if (history.Visible)
            {
                var repeater = (Repeater)e.Row.FindControl("ValidatorCommentGrid");
                repeater.DataSource = StandardValidationChangeSearch.SelectStatusHistory(standard, user);
                repeater.DataBind();
            }
        }


        protected override int SelectCount(EmployeeCompetencyFilter filter)
        {
            return UserCompetencyRepository.CountSearchResults(filter, GetValidator(), GetUser());
        }

        protected override IListSource SelectData(EmployeeCompetencyFilter filter)
        {
            return UserCompetencyRepository.SelectSearchResultsPaged(filter, GetValidator(), GetUser());
        }

        protected string GetCategories(Guid competencyId)
        {
            if (competencyId == Guid.Empty)
                return null;

            var categories = StandardClassificationSearch
                .Bind(x => x.CategoryIdentifier, x => x.StandardIdentifier == competencyId)
                .Select(x => TCollectionItemCache.GetName(x))
                .Where(x => x.IsNotEmpty())
                .OrderBy(x => x)
                .ToList();

            if (categories.Count == 0)
                return null;

            var html = new StringBuilder();

            foreach (var category in categories)
                html.Append($"<div>{category}</div>");

            return html.ToString();
        }

        private Guid? GetUser()
        {
            return Identity.IsGranted(PermissionName, PermissionOperation.Delete)
                || Identity.IsGranted(PermissionName, PermissionOperation.Configure)
                    ? (Guid?)null
                    : User.UserIdentifier;
        }

        private Guid? GetValidator()
        {
            return IsCompetenciesToValidate
                && !Identity.IsGranted(PermissionName, PermissionOperation.Configure)
                    ? User.UserIdentifier
                    : (Guid?)null;
        }
    }
}