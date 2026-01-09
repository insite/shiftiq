using System;

using InSite.Admin.Contacts.People.Utilities;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Accounts.Users
{
    public partial class GrantAccess : PortalBasePage
    {
        private class UserInfo
        {
            public Guid UserIdentifier { get; internal set; }
            public string FullName { get; internal set; }
            public string Email { get; internal set; }
            public bool IsAccessGranted { get; internal set; }
            public string Reason { get; internal set; }
        }

        private Guid UserID => Guid.TryParse(Request["user"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            IsAccessGranted.AutoPostBack = true;
            IsAccessGranted.CheckedChanged += IsAccessGranted_CheckedChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            if (UserID == Guid.Empty)
            {
                ScreenStatus.AddMessage(
                    AlertType.Error,
                    Translate("The URL for this page must include a user identifier."));
                return;
            }

            var user = UserSearch.Bind(UserID, x => new UserInfo
            {
                UserIdentifier = x.UserIdentifier,
                FullName = x.FullName,
                Email = x.Email,
            });

            if (user == null)
            {
                ScreenStatus.AddMessage(
                    AlertType.Error,
                    Translate("User Not Found: ") + UserID);
                return;
            }

            var person = PersonSearch.Select(Organization.Identifier, user.UserIdentifier);

            user.IsAccessGranted = person?.UserAccessGranted != null;
            user.Reason = person?.UserApproveReason;

            ApproveFullName.InnerHtml = person != null
                ? $"<a href=\"/ui/admin/contacts/people/edit?contact={user.UserIdentifier}\">{user.FullName}</a>"
                : $"<a href=\"/ui/admin/accounts/users/edit?contact={user.UserIdentifier}\">{user.FullName}</a>";

            ApproveEmail.InnerText = user.Email;

            IsAccessGranted.Checked = user.IsAccessGranted;
            IsAccessGranted.Enabled = person != null;

            FormRow.Visible = true;
            ApproveReason.Text = user.Reason;
        }

        private void IsAccessGranted_CheckedChanged(object sender, EventArgs e)
        {
            var person = ServiceLocator.PersonSearch.GetPerson(UserID, Organization.Identifier);
            if (person == null)
            {
                ScreenStatus.AddMessage(
                    AlertType.Error,
                    Translate("User Not Found: ") + UserID);
                return;
            }

            var isPasswordReset = false;
            var user = ServiceLocator.UserSearch.GetUser(UserID);

            person.UserApproveReason = ApproveReason.Text;

            if (IsAccessGranted.Checked)
            {
                person.UserAccessGranted = DateTime.UtcNow;
                person.UserAccessGrantedBy = User.FullName;

                if (user.IsNullPassword())
                {
                    user.SetDefaultPassword();
                }
                else if (user.IsDefaultPassword() && user.DefaultPasswordExpired < DateTimeOffset.UtcNow)
                {
                    var password = RandomStringGenerator.CreateUserPassword();

                    user.SetPassword(password);
                    user.SetDefaultPassword(password);

                    isPasswordReset = true;
                }
            }
            else
            {
                person.UserAccessGranted = null;
                person.UserAccessGrantedBy = null;
            }

            UserStore.Update(user, null);
            PersonStore.Update(person);

            if (person.UserAccessGranted.HasValue && person.EmailEnabled)
                PersonHelper.SendWelcomeMessage(Organization.OrganizationIdentifier, user.UserIdentifier);

            if (person.UserAccessGranted.HasValue)
                ScreenStatus.AddMessage(
                    AlertType.Success,
                    $"{user.FullName} {Translate("is now approved for access to this system")}");
            else
                ScreenStatus.AddMessage(
                    AlertType.Warning,
                    $"{user.FullName} {Translate("is no longer approved for access to this system")}");

            if (isPasswordReset)
                ScreenStatus.AddMessage(AlertType.Success, "The password has been reset.");
        }
    }
}