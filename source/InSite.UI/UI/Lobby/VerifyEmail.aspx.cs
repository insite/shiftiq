using System;

using InSite.Common;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Lobby
{
    public partial class VerifyEmail : Layout.Lobby.LobbyBasePage
    {
        private class UserInfo
        {
            public Guid UserIdentifier { get; set; }
            public string Email { get; set; }
            public string EmailVerified { get; set; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Title = LabelHelper.GetTranslation(ActionModel.ActionName);

            CloseLink.NavigateUrl = Common.Web.HttpRequestHelper.CurrentRootUrl;

            UserInfo user = GetUser();

            if (user != null)
            {
                CompleteEmailVerification(user);
                Success();
            }
            else
            {
                Failure();
            }
        }

        private UserInfo GetUser()
        {
            var thumbprint = GetThumbprint();
            if (thumbprint != null)
                return UserSearch.Bind(thumbprint.Value, x => new UserInfo
                {
                    UserIdentifier = x.UserIdentifier,
                    Email = x.Email,
                    EmailVerified = x.EmailVerified
                });
            return null;
        }

        private Guid? GetThumbprint()
        {
            try
            {
                var thumbprint = StringHelper.DecodeBase64Url(Request["thumbprint"]);
                if (Guid.TryParse(thumbprint, out var userId))
                    return userId;
            }
            catch
            {
                return null;
            }
            return null;
        }

        private void CompleteEmailVerification(UserInfo user)
        {
            if (string.Compare(user.EmailVerified, user.Email, StringComparison.OrdinalIgnoreCase) != 0)
            {
                var u = ServiceLocator.UserSearch.GetUser(user.UserIdentifier);
                u.EmailVerified = u.Email;
                UserStore.Update(u, null);
            }
        }

        private void Success()
        {
            ScreenStatus.AddMessage(
                AlertType.Success,
                Translate("Thank you for verifying your email address!"));
        }

        private void Failure()
        {
            ScreenStatus.AddMessage(
                AlertType.Error,
                Translate("Your email address verification link is expired or invalid. Please try again, and if you are still unable to verify your email address then contact your account administrator."));
        }
    }
}