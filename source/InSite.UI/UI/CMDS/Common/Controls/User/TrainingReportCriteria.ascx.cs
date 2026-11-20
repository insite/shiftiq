using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Custom.CMDS.Common.Controls.Server;
using InSite.Persistence;
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

        /// <summary>
        /// Whether the current organization uses divisions. Only meaningful once this control has
        /// loaded, so read it from a postback handler rather than from the page's own OnLoad.
        /// </summary>
        public bool DivisionsEnabled => DivisionPanel.Visible;

        public bool JobDivisionEnabled => JobDivisionSection.Visible;

        public Guid? DivisionValue => DivisionIdentifier.ValueAsGuid;

        public Guid[] DepartmentValues => FindDepartment.Values;

        /// <summary>
        /// The departments to filter on, where an empty array means "do not filter by department".
        /// A division narrows to that division's departments even when no department is picked.
        /// </summary>
        public Guid[] DepartmentFilterValues
        {
            get
            {
                var values = FindDepartment.Values;
                if (values.Length > 0)
                    return values;

                if (!DivisionValue.HasValue)
                    return values;

                return FindDepartment.GetDataItems().Select(x => x.Value).ToArray();
            }
        }

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

        public Guid[] SelectedAchievements => AchievementSelector.ResolveSelection().Achievements;

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

        public string JobDivisionMode => JobDivisionPresence.Value;

        public string JobDivisionFilter =>
            JobDivisionMode == "Without" ? null : JobDivisionValue.Value.NullIfEmpty();

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

            DivisionIdentifier.AutoPostBack = true;
            DivisionIdentifier.ValueChanged += (s, a) => OnDivisionChanged();

            FindDepartment.AutoPostBack = true;
            FindDepartment.ValueChanged += (s, a) => OnDepartmentChanged();

            FindProgram.AutoPostBack = true;
            FindProgram.ValueChanged += (s, a) => LoadAchievements();

            IsRequired.AutoPostBack = true;
            IsRequired.SelectedIndexChanged += (s, a) => LoadAchievements();

            AchievementSelectorValidator.ServerValidate += (s, a) =>
            {
                var selection = AchievementSelector.ResolveSelection();
                a.IsValid = selection.HasSelection;
            };

            JobDivisionPresence.ValueChanged += (s, a) => UpdateJobDivisionValueVisibility();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Page.IsPostBack)
                return;

            FindDepartment.Filter.OrganizationIdentifier = Organization.Identifier;

            if (!Identity.HasAccessToAllCompanies)
                FindDepartment.Filter.UserIdentifier = User.UserIdentifier;

            // After FindDepartment's filter is set, so the division list is scoped the same way as the
            // department cascade (a division appears only when the user can reach a department in it).
            SetupDivisions();

            JobDivisionSection.Visible = ServiceLocator.PersonSearch.HasAccessGrantedJobDivisions(Organization.Identifier);
            UpdateJobDivisionValueVisibility();

            OnDepartmentChanged();
        }

        private void UpdateJobDivisionValueVisibility()
        {
            JobDivisionValuePanel.Visible = JobDivisionMode != "Without";
        }

        private void SetupDivisions()
        {
            var organization = OrganizationSearch.Select(Organization.Identifier);

            DivisionPanel.Visible = OrganizationHelper.EnableDivisions(organization.CompanyDescription.CompanySize);

            // Only query and populate the division list when the organization uses divisions. Skips the
            // scoping round-trip entirely for organizations whose departments are not grouped into
            // divisions.
            if (DivisionPanel.Visible)
                DivisionScoping.Bind(DivisionIdentifier, FindDepartment, Organization.Identifier);
        }

        public bool ValidateNarrowSelection(out string error)
        {
            var departmentTotal = FindDepartment.GetDataItems().Count();
            var hasDivision = DivisionValue.HasValue;
            var allDepartmentsSelected = DepartmentValues.Length == 0 || DepartmentValues.Length >= departmentTotal;
            var isDepartmentWide = !hasDivision && allDepartmentsSelected;
            var selection = AchievementSelector.ResolveSelection();
            var isAchievementWide = selection.IsEveryTypeAll;

            if (isDepartmentWide && isAchievementWide)
            {
                error = "Narrow your selection. Pick a specific division, specific departments, or specific achievements. You cannot run the report with everything left wide open.";
                return false;
            }

            error = null;
            return true;
        }

        private void OnDivisionChanged()
        {
            FindDepartment.Filter.DivisionIdentifier = DivisionValue;
            FindDepartment.Value = null;

            OnDepartmentChanged();
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
                MessageRaised?.Invoke(AlertType.Error,
                    "The departments you have selected do not have any training achievements.");
        }
    }
}
