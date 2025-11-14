using System;
using System.Globalization;
using System.Linq;
using System.Web;

using InSite.Common.Web;
using InSite.Domain.Foundations;
using InSite.Domain.Organizations;
using InSite.UI.Portal.Billing.Models;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Layout.Portal.Controls
{
    public partial class PortalHeader : Lobby.LobbyBaseControl
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (Page.Master is PortalMaster portalMaster && portalMaster.IsSalesReady)
                BindCartBadge();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var security = CurrentSessionState.Identity;

            BindHomeLink(security);
            BindImpersonator();
            BindLanguages(security.Organization.Languages);
            BindHelpMenu(security.Organization);

            BindSalesNav(security);
        }

        private void BindHomeLink(ISecurityFramework security)
        {
            var logoImageUrl = GetLogoImageUrl(security, Page.Server);

            var maxheight = "style='max-height:60px'";

            var p = (logoImageUrl == null)
                ? $"<i class='fas fa-share-alt-square me-2'></i> {ServiceLocator.Partition.GetPlatformName()}"
                : $"<img src='{logoImageUrl}' {maxheight} />";

            HomeLink.InnerHtml = p;
            HomeLink.Visible = Request.IsAuthenticated;

            if (string.IsNullOrEmpty(HomeLink.HRef))
                HomeLink.HRef = RelativeUrl.PortalHomeUrl;

            var isCmds = ServiceLocator.Partition.IsE03() && security.User != null && security.User.AccessGrantedToCmds;

            CmdsHomeLink.HRef = Urls.CmdsHomeUrl;
            CmdsHomeItem.Visible = isCmds;
            PortalHomeItem.Visible = !isCmds;
            AdminHomeItem.Visible = security.IsActionAuthorized(RelativeUrl.AdminHomeUrl);

            if (CmdsHomeItem.Visible && StringHelper.StartsWithAny(Request.RawUrl, new[] { "/ui/portal/accounts/my", "/ui/portal/reports/my" }))
                HomeLink.HRef = CmdsHomeLink.HRef;

            UserNavItem.Visible = Request.IsAuthenticated;
        }

        public void OverrideHomeLink(string url)
            => HomeLink.HRef = url;

        private void BindImpersonator()
        {
            var isImpersonation = CurrentSessionState.Identity.IsImpersonating;

            ImpersonatorItem.Visible = isImpersonation;

            if (isImpersonation)
                ImpersonatorName.Text = CurrentSessionState.Identity.Impersonator.User.FullName;
        }

        public static string GetLogoImageUrl(ISecurityFramework security, HttpServerUtility server)
            => WallpaperManager.GetLogoUrl(security.Organization.PlatformCustomization?.PlatformUrl?.Logo, security.Organization.Code);

        private void BindLanguages(CultureInfo[] organizationLanguages)
        {
            var multiLang = organizationLanguages.Length > 1;

            LanguageItem.Visible = multiLang;

            if (!multiLang)
                return;

            var current = CookieTokenModule.Current.Language;

            CurrentLanguageOutput.Text = current.ToUpper();

            var index = Request.RawUrl.IndexOf('?');
            var baseUrl = index > 0 ? Request.RawUrl.Substring(0, index) : Request.RawUrl;

            LanguageMenuItems.DataSource = organizationLanguages
                .Where(x => !x.TwoLetterISOLanguageName.Equals(current, StringComparison.OrdinalIgnoreCase))
                .Select(cultureInfo =>
                {
                    var queryString = HttpUtility.ParseQueryString(Request.Url.Query);
                    queryString.Remove("action-url");
                    queryString.Remove("path");

                    queryString["language"] = cultureInfo.TwoLetterISOLanguageName;

                    return new
                    {
                        Name = $"{char.ToUpper(cultureInfo.NativeName[0])}{cultureInfo.NativeName.Substring(1)}",
                        Url = $"{baseUrl}?{queryString}"
                    };
                });
            LanguageMenuItems.DataBind();
        }

        private void BindHelpMenu(OrganizationState organization)
        {
            var isResourcesVisible = organization.Identifier == OrganizationIdentifiers.Global;
            var isGetHelpVisible = Request.IsAuthenticated;

            HelpMenuItem.Visible = isResourcesVisible || isGetHelpVisible;

            ResourcesGroupContainer.Visible = isResourcesVisible;
            HelpAnchor.HRef = ServiceLocator.Urls.HelpUrl;

            GetHelpGroupContainer.Visible = isGetHelpVisible;
            SupportAnchor.HRef = $"/ui/portal/support?ref={Request.RawUrl}";
        }

        private void BindSalesNav(ISecurityFramework security)
        {
            if (security.IsAuthenticated && IsSalesReady() && IsSalesNavVisible())
            {
                SetSimpleMode(security);
                BindSalesLink(security);
            }

            if (!security.IsAuthenticated && IsSalesReady())
                BindSalesLink(security);
        }

        private void BindSalesLink(ISecurityFramework security)
        {
            var cartUrl = $"/ui/portal/billing/cart";

            BindDefaultSalesHeaderNavigation(cartUrl);

            var returnUrl = HttpUtility.UrlEncode(Request.RawUrl);
            var loginUrl = $"/ui/lobby/signin?ReturnUrl={returnUrl}";

            LoginItem.Visible = !security.IsAuthenticated;
            LoginLink.HRef = loginUrl;
        }

        #region Sales Nav

        private void SetSimpleMode(ISecurityFramework security)
        {
            if (security.IsAuthenticated)
                UserNav.SimpleMode = true;
        }

        public void BindCartBadge()
        {
            var cart = CartStorage.Get();
            int count = cart.TotalSelected;

            CartBadge.Visible = (count > 0);
            if (CartBadge.Visible)
                CartBadge.InnerText = count.ToString();

            CartLink.Attributes["aria-label"] = (count > 0) ? $"Cart ({count})" : "Cart";
        }

        private void BindDefaultSalesHeaderNavigation(string cartUrl)
        {
            CartItem.Visible = true;
            CartLink.HRef = cartUrl;

            UserNavItem.Visible = Request.IsAuthenticated;
            HelpMenuItem.Visible = false;
            AdminHomeItem.Visible = false;
            CmdsHomeItem.Visible = false;
            PortalHomeItem.Visible = false;
            LanguageItem.Visible = false;

            HomeLink.Visible = true;
            OverrideHomeLink("/");
        }

        private bool IsSalesNavVisible()
        {
            return Page.Master is PortalMaster portalMaster
                && (portalMaster.IsManagerGroupMember || portalMaster.IsLearnerGroupMember);
        }

        private bool IsSalesReady()
        {
            return Page.Master is PortalMaster portalMaster
                && portalMaster.IsSalesReady;
        }

        #endregion
    }
}