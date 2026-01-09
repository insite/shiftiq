using System;

using InSite.Common.Web.UI;
using InSite.Custom.CMDS.Common.Controls.Server;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;
using Shift.Constant;

namespace InSite.Cmds.Controls.Contacts.Persons
{
    public partial class PersonSearchCriteria : SearchCriteriaController<CmdsPersonFilter>
    {
        #region Security

        public void ApplySecurityPermissions()
        {
            ArchivedUsersPanel.Visible = Identity.IsOperator
                || Identity.IsInRole(CmdsRole.Programmers)
                || Identity.IsInRole(CmdsRole.SystemAdministrators);

            PersonAssignmentPanel.Visible = Identity.IsOperator
                || Identity.IsInRole(CmdsRole.Programmers)
                || Identity.IsInRole(CmdsRole.SystemAdministrators)
                || Identity.IsInRole(CmdsRole.OfficeAdministrators);
        }

        #endregion

        #region Properties

        public override CmdsPersonFilter Filter
        {
            get
            {
                var filter = new CmdsPersonFilter
                {
                    Name = Name.Text,
                    NameFilterType = NameFilterType.Value,
                    EmailWork = Email.Text,
                    PersonCode = PersonCode.Text,
                    PersonType = PersonType.Text,
                    IsApproved = Status.IsApproved,
                    DepartmentIdentifier = Department.Value,
                    IsArchived = ArchivedUsersPanel.Visible && IsArchived.ValueAsBoolean.HasValue && IsArchived.ValueAsBoolean.Value,
                    AccessGrantedToCmds = IsCmdsAccessGranted.ValueAsBoolean,
                    OrganizationIdentifier = Company.Visible ? Company.Value : CurrentIdentityFactory.ActiveOrganizationIdentifier
                };

                if (PersonAssignmentPanel.Visible)
                {
                    filter.RoleType = string.IsNullOrEmpty(PersonAssignment.Value)
                        ? null
                        : new[] { PersonAssignment.Value };
                }
                else
                {
                    filter.RoleType = new[] { MembershipType.Organization, MembershipType.Department };
                }

                filter.EmployeeType = EmployeeType.Text;

                var userId = User.UserIdentifier;

                // Apply this part of the filter only for non-programmers and non-system-administrators.  Programmers and system administrators should 
                // be able to find everyone in the database, regardless of whether or not they are assigned to corresponding departments.

                if (!Identity.HasAccessToAllCompanies)
                {
                    filter.DepartmentsForParentId = Department.HasValue ? (Guid?)null : userId;
                    filter.CompaniesForParentId = filter.OrganizationIdentifier.HasValue ? (Guid?)null : userId;
                }

                filter.EmailStatus = EmailStatus.Value;

                if (Role.ValueAsGuid.HasValue)
                    filter.KeyeraRoles = new string[] { Role.GetSelectedOption().Text };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                Name.Text = value.Name;
                NameFilterType.Value = value.NameFilterType;
                Email.Text = value.EmailWork;
                PersonCode.Text = value.PersonCode;
                PersonType.Text = value.PersonType;

                Status.EnsureDataBound();

                if (value.IsApproved.HasValue)
                    Status.Value = value.IsApproved.Value ? PersonStatusSelector.Approved : PersonStatusSelector.Disabled;

                Role.EnsureDataBound();
                Role.ClearSelection();

                if (value.KeyeraRoles.IsNotEmpty())
                {
                    var option = Role.FindOptionByText(value.KeyeraRoles[0]);
                    if (option != null)
                        option.Selected = true;
                }

                EmployeeType.Text = value.EmployeeType;

                Company.Value = value.OrganizationIdentifier;

                InitDepartment();

                Department.Value = value.DepartmentIdentifier;
                IsArchived.ValueAsBoolean = value.IsArchived;
                IsCmdsAccessGranted.ValueAsBoolean = value.AccessGrantedToCmds;
                PersonAssignment.Value = value.RoleType.IsNotEmpty() ? value.RoleType[0] : null;
                EmailStatus.Value = value.EmailStatus;
                EmailStatus.Value = value.EmailStatus;
            }
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Company.AutoPostBack = true;
            Company.ValueChanged += (s, a) => InitDepartment();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!Identity.HasAccessToAllCompanies)
                Company.Filter.UserIdentifier = User.UserIdentifier;

            Company.Visible =
                   Identity.IsOperator
                || Identity.IsInRole(CmdsRole.Programmers)
                || Identity.IsInRole(CmdsRole.SystemAdministrators)
                || Identity.IsInRole(CmdsRole.Validators);

            InitDepartment();

            PersonAssignment.FindOptionByValue(MembershipType.Administration).Visible =
                   Identity.IsOperator
                || Identity.IsInRole(CmdsRole.Programmers)
                || Identity.IsInRole(CmdsRole.SystemAdministrators)
                || Identity.IsInRole(CmdsRole.OfficeAdministrators);
        }

        #endregion

        #region Overriden methods

        public override void Clear()
        {
            Name.Text = null;
            NameFilterType.ClearSelection();
            Email.Text = null;
            PersonCode.Text = null;
            PersonType.Text = null;
            Role.ClearSelection();
            EmployeeType.Text = null;
            Status.ClearSelection();
            Company.Value = null;
            Department.Value = null;
            IsArchived.ValueAsBoolean = false;
            IsCmdsAccessGranted.ValueAsBoolean = true;
            PersonAssignment.ClearSelection();
            EmailStatus.ClearSelection();

            InitDepartment();
        }

        #endregion

        #region Helper methods

        private void InitDepartment()
        {
            if (Company.Visible)
                Department.Filter.OrganizationIdentifier = Company.Value ?? Guid.Empty;
            else
                Department.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;

            if (!Identity.HasAccessToAllCompanies)
                Department.Filter.UserIdentifier = User.UserIdentifier;

            Department.Value = null;
        }

        #endregion
    }
}