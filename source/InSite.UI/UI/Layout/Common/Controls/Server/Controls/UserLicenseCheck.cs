using System;
using System.Web.UI;

namespace InSite.Common.Web.UI
{
    public class UserLicenseCheck : Control
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
                CheckEULA();
        }

        private void CheckEULA()
        {
            const string url = "/ui/portal/identity/license";

            var identity = CurrentSessionState.Identity;

            // If the user is not authenticated then skip the EULA check.
            if (!identity.IsAuthenticated || identity.User == null)
                return;

            // If the user is impersonating someone else, then skip the EULA check.
            if (identity.IsImpersonating)
                return;

            // Avoid an infinite redirect loop.
            if (url.Equals(Page.Request.Url.AbsolutePath, StringComparison.OrdinalIgnoreCase))
                return;

            if (!ServiceLocator.Partition.IsE03() && !identity.User.UserLicenseAccepted.HasValue)
                Page.Response.Redirect(url);
        }
    }
}