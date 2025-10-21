using System;
using System.Linq;
using System.Web.UI;

using Humanizer;

using InSite.Admin.Contacts.People.Utilities;
using InSite.Application.Contacts.Read;
using InSite.Common.Web;
using InSite.Domain.Contacts;
using InSite.Persistence;
using InSite.UI.Admin.Reports.Changes.Models;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Accounts.Users.Forms
{
    public partial class Edit : AdminBasePage
    {
        private const string SearchUrl = "/ui/admin/accounts/users/search";

        #region Properties

        protected Guid UserID => Guid.TryParse(Request.QueryString["contact"], out var value) ? value : Guid.Empty;

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SendWelcomeEmailButton.Click += (sender, args) => OnSendWelcomeEmail();
            ResetPasswordButton.Click += (sender, args) => OnResetPassword();

            SaveButton.Click += SaveButton_Click;
        }

        public override void ApplyAccessControl()
        {
            base.ApplyAccessControl();

            CanEdit = CurrentSessionState.Identity.IsGranted(PermissionIdentifiers.Admin_Contacts, PermissionOperation.Write);

            SaveButton.Visible = CanEdit;
            DeleteButton.Visible = CanEdit && CanDelete;
            MembershipGrid.AllowEdit = CanEdit;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (Request.QueryString["status"] == "saved")
                SetStatus(ScreenStatus, StatusType.Saved);

            {
                var panel = Request.QueryString["panel"];
                var scrollToId = panel == "roles"
                    ? RolesSection.ClientID
                    : panel == "organizations"
                        ? OrganizationSection.ClientID
                        : null;

                if (scrollToId != null)
                    ScriptManager.RegisterStartupScript(Page, typeof(Edit), "scroll_to", $"personEditor.scrollTo('{scrollToId}');", true);
            }

            Open();

            ViewHistoryLink.OnClientClick = "personEditor.onViewHistoryClick(); return false;";
            DeleteButton.NavigateUrl = $"/ui/admin/accounts/users/delete?user={UserID}";
            ImpersonateButton.NavigateUrl = $"/ui/portal/identity/impersonate?user={UserID}";
            CancelButton.NavigateUrl = SearchUrl;
        }

        #endregion

        #region Event handlers

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            if (Save())
            {
                Open();

                SetStatus(ScreenStatus, StatusType.Saved);
            }
        }

        private void OnSendWelcomeEmail()
        {
            PersonHelper.SendWelcomeMessage(Organization.OrganizationIdentifier, UserID);

            ScreenStatus.AddMessage(AlertType.Success, "Welcome Email sent.");
        }

        private void OnResetPassword()
        {
            var entity = ServiceLocator.UserSearch.GetUser(UserID);
            entity.SetDefaultPassword();
            UserStore.Update(entity, OrganizationSearch.GetPersonFullNamePolicy(Organization.Identifier));

            PasswordExpires.Value = DateTime.UtcNow;
            PasswordExpires.DefaultTimeZone = CurrentSessionState.Identity.User.TimeZone.Id;

            ScreenStatus.AddMessage(
                AlertType.Information,
                $"<strong>Reset Password</strong> The password for this account has been reset to <strong>{entity.DefaultPassword}</strong>");
        }

        #endregion

        #region Load/Save

        private void Open()
        {
            var user = ServiceLocator.UserSearch.GetUser(UserID);
            if (user == null)
            {
                HttpResponseHelper.Redirect(SearchUrl);
                return;
            }

            var person = ServiceLocator.PersonSearch.GetPerson(user.UserIdentifier, Organization.Identifier);
            if (person == null)
                ScreenStatus.AddMessage(AlertType.Warning, "The user is not registered in this organization.");

            SetInputValues(user, person);
        }

        private bool Save()
        {
            var user = ServiceLocator.UserSearch.GetUser(UserID);
            var person = ServiceLocator.PersonSearch.GetPerson(UserID, Organization.Identifier);

            var contactState = Shift.Common.ObjectComparer.GetSnapshot(user, QUser.DiffExclusions);

            var utcApprovedBefore = person?.UserAccessGranted;
            var emailBefore = user.Email;

            GetInputValues(user, person);

            if (user.Email.IsEmpty())
            {
                ScreenStatus.AddMessage(AlertType.Error, "The email address must be specified for user.");
                return false;
            }

            if (UserSearch.IsEmailDuplicate(user.UserIdentifier, user.Email))
            {
                ScreenStatus.AddMessage(AlertType.Error, $"There is another user already registered with the email address <strong>{user.Email}</strong>.");
                return false;
            }

            var beforeUpdate = DateTimeOffset.UtcNow;

            UserStore.Update(user, OrganizationSearch.GetPersonFullNamePolicy(Organization.Identifier));

            if (person != null)
                PersonStore.Update(person);

            var isUpdated = user != null;

            if (isUpdated)
            {
                ShowChanges(beforeUpdate);

                if (person != null && person.UserAccessGranted.HasValue && utcApprovedBefore != person.UserAccessGranted && person.EmailEnabled)
                    OnSendWelcomeEmail();

                var changes = Shift.Common.ObjectComparer.Compare(contactState, user);

                var isNameChanged = changes.Any(x => x.PropertyName == "Name");
                if (isNameChanged)
                    ServiceLocator.ChangeQueue.Publish(new PersonRenamed(user.UserIdentifier, user.FirstName, user.LastName));

                var isEmailChanged = changes.Any(x => x.PropertyName == "Email");
                if (isEmailChanged)
                    ServiceLocator.ChangeQueue.Publish(new PersonEmailChanged(user.UserIdentifier, user.Email));
            }

            InSite.Web.SignIn.SignInLogic.UpdateLoginName(emailBefore, user.Email);

            return isUpdated;
        }

        private void ShowChanges(DateTimeOffset beforeUpdate)
        {
            var changeHtml = HistoryReader.ReadUserLatestAsHtml(UserID, Organization.Identifier, beforeUpdate);

            if (InfoStatus.Visible = changeHtml.IsNotEmpty())
                InfoStatus.Text = changeHtml;
        }

        #endregion

        #region Set/Get values

        private void SetInputValues(QUser user, QPerson person)
        {
            var hasPerson = person != null;
            var isAccessGranted = hasPerson && person.UserAccessGranted != null;

            {
                var title = user.FullName;

                if (hasPerson)
                    title += " <span class='form-text'>"
                        + UserSearch.GetTimestampHtml(person.ModifiedBy, "contact", "changed", person.Modified)
                        + "</span>";

                if (user.UtcArchived.HasValue)
                    title += " <span class='badge bg-custom-default'>Archived</span>";

                PageHelper.AutoBindHeader(Page, qualifier: title);
            }

            // User

            FirstName.Text = user.FirstName;
            LastName.Text = user.LastName;

            Email.Text = user.Email.EmptyIfNull().ToLower();
            Email.Enabled = true;
            EmailDisabled.Checked = !hasPerson || !person.EmailEnabled;
            EmailDisabled.Enabled = hasPerson;
            EmailCommand.Visible = user.Email.IsNotEmpty();
            EmailCommand.NavigateUrl = $"mailto:{user.Email}";
            SendWelcomeEmailButton.Visible = ServiceLocator.PersonSearch.IsPersonExist(UserID, Organization.OrganizationIdentifier);

            PhoneField.Visible = hasPerson;
            Phone.Text = hasPerson ? Shift.Common.Phone.Format(person.Phone) : null;

            TimeZone.Value = user.TimeZone;

            UserIdentifier.InnerText = user.UserIdentifier.ToString();

            IsApproved.Visible = hasPerson;

            IsApproved.Checked = isAccessGranted;
            if (isAccessGranted)
                IsApproved.Text = $"Approved <small class='text-body-secondary'>{person.UserAccessGranted.Humanize()} by {person.UserAccessGrantedBy ?? UserNames.Someone}</small>";
            else
                IsApproved.Text = "Approved";

            IsLicensed.Checked = user.UserLicenseAccepted.HasValue;
            IsLicensed.Text = user.UserLicenseAccepted.HasValue ? $"Licensed {user.UserLicenseAccepted.Value.Humanize()}" : "Licensed";

            if (user.UserPasswordExpired != null)
            {
                PasswordExpires.Enabled = true;
                PasswordExpires.DefaultTimeZone = CurrentSessionState.Identity.User.TimeZone.Id;
                PasswordExpires.Value = user.UserPasswordExpired;
            }
            else
            {
                PasswordExpires.Value = null;
            }

            LoginEmail.InnerText = user.Email;
            LoginOrganizationCode.Text = user.LoginOrganizationCode;

            ImpersonateButton.Visible = isAccessGranted;

            // Roles

            MembershipGrid.ReturnQuery = $"contact={UserID}&panel=roles";
            MembershipGrid.LoadData(UserID);

            // Default Organization

            IsAccessGrantedToCmds.Checked = user.AccessGrantedToCmds;

            // Accessible Organizations

            var allowViewOrganizations = Identity.IsOperator || Identity.IsInRole(CmdsRole.Programmers);
            if (allowViewOrganizations)
                OrganizationList.LoadData(UserID);

            OrganizationSection.Visible = allowViewOrganizations;
        }

        private void GetInputValues(QUser user, QPerson person)
        {
            user.FirstName = FirstName.Text;
            user.LastName = LastName.Text;
            user.AccessGrantedToCmds = IsAccessGrantedToCmds.Checked;
            user.TimeZone = TimeZone.Value;
            user.Email = Email.Text;

            if (IsLicensed.Checked && !user.UserLicenseAccepted.HasValue)
            {
                user.UserLicenseAccepted = DateTime.UtcNow;
            }
            else if (!IsLicensed.Checked && user.UserLicenseAccepted.HasValue)
            {
                user.UserLicenseAccepted = null;
            }

            if (PasswordExpires.Value.HasValue)
                user.UserPasswordExpired = PasswordExpires.Value.Value;

            user.LoginOrganizationCode = LoginOrganizationCode.Text;

            if (person != null)
            {
                person.EmailEnabled = !EmailDisabled.Checked;

                Phone.Text = Shift.Common.Phone.Format(Phone.Text);
                person.Phone = Phone.Text;

                if (IsApproved.Checked && !person.UserAccessGranted.HasValue)
                {
                    person.UserAccessGranted = DateTimeOffset.UtcNow;
                    person.UserAccessGrantedBy = User.FullName;
                }
                else if (!IsApproved.Checked)
                {
                    person.UserAccessGranted = null;
                    person.UserAccessGrantedBy = null;
                }

                if (person.UserAccessGranted.HasValue && user.IsNullPassword())
                    user.SetDefaultPassword();
            }
        }

        #endregion
    }
}