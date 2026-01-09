using System;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Domain.Foundations;
using InSite.Domain.Organizations;
using InSite.Persistence;
using InSite.UI.Portal.Billing.Models;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Layout.Portal.Controls
{
    public partial class PortalHeader : Lobby.LobbyBaseControl
    {
        private bool IsSalesReady => Page.Master is PortalMaster portalMaster && portalMaster.IsSalesReady;
        private bool IsManager => Page.Master is PortalMaster portalMaster && portalMaster.IsManagerGroupMember;

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (IsSalesReady)
                BindCartBadge();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var identity = CurrentSessionState.Identity;

            AdminMenu.Visible = identity != null && identity.IsActionAuthorized(RelativeUrl.AdminHomeUrl);

            if (AdminMenu.Visible)
                BindNavbar(AdminMenu);

            BindHomeLink(identity);
            BindImpersonator();
            BindLanguages(identity.Organization.Languages, LanguageItem, CurrentLanguageOutput, LanguageMenuItems, Request.RawUrl, Request.Url.Query);
            BindHelpMenu(identity.Organization);

            BindSalesNav(identity);
        }

        public static void BindNavbar(Control menu)
        {
            var identity = CurrentSessionState.Identity;

            foreach (var control in menu.Controls)
            {
                if (control is HtmlAnchor a)
                {
                    a.Visible = IsAccessGranted(a.HRef);
                }
                else if (control is HtmlGenericControl div)
                {
                    BindNavbar(div);
                }
            }

            bool IsAccessGranted(string href)
            {
                if (!href.StartsWith("/ui/"))
                    return true;

                var actionUrl = href.TrimStart('/');

                if (identity.IsActionAuthorized(actionUrl))
                    return true;

                var permission = TActionSearch.Get(actionUrl);
                if (permission == null)
                    return true;

                return identity.IsGranted(permission.PermissionParentActionIdentifier);
            }
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

        public static void BindLanguages(CultureInfo[] organizationLanguages, HtmlGenericControl menuItem, Literal currentLanguage, Repeater menuItems, string rawUrl, string query)
        {
            var multiLang = organizationLanguages.Length > 1;

            menuItem.Visible = multiLang;

            if (!multiLang)
                return;

            var current = CookieTokenModule.Current.Language;

            currentLanguage.Text = current.ToUpper();

            var index = rawUrl.IndexOf('?');
            var baseUrl = index > 0 ? rawUrl.Substring(0, index) : rawUrl;

            menuItems.DataSource = organizationLanguages
                .Where(x => !x.TwoLetterISOLanguageName.Equals(current, StringComparison.OrdinalIgnoreCase))
                .Select(cultureInfo =>
                {
                    var queryString = HttpUtility.ParseQueryString(query);
                    queryString.Remove("action-url");
                    queryString.Remove("path");

                    queryString["language"] = cultureInfo.TwoLetterISOLanguageName;

                    return new
                    {
                        Name = $"{char.ToUpper(cultureInfo.NativeName[0])}{cultureInfo.NativeName.Substring(1)}",
                        Url = $"{baseUrl}?{queryString}"
                    };
                });
            menuItems.DataBind();
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
            if (security.IsAuthenticated && IsSalesReady && IsSalesNavVisible())
            {
                SetSimpleMode(security);
                BindSalesLink(security);
            }

            if (!security.IsAuthenticated && IsSalesReady)
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

            PortalHomeItem.Visible = false;
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
            CartItem.Visible = IsManager;
            CartLink.HRef = cartUrl;

            if (!Request.IsAuthenticated)
            {
                UserNavItem.Visible = false;
                PortalHomeItem.Visible = false;
            }

            HelpMenuItem.Visible = false;
            AdminMenu.Visible = false;
            CmdsHomeItem.Visible = false;
            LanguageItem.Visible = false;

            HomeLink.Visible = true;

            var url = IsManager ? RelativeUrl.ManagerPortalHomeUrl : RelativeUrl.LearnerPortalHomeUrl;

            OverrideHomeLink(url);
        }

        private bool IsSalesNavVisible()
        {
            return Page.Master is PortalMaster portalMaster
                && (portalMaster.IsManagerGroupMember || portalMaster.IsLearnerGroupMember);
        }

        #endregion
    }
}