using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Constant;
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
            var result = new List<NavigationList>();
            if (!identity.IsGranted(PermissionNames.Admin_Courses))
                return result;

            foreach (var inputGroup in AdminMenu.Groups)
            {
                var outputItems = new List<NavigationItem>();
                foreach (var inputItem in inputGroup.MenuItems)
                {
                    var actionName = inputItem.Url.Substring(1);
                    if (!identity.IsGranted(actionName))
                    {
                        var action = _retrieveActionByUrl(actionName);
                        if (action == null || !identity.IsGranted(action.PermissionParentActionUrl))
                            continue;
                    }
                    outputItems.Add(new NavigationItem { Url = inputItem.Url, Icon = inputItem.Icon, Title = inputItem.Title });
                }

                if (outputItems.Count > 0)
                    result.Add(new NavigationList { Title = inputGroup.Title, MenuItems = outputItems.ToArray() });
            }

            return result;
        }
    }
}
