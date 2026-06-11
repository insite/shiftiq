using System;
using System.Collections.Generic;
using System.Linq;

using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Cmds.Controls.Reporting.Report
{
    public partial class TrainingReportCriteria : AdminBaseControl
    {
        public event Action<AlertType, string> MessageRaised;

        public string ValidationGroup
        {
            get => AchievementSelectorValidator.ValidationGroup;
            set => AchievementSelectorValidator.ValidationGroup = value;
        }

        public Guid[] DepartmentValues => FindDepartment.Values;

        public Guid[] EffectiveDepartmentValues
        {
            get
            {
                var values = FindDepartment.Values;
                if (values.Length > 0)
                    return values;
                return FindDepartment.GetDataItems().Select(x => x.Value).ToArray();
            }
        }

        public Guid[] LearnerValues => FindLearner.Values;

        public Guid[] SelectedAchievements => AchievementSelector.GetSelectedAchievements();

        public string[] MembershipFunctions
        {
            get
            {
                var functions = new List<string>();
                if (MembershipOrganization.Checked)
                    functions.Add("Organization");
                if (MembershipDepartment.Checked)
                    functions.Add("Department");
                return functions.ToArray();
            }
        }

        public string CredentialStatusFilter => CredentialStatus.Value;

        public bool ExcludeSelfDeclared => ExcludeSelfDeclaredCredentials.Checked;

        public DateTime? CompletedSinceFilter => CompletedSince.Value;

        public DateTime? CompletedBeforeFilter => CompletedBefore.Value;

        public bool? IsRequiredFilter
        {
            get
            {
                if (IsRequired.SelectedIndex <= 0)
                    return null;

                return bool.Parse(IsRequired.SelectedValue);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FindDepartment.AutoPostBack = true;
            FindDepartment.ValueChanged += (s, a) => OnDepartmentChanged();

            FindProgram.AutoPostBack = true;
            FindProgram.ValueChanged += (s, a) => LoadAchievements();

            IsRequired.AutoPostBack = true;
            IsRequired.SelectedIndexChanged += (s, a) => LoadAchievements();

            AchievementSelectorValidator.ServerValidate += (s, a) => a.IsValid = AchievementSelector.HasValue();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Page.IsPostBack)
                return;

            FindDepartment.Filter.OrganizationIdentifier = Organization.Identifier;

            if (!Identity.HasAccessToAllCompanies)
                FindDepartment.Filter.UserIdentifier = User.UserIdentifier;

            OnDepartmentChanged();
        }

        public bool ValidateNarrowSelection(out string error)
        {
            var departmentTotal = FindDepartment.GetDataItems().Count();
            var isDepartmentWide = DepartmentValues.Length == 0 || DepartmentValues.Length >= departmentTotal;
            var hasAchievement = AchievementSelector.HasValue();
            var allAchievementsSelected = AchievementSelector.IsAllSelected();
            var isAchievementWide = !hasAchievement || allAchievementsSelected;

            if (isDepartmentWide && isAchievementWide)
            {
                error = "Narrow your selection. Pick specific departments or achievements — you cannot run the report with both left wide open.";
                return false;
            }

            error = null;
            return true;
        }

        private void OnDepartmentChanged()
        {
            FindLearner.Enabled = FindDepartment.HasValue;
            FindLearner.Filter.OrganizationIdentifier = Organization.Identifier;
            FindLearner.Filter.GroupDepartmentIdentifiers = FindDepartment.Values;
            if (ServiceLocator.Partition.IsE03())
                FindLearner.Filter.GroupDepartmentFunctions = new[] { "Department" };
            FindLearner.Value = null;

            LoadAchievements();
        }

        private void LoadAchievements()
        {
            Guid[] departments = null;

            if (FindDepartment.Enabled)
            {
                departments = FindDepartment.Values;

                if (departments.Length == 0)
                    departments = FindDepartment.GetDataItems().Select(x => x.Value).ToArray();
            }

            var hasAchievements = AchievementSelector.LoadData(departments, IsRequiredFilter);

            AchievementSelector.Visible = hasAchievements;

            if (!hasAchievements)
                MessageRaised?.Invoke(AlertType.Error, "The departments you have selected do not have any training achievements.");
        }
    }
}
