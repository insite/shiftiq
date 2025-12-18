using System;
using System.Linq;
using System.Web;
using System.Web.UI;

using InSite.Admin.Contacts.People.Utilities;
using InSite.Application.Contacts.Read;
using InSite.Application.Records.Read;
using InSite.Cmds.Actions.BulkTool.Assign;
using InSite.Cmds.Admin.People.Controls;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Messages;
using InSite.Persistence;
using InSite.UI.Admin.Records.Programs.Utilities;
using InSite.UI.Layout.Admin;
using InSite.Web.Data;

using Shift.Common;
using Shift.Constant;
using Shift.Contract;
using Shift.Sdk.UI;

namespace InSite.Cmds.Admin.People.Forms
{
    public partial class Edit : AdminBasePage, ICmdsUserControl
    {
        #region Constants

        private const string SearchUrl = "/ui/cmds/admin/users/search";
        private const string CreateUrl = "/ui/cmds/admin/users/create";

        private const string FieldsTool = PermissionNames.Custom_CMDS_Fields;
        private const string SupervisorsAction = "cmds/users/assign-supervisors";
        private const string ValidatorsAction = "cmds/users/assign-validators";

        #endregion

        #region Properties

        private Guid UserIdentifier => Guid.TryParse(Request.QueryString["userid"], out var value) ? value : User.UserIdentifier;

        private PersonFinderSecurityInfoWrapper _finderSecurityInfo;
        private PersonFinderSecurityInfoWrapper FinderSecurityInfo =>
            _finderSecurityInfo ?? (_finderSecurityInfo = new PersonFinderSecurityInfoWrapper(ViewState));

        private bool CanArchive
        {
            get => ViewState[nameof(CanArchive)] != null && (bool)ViewState[nameof(CanArchive)];
            set => ViewState[nameof(CanArchive)] = value;
        }

        private bool CanUnarchive
        {
            get => ViewState[nameof(CanUnarchive)] != null && (bool)ViewState[nameof(CanUnarchive)];
            set => ViewState[nameof(CanUnarchive)] = value;
        }

        #endregion

        #region Security

        public override void ApplyAccessControlForCmds()
        {
            base.ApplyAccessControlForCmds();

            FinderSecurityInfo.LoadPermissions();

            if (Access.Delete)
                Access = Access.SetDelete(
                    UserIdentifier != User.UserIdentifier
                    && Identity.IsGranted(PermissionNames.Custom_CMDS_Administrators)
                    && Identity.IsInRole(CmdsRole.Programmers), false);

            Access = Access.SetCreate(Access.Administrate || Access.Configure, false);

            ApplySecurityPermissionsToSections();

            EmploymentUpdatePanel.Visible = EmploymentGrid.ApplySecurityPermissions();

            CanDelete = Access.Delete;
            DeleteButton.Visible = CanDelete;

            CanUnarchive = Identity.IsInRole(CmdsRole.Programmers) || Identity.IsInRole(CmdsRole.SystemAdministrators);
            CanArchive = CanUnarchive;
        }

        private void ApplySecurityPermissionsToSections()
        {
            UserDepartments.Visible = true;
            UserDepartments.ApplyPermissions();

            CompanyTab.Visible = UserDepartments.Visible;

            var isLeaderModuleEnabled = Identity.IsGranted(FieldsTool);
            var isManagerModuleEnabled = Identity.IsGranted(FieldsTool);
            var isSupervisorModuleEnabled = Identity.IsGranted(SupervisorsAction);
            var isValidatorModuleEnabled = Identity.IsGranted(ValidatorsAction);

            UserConnections.Visible = isLeaderModuleEnabled || isManagerModuleEnabled || isSupervisorModuleEnabled || isValidatorModuleEnabled;

            ManagerTab.Visible = UserConnections.Visible;

            UserConnections.SetSelectionEnabled(isLeaderModuleEnabled, isManagerModuleEnabled, isSupervisorModuleEnabled, isValidatorModuleEnabled);
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
            DeleteButton.Click += DeleteButton_Click;

            ArchiveButton.Click += (s, a) => ArchiveUnarchive(true);
            UnarchiveButton.Click += (s, a) => ArchiveUnarchive(false);

            ResetPasswordButton.Click += ResetPasswordButton_Click;

            UserConnections.Alert += (s, a) => ScreenStatus.AddMessage(a);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (Request["new"].IsNotEmpty())
                ScreenStatus.AddMessage(
                    AlertType.Success,
                    "Your changes have been saved. Please remember to assign this person to a department and to a manager and/or validator.");

            Open();

            CancelButton.NavigateUrl = SearchUrl;
            ViewHistoryButton.OnClientClick = $"showHistory('{UserIdentifier}'); return false;";
            EducationLinkButton.NavigateUrl = $"/ui/cmds/portal/achievements/credentials/search?userID={UserIdentifier}";
            ProfilesLinkButton.NavigateUrl = $"/ui/cmds/portal/validations/profiles/search?userID={UserIdentifier}";
            CompetenciesLinkButton.NavigateUrl = $"/ui/cmds/portal/validations/competencies/search?userID={UserIdentifier}";
            TrainingPlanLinkButton.NavigateUrl = $"/ui/portal/learning/plan?userID={UserIdentifier}";
        }

        #endregion

        #region Event handlers

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (!Save())
                return;

            Open();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (Delete())
                HttpResponseHelper.Redirect(SearchUrl);
        }

        private void ResetPasswordButton_Click(object sender, EventArgs e)
        {
            if (UserIdentifier == null)
                return;

            var user = ServiceLocator.UserSearch.GetUser(UserIdentifier);
            if (user == null || !user.AccessGrantedToCmds)
                return;

            user.SetDefaultPassword(Default.CmdsPassword);
            UserStore.Update(user, null);

            PersonDetails.SetPasswordExpires(user.UserPasswordExpired);

            var script = $"alert('The password for this account has been reset to {Default.CmdsPassword}. {user.FirstName} will be prompted to change this password on next login.');";
            ScriptManager.RegisterStartupScript(Page, typeof(Page), "ResetPassword", script, true);
        }

        #endregion

        #region Database operations

        private void Open()
        {
            var user = UserSearch.Select(UserIdentifier);
            if (user == null)
                HttpResponseHelper.Redirect(SearchUrl);

            var person = PersonSearch.Select(Organization.Identifier, UserIdentifier);
            if (person == null)
                ShowOrganizationWarning();

            var isGrantedUsers = Identity.IsGranted(PermissionNames.Custom_CMDS_Users);
            var addNewItem = Access.Create ? new BreadcrumbItem("Add New Person", CreateUrl, null, null) : null;

            var name = user.FullName +
                (person?.EmployeeType != null
                    ? $" <span class='form-text'>{person.EmployeeType}</span>"
                    : "");

            if (isGrantedUsers)
                PageHelper.AutoBindHeader(this, addNewItem, name);
            else
                PageHelper.BindHeader(this, null, addNewItem, name);

            PersonDetails.SetInputValues(user, person);
            PersonDetails.ShowTimeZone();

            UserConnections.LoadData(user.UserIdentifier, user.FirstName, FinderSecurityInfo);
            UserDepartments.LoadData(user.UserIdentifier, user.FirstName + " " + user.LastName);

            NoRolesWarningPanel.Visible = PersonDetails.IsUserDetailsVisible && !PersonDetails.HasRole;

            if (NoRolesWarningPanel.Visible)
            {
                NoRolesWarning.Text = $"{user.FullName} is not assigned a role in the system.";

                if (!Page.IsPostBack)
                {
                    var script = string.Format(@"setTimeout(""alert ('{0}')"", 1500);", NoRolesWarning.Text);
                    Page.ClientScript.RegisterStartupScript(GetType(), "wraning", script, true);
                }
            }

            EmploymentGrid.LoadData(UserIdentifier);

            var isUserArchived = user.UtcArchived.HasValue;
            ArchiveButton.Visible = !isUserArchived && CanArchive;
            UnarchiveButton.Visible = isUserArchived && CanUnarchive;

            var allowViewHistory = Identity.IsInRole(CmdsRole.SystemAdministrators)
                || Identity.IsInRole(CmdsRole.Programmers);

            ViewHistoryButton.Visible = allowViewHistory;
            AdditionalLinksSpacer.Visible = allowViewHistory && isGrantedUsers;
            EducationLinkButton.Visible = isGrantedUsers;
            ProfilesLinkButton.Visible = isGrantedUsers;
            CompetenciesLinkButton.Visible = isGrantedUsers;
            TrainingPlanLinkButton.Visible = isGrantedUsers;

            ImpersonateLink.NavigateUrl = "/ui/portal/identity/impersonate?user=" + user.UserIdentifier +
                          "&ReturnUrl=" + HttpUtility.UrlEncode(Urls.CmdsHomeUrl);

            ImpersonateLink.Visible = !Identity.IsImpersonating                 // A user who is already impersonating someone is not allowed to impersonate someone else.
                                      && Identity.IsInRole(CmdsRole.Impersonators)
                                      && !string.IsNullOrEmpty(user.Email)
                                      && person != null
                                      && person.UserAccessGranted.HasValue
                                      && user.UserIdentifier != User.UserIdentifier;

            ResetPasswordButton.Visible = person != null && person.UserAccessGranted.HasValue;
        }

        private void ShowOrganizationWarning()
        {
            NoPerson.Visible = true;

            CurrentOrganizationName.Text = Organization.CompanyName;

            OrganizationRepeater.DataSource = PersonSearch.GetOrganizationNames(UserIdentifier);
            OrganizationRepeater.DataBind();
        }

        private bool Save()
        {
            var isApprovedBefore = false;

            var user = ServiceLocator.UserSearch.GetUser(UserIdentifier);
            var person = ServiceLocator.PersonSearch.GetPerson(UserIdentifier, Organization.Identifier, x => x.HomeAddress)
                ?? UserFactory.CreatePerson(Organization.Identifier);

            isApprovedBefore = person.UserAccessGranted.HasValue;

            PersonDetails.GetInputValues(user, person);

            if (UserSearch.IsEmailDuplicate(user.UserIdentifier, user.Email))
            {
                ScreenStatus.AddMessage(
                    AlertType.Error,
                    $"There is another user already registered with the email address <strong>{user.Email}</strong>.");

                return true;
            }

            UserStore.Update(user, OrganizationSearch.GetPersonFullNamePolicy(Organization.Identifier));

            if (person.PersonIdentifier == Guid.Empty)
            {
                person.UserIdentifier = user.UserIdentifier;
                PersonStore.Insert(person);
            }
            else
                PersonStore.Update(person);

            PersonDetails.SaveRoles(user.UserIdentifier);

            if (person.UserAccessGranted.HasValue && !isApprovedBefore)
                ScreenStatus.AddMessage(
                    AlertType.Success,
                    $"Your changes have been saved. <strong>Please remember to send a welcome email to {user.FirstName}.</strong>");
            else
                SetStatus(ScreenStatus, StatusType.Saved);

            return true;
        }

        private bool Delete()
        {
            PersonStore.Delete(UserIdentifier, Organization.OrganizationIdentifier);
            UserStore.Delete(UserIdentifier);
            return true;
        }

        private void ArchiveUnarchive(bool isArchive)
        {
            var user = ServiceLocator.UserSearch.GetUser(UserIdentifier);

            var helper = new ArchiveHelper(User.FullName);

            if (isArchive)
            {
                helper.CmdsArchive(user, Organization.OrganizationIdentifier);

                UnEnrollUser(user);

                ServiceLocator.AlertMailer.Send(Organization.OrganizationIdentifier, user.UserIdentifier, new AlertUserAccountArchived
                {
                    TenantIdentifier = OrganizationIdentifiers.CMDS,
                    Email = user.Email,
                    Name = user.FullName,
                    Status = "Archived"
                });

                HttpResponseHelper.Redirect(SearchUrl);
            }
            else
            {
                helper.CmdsUnarchive(user);

                ServiceLocator.AlertMailer.Send(Organization.OrganizationIdentifier, user.UserIdentifier, new AlertUserAccountArchived
                {
                    TenantIdentifier = OrganizationIdentifiers.CMDS,
                    Email = user.Email,
                    Name = user.FullName,
                    Status = "Not Archived"
                });

                Open();
            }
        }

        private void UnEnrollUser(QUser user)
        {
            var organizations = OrganizationSearch.Search(new OrganizationFilter()).Select(x => x.OrganizationIdentifier).ToArray();
            if (organizations == null || organizations.Length == 0)
                return;

            var programsEnrollment = ProgramSearch1.GetUserProgramEnrollments(user.UserIdentifier, organizations);
            if (programsEnrollment == null)
                return;

            foreach (var programEnrollment in programsEnrollment)
            {
                var tasks = TaskStore.DeleteEnrollments(Organization.Identifier, programEnrollment.ProgramIdentifier, UserIdentifier);

                if (tasks != null && tasks.Length > 0)
                {
                    EnsureCourseEnrollmentDeletion(UserIdentifier, tasks);
                    EnsureLogbookEnrollmentDeletion(UserIdentifier, tasks);
                }

                ProgramStore.DeleteEnrollment(programEnrollment.ProgramIdentifier, UserIdentifier);
            }
        }

        private static void EnsureLogbookEnrollmentDeletion(Guid userIdentifier, TTask[] tasks)
        {
            foreach (var task in tasks.Where(x => x.ObjectType == "Logbook"))
                ProgramHelper.EnsureLogbookEnrollmentDeletion(userIdentifier, task.ObjectIdentifier, Organization.Identifier);
        }

        private static void EnsureCourseEnrollmentDeletion(Guid userIdentifier, TTask[] tasks)
        {
            foreach (var task in tasks.Where(x => x.ObjectType == "Course"))
                ProgramHelper.EnsureCourseEnrollmentDeletion(userIdentifier, task.ObjectIdentifier, Organization.Identifier);
        }

        #endregion

        #region Methods (helpers)

        protected string GetLocalTime(string name)
        {
            var dataItem = Page.GetDataItem();
            var value = (DateTimeOffset?)DataBinder.Eval(dataItem, name);

            return value.Format(User.TimeZone);
        }

        #endregion
    }
}