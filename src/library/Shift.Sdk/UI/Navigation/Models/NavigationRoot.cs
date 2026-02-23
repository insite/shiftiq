using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Domain.Foundations;

namespace Shift.Sdk.UI.Navigation
{
    public class NavigationRoot
    {
        public class CategoryItem
        {
            public string Category { get; set; }
            public List<NavigationLink> Links { get; set; }
        }

        public NavigationHome Home { get; set; }
        public List<NavigationCategory> Menu { get; set; }

        public List<CategoryItem> GetAccessibleCategories(INavigationIdentity identity)
        {
            if (Menu == null)
                return null;

            return Menu
                .Where(c => IsCategoryAccessible(identity, c))
                .Select(c => new CategoryItem
                {
                    Category = c.Category,
                    Links = GetAccessibleLinks(identity, c.Links)
                })
                .Where(x => x.Links.Count > 0)
                .ToList();
        }

        private static List<NavigationLink> GetAccessibleLinks(INavigationIdentity identity, List<NavigationLink> links)
        {
            if (links == null)
                return null;

            return links
                .Where(x => IsLinkAccessible(identity, x))
                .Select(x => x.Clone(GetAccessibleLinks(identity, x.Links)))
                .ToList();
        }

        private static bool IsCategoryAccessible(INavigationIdentity identity, NavigationCategory category)
        {
            return category.Security == null || category.Security.Evaluate(identity);
        }

        private static bool IsLinkAccessible(INavigationIdentity identity, NavigationLink link)
        {
            return (link.Resource == null || identity.IsGranted(link.Resource))
                && (link.Security == null || link.Security.Evaluate(identity));
        }

        public NavigationRoot Clone()
        {
            return new NavigationRoot
            {
                Home = this.Home?.Clone(),
                Menu = this.Menu?.Select(x => x.Clone()).ToList()
            };
        }
    }
}