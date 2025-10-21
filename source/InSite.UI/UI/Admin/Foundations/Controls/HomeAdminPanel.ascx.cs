using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Persistence;

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
            var isOperator = Identity.IsOperator;
            Func<string, bool> isGrantedByName = name => Identity.IsGranted(name);
            Func<Guid?, bool> isGrantedById = id => Identity.IsGranted(id);
            Func<string, bool> isInGroup = group => Identity.IsInGroup(group);

            Func<string, List<MenuHelper.ActionModel>> searchActions = startsWith =>
            {
                var startsWith1 = startsWith + "/";
                var startsWith2 = "ui/" + startsWith + "/";
                return TActionSearch
                    .Search(x => x.ActionUrl.StartsWith(startsWith1) || x.ActionUrl.StartsWith(startsWith2))
                    .Select(ToModel)
                    .ToList();
            };

            Func<string, MenuHelper.ActionModel> retrieveActionByUrl = url => TActionSearch
                .Search(x => x.ActionUrl == url)
                .Select(ToModel)
                .FirstOrDefault();

            return new MenuHelper(
                isOperator,
                isGrantedByName,
                isGrantedById,
                isInGroup,
                searchActions,
                retrieveActionByUrl
            ).GetNavigationGroups(Navigator?.IsCmds ?? false).ToArray();

            MenuHelper.ActionModel ToModel(TAction action) => new MenuHelper.ActionModel
            {
                ActionList = action.ActionList,
                ActionUrl = action.ActionUrl,
                ActionNameShort = action.ActionNameShort,
                ActionName = action.ActionName,
                ActionIcon = action.ActionIcon,
                PermissionParentActionIdentifier = action.PermissionParentActionIdentifier,
            };
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
            if (item.PermissionActionIdentifier.HasValue && Identity.Claims.IsTrial(item.PermissionActionIdentifier.Value))
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

            var domain = ServiceLocator.AppSettings.Security.Domain;
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
                if (tile.Url.StartsWith("http") || Identity.IsActionAuthorized(tile.Url))
                    list.Add(tile);

            return list.OrderBy(x => x.Title).ToList();
        }
    }
}