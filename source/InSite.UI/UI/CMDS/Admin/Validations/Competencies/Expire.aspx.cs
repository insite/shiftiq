using System;

using InSite.Cmds.Actions.BulkTool.Assign;
using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Constant;
using Shift.Sdk.UI;

using AlertType = Shift.Constant.AlertType;

namespace InSite.Cmds.Actions.BulkTool.Expiry
{
    public partial class Competency : AdminBasePage, ICmdsUserControl
    {
        private PersonFinderSecurityInfoWrapper _finderSecurityInfo;

        public override void ApplyAccessControl()
        {
            if (!Identity.IsGranted("cmds/users/expire-competencies"))
                CreateAccessDeniedException();

            FinderSecurityInfo.LoadPermissions();
        }

        private PersonFinderSecurityInfoWrapper FinderSecurityInfo
        {
            get
            {
                if (_finderSecurityInfo == null)
                    _finderSecurityInfo = new PersonFinderSecurityInfoWrapper(ViewState);

                return _finderSecurityInfo;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EntityType.AutoPostBack = true;
            EntityType.ValueChanged += EntityType_ValueChanged;

            ExpireButton.Click += ExpireButton_Click;

            CompetencySelector.AutoPostBack = true;
            CompetencySelector.ValueChanged += CompetencySelector_ValueChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                PageHelper.AutoBindHeader(this);

                Person.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;

                Person.Filter.ParentUserIdentifier = FinderSecurityInfo.CanSeeAllCompanyPeople || Identity.HasAccessToAllCompanies
                    ? (Guid?)null
                    : User.UserIdentifier;

                Person.Filter.DepartmentsForParentId = FinderSecurityInfo.CanSeeAllDepartments
                    || Identity.HasAccessToAllCompanies || Department.HasValue
                    ? (Guid?)null
                    : User.UserIdentifier;

                Person.Filter.RoleType = new[] { MembershipType.Organization, MembershipType.Department };

                Department.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;
                Department.Filter.UserIdentifier = FinderSecurityInfo.CanSeeAllDepartments || Identity.HasAccessToAllCompanies
                    ? (Guid?)null
                    : User.UserIdentifier;

                EntityType.FindOptionByValue("Organization").Visible = FinderSecurityInfo.CanSeeAllCompanies || Identity.HasAccessToAllCompanies;

                InitSelector();
            }
        }

        private void EntityType_ValueChanged(object sender, EventArgs e)
        {
            InitSelector();
        }

        private void ExpireButton_Click(object sender, EventArgs e)
        {
            DoExpire();
        }

        private void CompetencySelector_ValueChanged(object o, EventArgs e)
        {
            Company.Filter.CompetencyStandardIdentifier = CompetencySelector.Value;
            Company.Value = null;

            Department.Filter.CompetencyStandardIdentifier = CompetencySelector.Value;
            Department.Value = null;

            Person.Filter.CompetencyStandardIdentifier = CompetencySelector.Value;
            Person.Value = null;
        }

        private void InitSelector()
        {
            Person.Visible = false;
            PersonRequired.Visible = false;

            Department.Visible = false;
            DepartmentRequired.Visible = false;

            Company.Visible = false;
            CompanyRequired.Visible = false;

            switch (EntityType.Value)
            {
                case "Person":
                    Person.Visible = true;
                    Person.Value = null;
                    PersonRequired.Visible = true;
                    break;
                case "Department":
                    Department.Visible = true;
                    Department.Value = null;
                    DepartmentRequired.Visible = true;
                    break;
                case "Organization":
                    Company.Visible = true;
                    Company.Value = null;
                    CompanyRequired.Visible = true;
                    break;
            }

            Subject.Text = EntityType.Value;
        }

        private void DoExpire()
        {
            if (!CompetencySelector.Value.HasValue)
            {
                EditorStatus.AddMessage(AlertType.Error, "Competency is not selected.");
                return;
            }

            switch (EntityType.Value)
            {
                case "Person":
                    if (!Person.HasValue)
                        return;

                    UserCompetencyRepository.ExpireForPerson(Person.Value.Value, CompetencySelector.Value.Value);
                    break;

                case "Department":
                    if (!Department.HasValue)
                        return;

                    UserCompetencyRepository.ExpireForDepartment(Department.Value.Value, CompetencySelector.Value.Value);
                    break;

                case "Organization":
                    if (!Company.HasValue)
                        return;

                    UserCompetencyRepository.ExpireForCompany(Company.Value.Value, CompetencySelector.Value.Value);
                    break;

                default:
                    return;
            }

            EditorStatus.AddMessage(AlertType.Success, @"Your changes have been saved.");
        }

    }
}
