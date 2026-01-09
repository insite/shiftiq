using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Common.Controls.Navigation.Models;
using InSite.UI.Layout.Lobby;

using Shift.Common;
using Shift.Constant;

using PortalNavigation = InSite.UI.Layout.Portal.Controls.PortalHeader;

namespace InSite.UI.Layout.Admin
{
    public partial class AdminNavigation : AdminBaseControl
    {
        protected int SessionTimeountInMinutes => Session.Timeout;

        private static readonly NavigationRoot _navigationRoot = NavigationRoot.GetSidebarInstance();

        private static readonly ConcurrentDictionary<string, bool> _validUrlCache = new ConcurrentDictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

        private static bool TestUrl(string url)
        {
            try
            {

                var request = WebRequest.Create(url);
                request.Method = "HEAD";
                using (var response = (HttpWebResponse)request.GetResponse())
                    return response.StatusCode == HttpStatusCode.OK;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsUrlValid(string imgUrl)
        {
            return _validUrlCache.GetOrAdd(imgUrl, (url) =>
            {
                var absUrl = HttpRequestHelper.GetAbsoluteUrl(url);

                return TestUrl(absUrl);
            });
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SidebarCategoryRepeater.ItemDataBound += SidebarCategoryRepeater_ItemDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var sidebar = _navigationRoot;
            if (sidebar?.Home != null)
            {
                var home = sidebar.Home;
                var icon = home.Image.IsNotEmpty() && IsUrlValid(home.Image)
                    ? $"<img style='width:24px; max-height:24px;' class='me-2' src='{HttpUtility.UrlPathEncode(home.Image)}' />"
                    : $"<i class='{home.Icon} me-1'></i>";

                var homeLink = $"<a href='{home.Href}' class='text-light' title='Home'>{icon}<span class='hide-compact'>{home.Text}</span></a>";
                HomeContainer1.InnerHtml = homeLink;
                HomeContainer2.InnerHtml = homeLink;
                HomeContainer3.InnerHtml = homeLink;
            }

            var isE03 = ServiceLocator.Partition.IsE03();

            AdminMenu.Visible = Identity.IsGranted(PermissionNames.Admin_Courses);

            if (AdminMenu.Visible)
                PortalNavigation.BindNavbar(AdminMenu);

            BindUser();
            BindImpersonator();
            BindHelpLink();
            PortalNavigation.BindLanguages(Identity.Organization.Languages, LanguageItem, CurrentLanguageOutput, LanguageMenuItems, Request.RawUrl, Request.Url.Query);
            BindSetupLink();

            BindSidebar(sidebar?.Menu);

            BindThemeMode();
        }

        private void BindThemeMode()
        {
            var mode = ModeSwitch.GetCurrentThemeMode();
            if (mode == "Dark" && Page.Master is AdminHome home)
                home.BindThemeMode(mode);
        }

        private void BindUser()
        {
            RecentMenu.Visible = RecentLinkCache.IsVisible(Page.Session);

            MyDashboardLink.HRef = RelativeUrl.PortalHomeUrl;
            MyDashboardLink.InnerText = GetDisplayText("My Dashboard");
            MyDashboardLink.Visible = Organization.Toolkits.Portal.ShowMyDashboard;

            var identity = CurrentSessionState.Identity;

            // You can select another environment only if you are an administrator in the current organization account.

            SelectEnvironmentItem.Visible = identity.Person.IsAdministrator;

            // You can select another organization only if you are a multi-organization user.

            var isMultiOrganization = identity.Persons.Count(x => x.IsAdministrator || x.IsLearner) > 1;
            if (isMultiOrganization)
            {
                SelectOrganizationLink.HRef = "/ui/portal/security/organizations?auto-redirect=0";
                SelectOrganizationItem.Visible = true;
            }

            SignOutLink.HRef = InSite.UI.Lobby.SignOut.GetUrl();
            SignOutLink2.HRef = InSite.UI.Lobby.SignOut.GetUrl();

            UserNameAnchor.InnerHtml = "<i class='fa-regular fa-user me-2 fa-width-auto'></i>" + User.FirstName;
        }

        private void BindImpersonator()
        {
            var identity = CurrentSessionState.Identity;
            ImpersonatorMenu.Visible = identity.IsImpersonating;
            if (identity.IsImpersonating)
                ImpersonatorName.Text = identity.Impersonator.User.FullName;
        }

        private void BindHelpLink()
        {
            if (Page is LobbyBasePage lobbyPage && lobbyPage.ActionModel != null)
                ActionHelpAnchor.InnerText = lobbyPage.ActionModel.ActionName;

            ShiftContainer.Visible = Request.IsAuthenticated && !ServiceLocator.Partition.IsE03();
            CmdsContainer.Visible = Request.IsAuthenticated && ServiceLocator.Partition.IsE03();
        }

        private void BindSetupLink()
        {
            var allIntegrations = CurrentSessionState.Identity.IsGranted(PermissionIdentifiers.Admin_Integrations);
            var allAccounts = CurrentSessionState.Identity.IsGranted(PermissionIdentifiers.Admin_Accounts);
            var allSettings = CurrentSessionState.Identity.IsGranted(PermissionIdentifiers.Admin_Settings);

            AdminMenuUtilities.Visible = allIntegrations || allAccounts || allSettings;
            AdminIntegrationLink.Visible = allIntegrations;
            AdminSecurityLink.Visible = allAccounts;
            AdminSetupLink.Visible = allSettings;
            AdminTimelineLink.Visible = allAccounts;
        }

        private void BindSidebar(IEnumerable<NavigationCategory> categories)
        {
            var dataSource = categories?
                .Where(c => c.IsAccessPermitted(Identity))
                .Select(c => new
                {
                    Category = c.Category,
                    Items = c.Links.Where(l => l.IsAccessPermitted(Identity)).ToArray()
                })
                .Where(x => x.Items.Length > 0)
                .ToArray();

            SidebarCategoryRepeater.Visible = dataSource != null && dataSource.Any();
            SidebarCategoryRepeater.DataSource = dataSource;
            SidebarCategoryRepeater.DataBind();

            var parts = ServiceLocator.AppSettings.Release.Version.Split('.');
            if (parts.Length > 2)
                ReleaseVersion.Text = $"v{parts[0]}.{parts[1]}.{parts[2]}";
        }

        private void SidebarCategoryRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var items = (NavigationLink[])DataBinder.Eval(e.Item.DataItem, "Items");

            var itemRepeater = (Repeater)e.Item.FindControl("ItemRepeater");
            itemRepeater.ItemDataBound += ItemRepeater_ItemDataBound;
            itemRepeater.DataSource = items;
            itemRepeater.DataBind();

            e.Item.Visible = items.Length > 0;
        }

        private void ItemRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var link = (NavigationLink)e.Item.DataItem;
            var items = link.Links?.Where(l => l.IsAccessPermitted(Identity)).ToArray();

            var itemRepeater = (Repeater)e.Item.FindControl("SubItemRepeater");
            itemRepeater.Visible = items != null && items.Length > 0;
            itemRepeater.DataSource = items;
            itemRepeater.DataBind();
        }
    }
}