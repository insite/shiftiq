using System;
using System.Data;
using System.Web.UI;

using InSite.Common.Web.Cmds;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Constant.CMDS;

namespace InSite.Cmds.Actions.Contact.Company.Competency.Popup
{
    public partial class Setting : AdminBasePage
    {
        #region Properties

        private Guid? DepartmentIdentifier => Guid.TryParse(Request["departmentID"], out var value) ? value : (Guid?)null;

        private Guid? CompetencyStandardIdentifier => Guid.TryParse(Request["competencyID"], out var value) ? value : (Guid?)null;

        private Guid? ProfileStandardIdentifier => Guid.TryParse(Request["profileID"], out var value) ? value : (Guid?)null;

        #endregion

        #region Initialization & Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
            CopyButton.Click += CopyButton_Click;
            DeleteButton.Click += DeleteButton_Click;
            SetLevelButton.Click += SetLevelButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (DepartmentIdentifier.HasValue && CompetencyStandardIdentifier.HasValue && ProfileStandardIdentifier.HasValue)
            {
                var isOnly = string.Equals(Request["saveOption"], "Only", StringComparison.OrdinalIgnoreCase);

                OnlySaveOption.Checked = isOnly;
                AllSaveOption.Checked = !isOnly;

                Open();
            }
            else
            {
                ContentContainer.Visible = false;

                ScreenStatus.AddMessage(AlertType.Error, "Invalid input parameters.");
            }
        }

        #endregion

        #region Event handlers

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var cs = Save();

            string timeSensitiveText = TimeSensitivityHelper.GetTimeSensitiveText(ValidForUnits.Months, cs.LifetimeMonths);

            bool isCritical = StringHelper.Equals(Priority.Value, "Critical");

            string script = string.Format("closeWithoutDelete({0}, {1}, '{2}');",
                                                 cs.LifetimeMonths.HasValue.ToString().ToLower(), isCritical.ToString().ToLower(), timeSensitiveText);

            Page.ClientScript.RegisterStartupScript(GetType(), "CloseWindow", script, true);
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            Save();

            Page.ClientScript.RegisterStartupScript(GetType(), "CloseWindow", "closeWithCopy();", true);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            DeleteProfileCompetency();

            string script = string.Format("closeWithDelete('{0}');", CompetencyStandardIdentifier);

            Page.ClientScript.RegisterStartupScript(GetType(), "CloseWindow", script, true);
        }

        private void SetLevelButton_Click(object sender, EventArgs e)
        {
            Save();

            Page.ClientScript.RegisterStartupScript(GetType(), "CloseWindow", "closeWithSetLevel();", true);
        }

        #endregion

        #region Open & Save

        private void Open()
        {
            var entity = DepartmentProfileCompetencySearch.SelectFirst(x => x.DepartmentIdentifier == DepartmentIdentifier.Value && x.ProfileStandardIdentifier == ProfileStandardIdentifier.Value && x.CompetencyStandardIdentifier == CompetencyStandardIdentifier.Value);

            var department = DepartmentSearch.Select(DepartmentIdentifier.Value);

            SetInputValues(entity);

            var competencyInfo = Persistence.Plugin.CMDS.CompetencyRepository.Select(CompetencyStandardIdentifier.Value);

            CopySettingsInstructions.Text = $"Click the Copy button to copy the settings for this competency " +
                $"({competencyInfo.Number}) in this department ({department.DepartmentName}) " +
                $"to all other departments in this organization.";

            CopyButton.OnClientClick = $"return confirm('Are you sure you want to copy the settings for this competency " +
                $"({competencyInfo.Number}) in this department ({department.DepartmentName}) " +
                $"to all other departments in this organization?')";

            var profileInfo = StandardSearch.Select(ProfileStandardIdentifier.Value);

            RemoveCompetencyInstructions.Text = $"Click the Delete button to remove competency " +
                $"#{competencyInfo.Number} from this profile #{profileInfo.Code}.";

            DeleteButton.OnClientClick = $"return confirm('Are you sure you want to remove competency " +
                $"#{competencyInfo.Number} from this profile #{profileInfo.Code}?')";

            HelpText.Text = $"These are the properties assigned to competency " +
                $"#{competencyInfo.Number} in the {department.DepartmentName} department (irrespective of profile).";
        }

        private DepartmentProfileCompetency Save()
        {
            bool exists = true;

            var entity = DepartmentProfileCompetencySearch.SelectFirst(x =>
                x.DepartmentIdentifier == DepartmentIdentifier && x.ProfileStandardIdentifier == ProfileStandardIdentifier &&
                x.CompetencyStandardIdentifier == CompetencyStandardIdentifier);

            if (entity == null)
            {
                exists = false;
                entity = new DepartmentProfileCompetency
                {
                    DepartmentIdentifier = DepartmentIdentifier.Value,
                    ProfileStandardIdentifier = ProfileStandardIdentifier.Value,
                    CompetencyStandardIdentifier = CompetencyStandardIdentifier.Value
                };
            }

            GetInputValues(entity);

            if (AllSaveOption.Checked)
                SaveAllProfiles(entity);
            else if (exists)
                DepartmentProfileCompetencyStore.Update(entity);
            else
                DepartmentProfileCompetencyStore.Insert(entity);

            return entity;
        }

        private void SaveAllProfiles(DepartmentProfileCompetency entity)
        {
            DataTable table = Persistence.Plugin.CMDS.ProfileRepository.SelectProfilesForDepartment(DepartmentIdentifier.Value, entity.CompetencyStandardIdentifier);

            foreach (DataRow row in table.Rows)
            {
                var profileStandardIdentifier = (Guid)row["ProfileStandardIdentifier"];
                var hasCompetency = (bool)row["HasCompetency"];

                entity.ProfileStandardIdentifier = profileStandardIdentifier;

                if (hasCompetency)
                {
                    DepartmentProfileCompetencyStore.Update(entity);
                }
                else
                {
                    DepartmentProfileCompetencyStore.Insert(entity);
                }
            }
        }

        private void DeleteProfileCompetency()
        {
            StandardContainmentStore.Delete(x => x.ParentStandardIdentifier == ProfileStandardIdentifier && x.ChildStandardIdentifier == CompetencyStandardIdentifier);
        }

        #endregion

        #region Setting and getting input values

        private void SetInputValues(DepartmentProfileCompetency entity)
        {
            var isTimeSensitive = entity != null && entity.LifetimeMonths.HasValue;

            IsTimeSensitive.Checked = isTimeSensitive;
            ValidForCount.ValueAsInt = entity?.LifetimeMonths;
            ValidForUnit.Value = ValidForUnits.Months;
            Priority.Value = entity != null && entity.IsCritical ? "Critical" : "Non-Critical";

            ValidForCount.Enabled = isTimeSensitive;
            ValidForUnit.Enabled = isTimeSensitive;
        }

        private void GetInputValues(DepartmentProfileCompetency entity)
        {
            if (IsTimeSensitive.Checked && ValidForCount.ValueAsInt.HasValue)
                entity.LifetimeMonths = ValidForUnit.Value == ValidForUnits.Years ? 12 * ValidForCount.ValueAsInt : ValidForCount.ValueAsInt;
            else
                entity.LifetimeMonths = null;

            entity.IsCritical = Priority.Value == "Critical";
        }

        #endregion
    }
}
