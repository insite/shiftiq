using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using Shift.Common;
using Shift.Contract;

namespace InSite.UI.Layout.Admin
{
    public partial class HomeAdminPanel : AdminBaseControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ToolkitRepeater.DataBinding += ToolkitRepeater_DataBinding;
            ToolkitRepeater.ItemDataBound += ToolkitRepeater_ItemDataBound;

            ShortcutRepeater.DataBinding += ShortcutRepeater_DataBinding;
            ShortcutRepeater.ItemDataBound += ShortcutRepeater_ItemDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ToolkitRepeater.DataBind();
            ShortcutRepeater.DataBind();
        }

        private NavigationList[] GetNavigationGroups()
        {
            return MenuHelperFactory
                .Create()
                .GetNavigationGroups(new NavigationIdentity(Identity, ServiceLocator.Partition.Slug), Navigator?.IsCmds ?? false)
                .ToArray();
        }

        private void ToolkitRepeater_DataBinding(object sender, EventArgs e)
        {
            var lists = GetNavigationGroups()
                .SelectMany(x => x.MenuItems)
                .OrderBy(x => x.Title)
                .ToList();

            ToolkitRepeater.DataSource = lists;
            ToolkitRepeater.Visible = lists.Count > 0;
        }

        private void ToolkitRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var item = (NavigationItem)e.Item.DataItem;

            var iconHtml = $"<i class='{item.Icon} fa-3x mb-3'></i>";
            if (item.PermissionActionUrl != null && Identity.Claims.IsGranted(item.PermissionActionUrl, FeatureAccess.Trial))
                iconHtml = "<span class=\"badge border border-warning text-warning fs-xl bg-white badge-trial\">Trial</span>" + iconHtml;

            var icon = (Label)e.Item.FindControl("CardIcon");
            icon.Text = iconHtml;

            var title = (Literal)e.Item.FindControl("CardTitle");
            title.Text = $"<h3 class='h5 text-nowrap nav-heading mb-2 {icon.CssClass}'>{item.Title}</h3>";
        }

        private void ShortcutRepeater_DataBinding(object sender, EventArgs e)
        {
            var cards = GetShortcutCards();
            ShortcutRepeater.DataSource = cards;
            ShortcutPanel.Visible = cards.Count > 0;
        }

        private void ShortcutRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var card = (NavigationItem)e.Item.DataItem;

            var icon = (Label)e.Item.FindControl("CardIcon");
            icon.Text = $"<i class='{card.Icon} fa-3x mb-3'></i>";

            var title = (Literal)e.Item.FindControl("CardTitle");
            title.Text = $"<h3 class='h5 nav-heading mb-2 {icon.CssClass}'>{card.Title}</h3>";
        }

        internal static IList<NavigationItem> GetShortcutCards()
        {
            // Synchronize code with NavigationService.SearchShortcuts

            var domain = ServiceLocator.AppSettings.Partition.Domain;
            var pages = ServiceLocator.PageSearch.Select(x => x.Site.SiteDomain == Organization.Code + "." + domain && x.Parent.PageSlug == "admin" && x.Parent.PageType == "Folder");
            var tiles = pages
                .Where(x => x.NavigateUrl != null)
                .Select(x => new NavigationItem
                {
                    Url = x.NavigateUrl,
                    Title = x.Title,
                    Icon = x.PageIcon
                })
                .OrderBy(x => x.Title)
                .ThenBy(x => x.Title);

            var list = new List<NavigationItem>();
            foreach (var tile in tiles)
                if (tile.Url.StartsWith("http") || Identity.IsGranted(tile.Url))
                    list.Add(tile);

            return list.OrderBy(x => x.Title).ToList();
        }
    }
}