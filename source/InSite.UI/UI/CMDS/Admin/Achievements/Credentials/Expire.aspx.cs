using System;
using System.Linq;

using Humanizer;

using InSite.Application.Records.Read;
using InSite.Cmds.Actions.BulkTool.Assign;
using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Constant;

using AlertType = Shift.Constant.AlertType;

namespace InSite.Cmds.Actions.BulkTool.Expiry
{
    public partial class Achievement : AdminBasePage
    {
        public override void ApplyAccessControl()
        {
            if (!Identity.IsGranted("cmds/users/expire-achievements"))
                CreateAccessDeniedException();

            FinderSecurityInfo.LoadPermissions();
        }

        private PersonFinderSecurityInfoWrapper _finderSecurityInfo;

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

            AchievementIdentifier.AutoPostBack = true;
            AchievementIdentifier.ValueChanged += AchievementSelector_ValueChanged;

            OperationScope.AutoPostBack = true;
            OperationScope.ValueChanged += OperationScope_ValueChanged;

            DepartmentFinder.AutoPostBack = true;
            DepartmentFinder.ValueChanged += (x, y) => BindCredentialStatus();

            OrganizationFinder.AutoPostBack = true;
            OrganizationFinder.ValueChanged += (x, y) => BindCredentialStatus();

            CredentialStatusPending.AutoPostBack = true;
            CredentialStatusPending.CheckedChanged += (x, y) => BindGradebookStatus();

            CredentialStatusValid.AutoPostBack = true;
            CredentialStatusValid.CheckedChanged += (x, y) => BindGradebookStatus();

            CredentialStatusExpired.AutoPostBack = true;
            CredentialStatusExpired.CheckedChanged += (x, y) => BindGradebookStatus();

            ExpireButton.Click += (x, y) => DoExpire();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                PageHelper.AutoBindHeader(this);

                AchievementIdentifier.Filter.OrganizationIdentifier = Identity.Organization.Identifier;
                AchievementIdentifier.Value = null;

                PersonFinder.Filter.OrganizationIdentifier = Identity.Organization.Identifier;

                if (!FinderSecurityInfo.CanSeeAllCompanyPeople && !Identity.HasAccessToAllCompanies)
                    PersonFinder.Filter.ParentUserIdentifier = User.UserIdentifier;

                if (!FinderSecurityInfo.CanSeeAllDepartments && !Identity.HasAccessToAllCompanies && !DepartmentFinder.HasValue)
                    PersonFinder.Filter.DepartmentsForParentId = User.UserIdentifier;

                PersonFinder.Filter.RoleType = new[] { MembershipType.Organization, MembershipType.Department };

                DepartmentFinder.Filter.OrganizationIdentifier = Identity.Organization.Identifier;

                if (!FinderSecurityInfo.CanSeeAllDepartments && !Identity.HasAccessToAllCompanies)
                    DepartmentFinder.Filter.UserIdentifier = User.UserIdentifier;

                InitializeFinders();
            }
        }

        private void AchievementSelector_ValueChanged(object o, EventArgs e)
        {
            OrganizationFinder.Filter.AchievementForEmployeeId = AchievementIdentifier.Value;
            OrganizationFinder.Filter.IncludeOrganizationIdentifiers =
                !FinderSecurityInfo.CanSeeAllCompanies && !Identity.HasAccessToAllCompanies
                    ? new[] { CurrentIdentityFactory.ActiveOrganizationIdentifier }
                    : null;
            OrganizationFinder.Value = null;
            OrganizationFinder.Enabled = true;

            DepartmentFinder.Filter.AchievementForEmployeeId = AchievementIdentifier.Value;
            DepartmentFinder.Value = null;
            DepartmentFinder.Enabled = true;

            PersonFinder.Filter.AchievementIdentifier = AchievementIdentifier.Value;
            PersonFinder.Value = null;
            PersonFinder.Enabled = true;

            BindCredentialStatus();
        }

        private void OperationScope_ValueChanged(object sender, EventArgs e)
        {
            InitializeFinders();
            AchievementSelector_ValueChanged(sender, e);
        }

        private void InitializeFinders()
        {
            PersonFinder.Visible = false;
            PersonRequired.Visible = false;

            DepartmentFinder.Visible = false;
            DepartmentRequired.Visible = false;

            OrganizationFinder.Visible = false;
            OrganizationRequired.Visible = false;

            switch (OperationScope.Value)
            {
                case "Person":
                    PersonFinder.Visible = true;
                    PersonFinder.Value = null;
                    PersonRequired.Visible = true;
                    break;

                case "Department":
                    DepartmentFinder.Visible = true;
                    DepartmentFinder.Value = null;
                    DepartmentRequired.Visible = true;
                    break;

                case "Organization":
                    OrganizationFinder.Visible = true;
                    OrganizationFinder.Value = null;
                    OrganizationRequired.Visible = true;
                    break;
            }

            Subject.Text = OperationScope.Value;
        }

        private void BindCredentialStatus()
        {
            CredentialStatus.Visible = false;

            var filter = new VCredentialFilter
            {
                AchievementIdentifier = AchievementIdentifier.Value,
                DepartmentIdentifier = DepartmentFinder.Value,
                OrganizationIdentifier = OrganizationFinder.Value
            };

            if (filter.AchievementIdentifier == null
                || (OperationScope.Value == "Person")
                || (OperationScope.Value == "Department" && filter.DepartmentIdentifier == null)
                || (OperationScope.Value == "Organization" && filter.OrganizationIdentifier == null)
                )
                return;

            var credentials = ServiceLocator.AchievementSearch.GetCredentials(filter);

            var pending = credentials.Count(c => c.CredentialStatus == "Pending");
            var valid = credentials.Count(c => c.CredentialStatus == "Valid");
            var expired = credentials.Count(c => c.CredentialStatus == "Expired");

            CredentialStatus.Visible = pending > 0 || valid > 0 || expired > 0;

            BindCredentialStatus(CredentialStatusPending, pending, "Pending");
            BindCredentialStatus(CredentialStatusValid, valid, "Valid");
            BindCredentialStatus(CredentialStatusExpired, expired, "Expired");
        }

        private void BindCredentialStatus(CheckBox box, int count, string status)
        {
            box.Enabled = count > 0;
            box.Text = $"{status} ({count:n0})";
        }

        private void BindGradebookStatus()
        {
            var learnerId = OperationScope.Value == "Person" ? PersonFinder.Value : null;

            var count = ServiceLocator.AchievementSearch.CountGradebookEnrollmentsForCredentials(
                learnerId,
                AchievementIdentifier.Value.Value,
                CredentialStatusPending.Checked,
                CredentialStatusValid.Checked,
                CredentialStatusExpired.Checked);

            GradebookStatus.Visible = count > 0;
            GradebookStatusEnrollment.Text = $"Course Enrollments ({count:n0})";
        }

        private void DoExpire()
        {
            if (!ValidateBulkExpiryOptions())
                return;

            var count = 0;
            var store = new VCmdsCredentialStore(ServiceLocator.SendCommand);

            switch (OperationScope.Value)
            {
                case "Person":
                    count = store.ExpireForLearner(PersonFinder.Value.Value, AchievementIdentifier.Value.Value, true, true, true);
                    break;

                case "Department":
                    count = store.ExpireForDepartment(DepartmentFinder.Value.Value, AchievementIdentifier.Value.Value, CredentialStatusPending.Checked, CredentialStatusValid.Checked, CredentialStatusExpired.Checked);
                    break;

                case "Organization":
                    count = store.ExpireForOrganization(OrganizationFinder.Value.Value, AchievementIdentifier.Value.Value, CredentialStatusPending.Checked, CredentialStatusValid.Checked, CredentialStatusExpired.Checked);
                    break;

                default:
                    return;
            }

            EditorStatus.AddMessage(AlertType.Success, "learner achievement".ToQuantity(count) + " expired.");
        }

        private bool ValidateBulkExpiryOptions()
        {
            if (!AchievementIdentifier.HasValue)
                EditorStatus.AddMessage(AlertType.Error, "You must select an achievement.");

            if (OperationScope.Value == "Person" && !PersonFinder.Value.HasValue)
                EditorStatus.AddMessage(AlertType.Error, "You must select a person.");

            else if (OperationScope.Value == "Department" && !DepartmentFinder.Value.HasValue)
                EditorStatus.AddMessage(AlertType.Error, "You must select a department.");

            else if (OperationScope.Value == "Organization" && !OrganizationFinder.Value.HasValue)
                EditorStatus.AddMessage(AlertType.Error, "You must select an organization.");

            return !EditorStatus.HasMessage;
        }
    }
}