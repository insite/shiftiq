using System;

using InSite.Application.Payments.Read;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Portal;
using InSite.Web.Security;

using Shift.Common;

namespace InSite.UI.Portal.Accounts.Users
{
    public partial class AcceptLicense : PortalBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AgreeButton.Click += (sender, args) => Licensed(DateTimeOffset.UtcNow);
            DisagreeButton.Click += (sender, args) => Licensed(null);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            BindModelToControls();
        }

        private void BindModelToControls()
        {
            PortalMaster.SidebarVisible(false);
            PortalMaster.HideBreadcrumbs();

            LicenseContainer.InnerHtml = AccountHelper.GetLicenseAgreement(true, true);

            var isReview = StringHelper.Equals(Request.QueryString["review"], "true");

            ButtonPanel.Visible = !Identity.IsImpersonating && !isReview;
            ImpersonatorPanel.Visible = Identity.IsImpersonating;
        }

        private void Licensed(DateTimeOffset? agreed)
        {
            User.UserLicenseAccepted = agreed;

            var user = ServiceLocator.UserSearch.GetUserByEmail(User.Email);
            user.UserLicenseAccepted = agreed;
            UserStore.Update(user, null);

            if (user != null)
            {
                if (agreed.HasValue)
                {
                    var url = GetReturnUrl();
                    HttpResponseHelper.Redirect(url);
                }
                else
                    Lobby.SignOut.Redirect(this, "User has not agreed to the license");
            }

            if (user == null)
                Lobby.SignOut.Redirect(this, "User is null");

            Web.SignIn.SignInLogic.Redirect();
        }

        protected override string GetReturnUrl()
        {
            if (Identity?.Organization?.Toolkits?.Sales?.ManagerGroup == null
                || !Identity.IsInGroup(Identity.Organization.Toolkits.Sales.ManagerGroup.Value)
                )
            {
                return "/ui/portal/home";
            }

            var hasPayments = ServiceLocator.PaymentSearch.CountPayments(new QPaymentFilter { CreatedBy = User.Identifier }) > 0;

            return hasPayments
                ? "/ui/portal/management/dashboard/home?added=1"
                : "/ui/portal/management/dashboard/home";
        }
    }
}