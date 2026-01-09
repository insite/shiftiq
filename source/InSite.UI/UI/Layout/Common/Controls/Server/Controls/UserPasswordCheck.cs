using System;
using System.Web;
using System.Web.UI;

using InSite.Application.Users.Write;
using InSite.Domain.Contacts;
using InSite.Persistence;
using InSite.UI.Lobby;

namespace InSite.Common.Web.UI
{
    public class UserPasswordCheck : Control
    {
        public const string ChangePasswordUrl = "/ui/portal/identity/password";

        public const int PasswordChangeRequestLimit = 3;

        private static readonly string _isPasswordChangeRequestLimitExceededKey =
            typeof(UserPasswordCheck).FullName + "." + nameof(IsPasswordChangeRequestLimitExceeded);

        public static bool IsPasswordChangeRequestLimitExceeded
        {
            get => (bool?)HttpContext.Current.Session[_isPasswordChangeRequestLimitExceededKey] ?? false;
            private set => HttpContext.Current.Session[_isPasswordChangeRequestLimitExceededKey] = value;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
                CheckPasswordExpired();
        }

        private void CheckPasswordExpired()
        {
            var identity = CurrentSessionState.Identity;
            if (!identity.IsAuthenticated || identity.IsImpersonating || ServiceLocator.Partition.IsE03())
                return;

            if (identity.User == null || !IsPasswordExpired(identity.User))
                return;

            if (!ChangePasswordUrl.Equals(Page.Request.Url.AbsolutePath, StringComparison.OrdinalIgnoreCase))
            {
                var requestsNumber = UserSearch.Bind(identity.User.UserIdentifier, x => x.UserPasswordChangeRequested);

                if (requestsNumber >= PasswordChangeRequestLimit)
                {
                    IsPasswordChangeRequestLimitExceeded = true;
                    SignOut.Redirect(Page, "Password change request limit exceeded");
                }

                Page.Response.Redirect(ChangePasswordUrl, true);
            }
            else
            {
                var user = UserSearch.Select(identity.User.UserIdentifier);

                ServiceLocator.SendCommand(new ModifyUserFieldInt(
                    identity.User.UserIdentifier,
                    UserField.UserPasswordChangeRequested,
                    (user.UserPasswordChangeRequested ?? 0) + 1)
                );
            }
        }

        public static bool IsPasswordExpired(Domain.Foundations.User user)
        {
            return user.PasswordExpiry.HasValue && user.PasswordExpiry.Value <= DateTimeOffset.UtcNow;
        }

        public static bool IsPasswordExpired(Persistence.User user)
        {
            return user.UserPasswordExpired <= DateTimeOffset.UtcNow;
        }
    }
}