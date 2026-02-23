using System;
using System.Web.UI;

namespace InSite.Common.Web.UI
{
    public class UserEmailVerificationCheck : Control
    {
        private const string VerifyEmailUrl = "/ui/lobby/verify-email";

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Page.IsPostBack || Page.Request.Url.AbsolutePath.Equals(VerifyEmailUrl, StringComparison.OrdinalIgnoreCase))
                return;

            if (ServiceLocator.Partition.IsE03())
                return;

            var identity = CurrentSessionState.Identity;
            if (identity == null || !identity.IsAuthenticated || identity.IsImpersonating)
                return;

            if (identity.Organization == null || !identity.Organization.PlatformCustomization.RequireEmailVerification)
                return;

            if (identity.User == null
             || string.Equals(identity.User.EmailVerified, identity.User.Email, StringComparison.OrdinalIgnoreCase))
                return;

            HttpResponseHelper.Redirect(VerifyEmailUrl);
        }
    }
}