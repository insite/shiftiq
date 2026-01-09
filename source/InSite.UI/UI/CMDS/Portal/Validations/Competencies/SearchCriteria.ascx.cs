using System;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;
using Shift.Constant;

using PersonFilter = InSite.Persistence.Plugin.CMDS.CmdsPersonFilter;

namespace InSite.Cmds.Controls.Talents.EmployeeCompetencies
{
    public partial class EmployeeCompetencySearchCriteria : SearchCriteriaController<EmployeeCompetencyFilter>
    {
        private static readonly string PermissionName = PermissionNames.Custom_CMDS_Workers;

        public override EmployeeCompetencyFilter Filter
        {
            get
            {
                var filter = new EmployeeCompetencyFilter { OrganizationIdentifier = OrganizationIdentifier.Value.Value };

                // Managers, Validators, Administrators, and Programmers can search across multiple employees. Employees can see
                // only their own competencies.

                if (CanSeeGroupCompetencies && SearchMode.Value == "Group")
                {
                    filter.ManagerUserIdentifier = GroupManagerID.Value ?? User.UserIdentifier;
                }
                else
                {
                    filter.UserIdentifier = Identity.IsGranted(PermissionName, PermissionOperation.Delete)
                        || Identity.IsGranted(PermissionName, PermissionOperation.Configure)
                            ? Employee.Value
                            : User.UserIdentifier;

                    filter.ProfileStandardIdentifier = CurrentProfile.Value;
                    filter.DepartmentIdentifier = Department.Value;
                }

                filter.Statuses = string.IsNullOrEmpty(Status.Value) ? null : new[] { Status.Value };
                filter.IsValidated = IsValidated.ValueAsBoolean;
                filter.Criticality = Priority.Value;
                filter.CategoryIdentifier = Category.ValueAsGuid;
                filter.Keyword = Keyword.Text;
                filter.SelfAssessmentStatus = SelfAssessmentStatus.Value;
                filter.Number = Number.Text;
                filter.NumberOld = NumberOld.Text;

                if (filter.ManagerUserIdentifier == null && filter.ProfileStandardIdentifier == null && Request["compliance"] == "1")
                    filter.IsComplianceRequired = true;

                filter.ShowValidationHistory = ShowValidationHistory.ValueAsBoolean ?? false;

                return filter;
            }
            set
            {
                var filter = value;

                OrganizationIdentifier.Value = filter.OrganizationIdentifier != Guid.Empty ? filter.OrganizationIdentifier : CurrentIdentityFactory.ActiveOrganizationIdentifier;

                InitSelectors();

                // Managers, Validators, Administrators, and Programmers can search across multiple employees. Employees can see
                // only their own competencies.

                SearchMode.Value = !CanSeeGroupCompetencies || filter.ManagerUserIdentifier == null ? "Person" : "Group";

                if (SearchMode.Value == "Group")
                    GroupManagerID.Value = filter.ManagerUserIdentifier ?? User.UserIdentifier;

                if (Identity.IsGranted(PermissionName, PermissionOperation.Delete)
                    || Identity.IsGranted(PermissionName, PermissionOperation.Configure)
                    || Identity.IsGranted(PermissionName, PermissionOperation.Delete)
                    || Identity.IsGranted(PermissionName, PermissionOperation.Configure)
                )
                    Employee.Value = filter.UserIdentifier;
                else
                    Employee.Value = User.UserIdentifier;

                CurrentProfile.Value = filter.ProfileStandardIdentifier;
                Status.Value = filter.Statuses.IsNotEmpty() ? filter.Statuses[0] : null;
                IsValidated.ValueAsBoolean = filter.IsValidated ?? IsValidated.ValueAsBoolean;
                Priority.Value = filter.Criticality;
                Category.ValueAsGuid = filter.CategoryIdentifier;
                Keyword.Text = filter.Keyword;
                SelfAssessmentStatus.Value = filter.SelfAssessmentStatus;
                Number.Text = filter.Number;
                NumberOld.Text = filter.NumberOld;

                if (CurrentProfile.Filter.ProfileUserIdentifier != Employee.Value)
                    CurrentProfile.Filter.ProfileUserIdentifier = Employee.Value;

                Department.Value = filter.DepartmentIdentifier;

                InitGroupPanelVisibility();

                ShowValidationHistory.ValueAsBoolean = filter.ShowValidationHistory;
            }
        }

        private bool CanSeeGroupCompetencies
        {
            get
            {
                if (ViewState[nameof(CanSeeGroupCompetencies)] == null)
                {
                    if (Identity.IsGranted(PermissionName, PermissionOperation.Delete)
                        || Identity.IsGranted(PermissionName, PermissionOperation.Configure)
                    )
                    {
                        ViewState[nameof(CanSeeGroupCompetencies)] = true;
                    }
                    else
                    {
                        var filter = new PersonFilter
                        {
                            OrganizationIdentifier = OrganizationIdentifier.Value.Value,
                            ParentUserIdentifier = User.UserIdentifier,
                            ExcludeUserIdentifier = User.UserIdentifier,
                            RelationWithParent = new[] { RelationCategory.Manager, RelationCategory.Supervisor }
                        };

                        var count = ContactRepository3.CountSearchResults(filter);

                        ViewState[nameof(CanSeeGroupCompetencies)] = count > 0;
                    }
                }

                return (bool)ViewState[nameof(CanSeeGroupCompetencies)];
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            OrganizationIdentifier.AutoPostBack = true;
            OrganizationIdentifier.ValueChanged += (x, y) => InitSelectors();

            Employee.AutoPostBack = true;
            Employee.ValueChanged += (x, y) =>
            {
                CurrentProfile.Filter.ProfileUserIdentifier = Employee.Value ?? User.UserIdentifier;
                CurrentProfile.Value = null;
            };

            SearchMode.AutoPostBack = true;
            SearchMode.ValueChanged += (x, y) => InitGroupPanelVisibility();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                InitSelectors();

                if (!string.IsNullOrEmpty(Request["validated"]))
                    IsValidated.ValueAsBoolean = ValueConverter.ToBoolean(Request["validated"]);
            }
        }

        private void InitSelectors()
        {
            InitCompanySelector();

            SearchModePanel.Visible = CanSeeGroupCompetencies;

            CurrentProfile.Filter.OrganizationIdentifier = OrganizationIdentifier.Value;
            CurrentProfile.Filter.ProfileUserIdentifier = User.UserIdentifier;

            Department.Filter.OrganizationIdentifier = OrganizationIdentifier.Value.Value;
            Department.Value = null;

            InitEmployeeSelector();
            InitGroupManagerSelector();

            InitGroupPanelVisibility();
        }

        private void InitCompanySelector()
        {
            var webUser = Identity;
            var canSee = webUser.IsInRole(CmdsRole.Programmers)
                         || webUser.IsInRole(CmdsRole.SystemAdministrators);

            var organizationId = canSee && OrganizationIdentifier.Value.HasValue ? OrganizationIdentifier.Value.Value : CurrentIdentityFactory.ActiveOrganizationIdentifier;

            OrganizationIdentifier.Value = organizationId;

            CompanyPanel.Visible = canSee;
        }

        private void InitEmployeeSelector()
        {
            Employee.Filter.KeyeraRoles = new[] { CmdsRole.Workers };
            Employee.Filter.OrganizationIdentifier = OrganizationIdentifier.Value.Value;

            Employee.Filter.ParentUserIdentifier =
                Identity.IsGranted(PermissionName, PermissionOperation.Delete)
                || Identity.IsGranted(PermissionName, PermissionOperation.Configure)
                || Identity.HasAccessToAllCompanies
                    ? (Guid?)null
                    : User.UserIdentifier
                ;

            if (!Employee.HasValue)
                Employee.Value = User.UserIdentifier;
        }

        private void InitGroupManagerSelector()
        {
            GroupManagerID.Filter.KeyeraRoles = new[] { CmdsRole.Supervisors, CmdsRole.Managers };

            GroupManagerID.Filter.OrganizationIdentifier = OrganizationIdentifier.Value.Value;

            GroupManagerID.Filter.ParentUserIdentifier =
                Identity.IsGranted(PermissionName, PermissionOperation.Delete)
                || Identity.IsGranted(PermissionName, PermissionOperation.Configure)
                || Identity.HasAccessToAllCompanies
                    ? (Guid?)null
                    : User.UserIdentifier
                ;

            if (!GroupManagerID.HasValue)
                GroupManagerID.Value = User.UserIdentifier;
        }

        private void InitGroupPanelVisibility()
        {
            GroupManagerPanel.Visible = SearchMode.Value == "Group"
                && (Identity.IsInRole(CmdsRole.Programmers) || Identity.IsInRole(CmdsRole.SystemAdministrators));
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            var isPersonSearchMode = SearchMode.Value == "Person";

            EmployeePanel.Visible = isPersonSearchMode;
            ProfilePanel.Visible = isPersonSearchMode;
            DepartmentPanel.Visible = isPersonSearchMode;
        }

        public void DisableCurrentStatus()
        {
            Status.Enabled = false;
        }

        public override void Clear()
        {
            OrganizationIdentifier.Value = CurrentIdentityFactory.ActiveOrganizationIdentifier;
            Employee.Value = User.UserIdentifier;
            CurrentProfile.Value = null;

            Status.ClearSelection();
            IsValidated.ValueAsBoolean = false;

            Priority.ClearSelection();

            Category.ValueAsGuid = null;

            SelfAssessmentStatus.ClearSelection();

            Keyword.Text = null;
            Number.Text = null;
            NumberOld.Text = null;
            Department.Value = null;
            SearchMode.Value = "Person";
            GroupManagerID.Value = User.UserIdentifier;

            InitGroupPanelVisibility();

            ShowValidationHistory.ValueAsBoolean = false;
        }
    }
}