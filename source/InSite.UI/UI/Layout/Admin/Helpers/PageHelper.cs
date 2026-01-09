using System;
using System.Collections.Generic;
using System.Web.UI;

using InSite.UI.Layout.Lobby;
using InSite.UI.Layout.Portal;

using Shift.Common;
using Shift.Contract;

namespace InSite.UI.Layout.Admin
{
    public static class PageHelper
    {
        public static void AutoBindHeader(Page page, BreadcrumbItem create = null, string qualifier = null, string linkTitle = null)
        {
            if (!(page is LobbyBasePage lobbyPage) || lobbyPage.Route == null)
                return;

            var translator = (page as LobbyBasePage)?.Translator;
            var overrideWebRouteParent = page as IOverrideWebRouteParent;
            var hasParentLinkParameters = page as IHasParentLinkParameters;

            var breadcrumbs = BreadcrumbsHelper.CollectBreadcrumbs(lobbyPage.Route, linkTitle, translator, overrideWebRouteParent, hasParentLinkParameters);

            BindHeader(page, breadcrumbs.ToArray(), create, qualifier);
        }

        public static void AutoBindFolderHeader(
            PortalBasePage page,
            BreadcrumbItem create,
            Guid? folderId,
            string qualifier = null,
            string linkTitle = null,
            string[] parentLinkTitles = null
            )
        {
            if (page.Route == null)
                return;

            var folder = folderId.HasValue ? ServiceLocator.PageSearch.GetPage(folderId.Value) : null;
            if (folder == null)
            {
                AutoBindHeader(page, create, qualifier, linkTitle);
                return;
            }

            if (page.Master is PortalMaster master)
                master.Breadcrumbs.RootText = page.Translate("Home");

            if (string.IsNullOrEmpty(linkTitle))
                linkTitle = page.Translate(page.Route.LinkTitle);

            var folderUrl = ServiceLocator.PageSearch.GetPagePath(folder.PageIdentifier, false);
            var language = CookieTokenModule.Current.Language;
            var folderTitle = ServiceLocator.ContentSearch.GetTitleText(folder.PageIdentifier, language).IfNullOrEmpty(folder.Title);

            var breadcrumbs = CreateFolderParentItems(page, parentLinkTitles);

            breadcrumbs.Insert(0, new BreadcrumbItem(folderTitle, folderUrl));
            breadcrumbs.Add(new BreadcrumbItem(linkTitle, null, null, "active"));

            BindHeader(page, breadcrumbs.ToArray(), create, qualifier);
        }

        private static List<BreadcrumbItem> CreateFolderParentItems(PortalBasePage page, string[] parentLinkTitles)
        {
            var translator = (page as LobbyBasePage)?.Translator;
            var overrideWebRouteParent = page as IOverrideWebRouteParent;
            var hasParentLinkParameters = page as IHasParentLinkParameters;

            var breadcrumbs = new List<BreadcrumbItem>();
            BreadcrumbsHelper.AddBreadcrumbParentItems(page.Route, breadcrumbs, translator, overrideWebRouteParent, hasParentLinkParameters);

            if (parentLinkTitles != null)
            {
                for (int i = 0, j = breadcrumbs.Count - 1; i < parentLinkTitles.Length; i++, j--)
                    breadcrumbs[j].Text = parentLinkTitles[i];
            }

            foreach (var b in breadcrumbs)
                b.Href = page.AddFolderToUrl(b.Href);

            return breadcrumbs;
        }

        public static void BindHeader(Page page, BreadcrumbItem[] breadcrumbs, BreadcrumbItem create = null, string qualifier = null)
        {
            if (page.Master is AdminHome adminHome)
            {
                adminHome.AdminHeader.BindTitle(qualifier);
                adminHome.AdminHeader.BindBreadcrumbs(breadcrumbs, create);
            }
            else if (page.Master is PortalMaster portal)
            {
                portal.Breadcrumbs.BindTitle(qualifier);
                portal.Breadcrumbs.BindBreadcrumbs(breadcrumbs, new[] { create });
            }
        }

        public static void BindHomeHeader(Page page, BreadcrumbItem[] breadcrumbs, BreadcrumbItem[] creates, string qualifier = null)
        {
            if (page.Master is AdminHome a)
            {
                a.AdminHeader.BindTitle(qualifier);
                a.AdminHeader.BindBreadcrumbs(breadcrumbs, creates);
            }
        }

        public static void BindTitle(Page page, string title)
        {
            if (page.Master is AdminHome home)
                home.AdminHeader.BindTitle(title);
            else if (page.Master is PortalMaster form)
                form.Breadcrumbs.BindTitle(title);
        }

        public static void BindSubtitle(Page page, string subtitle)
        {
            if (page.Master is AdminHome home)
                home.AdminHeader.BindSubtitle(subtitle);
        }

        public static void DisplayCalendarLink(Page page)
        {
            if (page.Master is AdminHome a)
                a.AdminHeader.DisplayCalendar();
        }

        public static void HideBreadcrumbs(Page page)
        {
            if (page.Master is AdminHome a)
                a.AdminHeader.HideBreadcrumbs();
            else if (page.Master is PortalMaster d)
                d.HideBreadcrumbsAndTitle();
        }

        public static void HideSideContent(Page page)
        {
            if (page.Master is PortalMaster a)
                a.HideSideContent();
        }

        public static void OverrideTitle(Page page, string title, string subtitle = null)
        {
            if (page.Master is AdminHome home)
                home.AdminHeader.OverrideTitle(title);
            else if (page.Master is PortalMaster portal)
                portal.Breadcrumbs.BindTitleAndSubtitle(title, subtitle);
        }

        public static void HideTitle(Page page)
        {
            if (page.Master is AdminHome home)
                home.AdminHeader.HideTitle();
            else if (page.Master is PortalMaster portal)
                portal.Breadcrumbs.HideTitle();
        }
    }
}