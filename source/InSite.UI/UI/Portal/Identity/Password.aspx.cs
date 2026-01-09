using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Accounts.Users
{
    public partial class ChangePassword : PortalBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CurrentSessionState.Identity.IsAuthenticated)
                HttpResponseHelper.SendHttp403();

            if (IsPostBack)
                return;

            Open();

            PageHelper.HideSideContent(Page);
        }

        private void Open()
        {
            PageHelper.AutoBindHeader(Page);

            var isPasswordExpired = UserPasswordCheck.IsPasswordExpired(User);
            PasswordExpiredHeadContent.Visible = isPasswordExpired;
            PasswordExpiredFooterContent.Visible = isPasswordExpired;
            CancelButton.Visible = !isPasswordExpired;

            CancelButton.NavigateUrl = "/ui/portal/profile";
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (Password.Text != PasswordConfirm.Text)
            {
                EditorStatus.AddMessage(AlertType.Error, Translate("Passwords do not match."), true);
                return;
            }

            if (!PasswordStrength.Validate())
            {
                EditorStatus.AddMessage(AlertType.Error, PasswordStrength.ValidationError, true);
                return;
            }

            var user = ServiceLocator.UserSearch.GetUser(User.UserIdentifier);
            var isOldPassword = PasswordHash.ValidatePassword(Password.Text, user.UserPasswordHash)
                || user.OldUserPasswordHash.IsNotEmpty() && PasswordHash.ValidatePassword(Password.Text, user.OldUserPasswordHash);

            if (isOldPassword)
            {
                EditorStatus.AddMessage(AlertType.Error, Translate("Your new password cannot be the same as any of your previous passwords. This is to enhance the security of your account and protect your data. Please choose a unique and strong password for your own safety."), true);
                return;
            }

            user.UserPasswordChangeRequested = null;
            user.OldUserPasswordHash = user.UserPasswordHash;
            user.SetPassword(Password.Text);

            UserStore.Update(user, null);

            CurrentIdentityFactory.Rebind(User.Email, Organization.Identifier);

            Open();

            EditorStatus.AddMessage(AlertType.Success, Translate("Your password has been changed."), true);

            ContentPanel.Visible = false;
            ButtonPanel.Visible = false;
            HomePanel.Visible = true;
            HomeButton.NavigateUrl = ServiceLocator.Urls.GetHomeUrl(Identity.User.AccessGrantedToCmds, ServiceLocator.Partition.IsE03(), Identity.IsAdministrator);
        }
    }
}