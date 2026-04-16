using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Lobby;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI.Navigation;

using PortalNavigation = InSite.UI.Layout.Portal.Controls.PortalHeader;

namespace InSite.UI.Layout.Admin
{
    public partial class AdminNavigation : AdminBaseControl
    {
        protected int SessionTimeountInMinutes => Session.Timeout;

        private static readonly NavigationRoot _navigationRoot = NavigationRootFactory.Create(
            ServiceLocator.Partition.Slug,
            url => TActionSearch.Get(url)?.PermissionParent?.ActionUrl
        );

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

            ImpersonatorAnchor.HRef = Urls.StopImpersonation;

            LoadPermissionMatrix();

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

            AdminMenu.LoadData();

            BindUser();
            BindImpersonator();
            BindHelpLink();
            PortalNavigation.BindLanguages(Identity.Organization.Languages, LanguageItem, CurrentLanguageOutput, LanguageMenuItems, Request.RawUrl, Request.Url.Query);

            BindSidebar(sidebar);
        }

        protected string GetRecentLinksKey()
        {
            var key = $"{ServiceLocator.AppSettings.Environment.Name}-{Organization.Identifier}-{User.Identifier}";
            var bytes = EncryptionHelper.ComputeHashMd5(key);
            return Convert.ToBase64String(bytes).Substring(0, 22);
        }

        private void BindUser()
        {
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

        private void BindSidebar(NavigationRoot sidebar)
        {
            var dataSource = sidebar?.GetAccessibleCategories(new NavigationIdentity(Identity, ServiceLocator.Partition.Slug));

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

            var links = (List<NavigationLink>)DataBinder.Eval(e.Item.DataItem, "Links");

            var itemRepeater = (Repeater)e.Item.FindControl("ItemRepeater");
            itemRepeater.ItemDataBound += ItemRepeater_ItemDataBound;
            itemRepeater.DataSource = links;
            itemRepeater.DataBind();

            e.Item.Visible = links.Count > 0;
        }

        private void ItemRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var categoryLink = (NavigationLink)e.Item.DataItem;
            var links = categoryLink.Links;

            var itemRepeater = (Repeater)e.Item.FindControl("SubItemRepeater");
            itemRepeater.Visible = links != null && links.Count > 0;
            itemRepeater.DataSource = links;
            itemRepeater.DataBind();
        }

        #region Access Control (improved)

        private void LoadPermissionMatrix()
        {
            if (!ServiceLocator.Partition.IsE03())
                return;

            // TODO: Use the new permission matrix in v26.1 to grant permission to users in the Keyera organization.
            var isKeyera = Organization.Code == "keyera";
            KeyeraHeading.Visible = isKeyera;
            KeyeraLinks.Visible = isKeyera;
        }

        #endregion
    }
}