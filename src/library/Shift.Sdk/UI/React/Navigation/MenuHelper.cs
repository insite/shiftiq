using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Sdk.UI.Navigation;

namespace Shift.Contract
{
    public class MenuHelper
    {
        public class ActionModel
        {
            public string ActionList { get; set; }
            public string ActionUrl { get; set; }
            public string ActionNameShort { get; set; }
            public string ActionName { get; set; }
            public string ActionIcon { get; set; }
            public string PermissionParentActionUrl { get; set; }
        }

        private readonly Func<string, List<ActionModel>> _searchActions;
        private readonly Func<string, ActionModel> _retrieveActionByUrl;

        public MenuHelper(
            Func<string, List<ActionModel>> searchActions,
            Func<string, ActionModel> retrieveActionByUrl
            )
        {
            _searchActions = searchActions;
            _retrieveActionByUrl = retrieveActionByUrl;
        }

        public List<NavigationList> GetNavigationGroups(INavigationIdentity identity, bool isCmds)
        {
            var root = NavigationRootFactory.Create(identity.PartitionSlug, GetResource);
            var categories = root.GetAccessibleCategories(identity);

            var result = new List<NavigationList>();

            foreach (var category in categories)
            {
                var items = category.Links.Select(x => new NavigationItem
                {
                    Url = x.Href,
                    Icon = x.Icon,
                    Title = x.Text
                }).ToArray();

                result.Add(new NavigationList
                {
                    Title = category.Category,
                    MenuItems = items
                });
            }

            return result;
        }

        private string GetResource(string url)
        {
            return !string.IsNullOrEmpty(url) ? _retrieveActionByUrl(url.Substring(1))?.PermissionParentActionUrl : null;
        }

        public List<NavigationList> GetAdminMenuGroups(INavigationIdentity identity)
        {
            var menu = new List<NavigationList>();

            foreach (var adminGroups in AdminMenu.Groups)
            {
                var items = new List<NavigationItem>();

                foreach (var adminGroupMenuItem in adminGroups.MenuItems)
                {
                    var adminActionUrl = adminGroupMenuItem.Url.Substring(1);

                    if (!identity.IsGranted(adminActionUrl))
                    {
                        var action = _retrieveActionByUrl(adminActionUrl);

                        if (action == null)
                            continue;

                        var resource = action.PermissionParentActionUrl;

                        if (resource == null || !identity.IsGranted(resource))
                            continue;
                    }

                    items.Add(new NavigationItem { Url = adminGroupMenuItem.Url, Icon = adminGroupMenuItem.Icon, Title = adminGroupMenuItem.Title });
                }

                if (items.Count > 0)
                    menu.Add(new NavigationList { Title = adminGroups.Title, MenuItems = items.ToArray() });
            }

            return menu;
        }
    }
}
