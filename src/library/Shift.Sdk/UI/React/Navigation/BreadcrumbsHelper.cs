using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public static class BreadcrumbsHelper
    {
        public static List<BreadcrumbItem> CollectBreadcrumbs(
            IWebRoute pageRoute,
            string linkTitle,
            ITranslator translator,
            IOverrideWebRouteParent overrideWebRouteParent,
            IHasParentLinkParameters hasParentLinkParameters
            )
        {
            var breadcrumbs = new List<BreadcrumbItem>();

            AddBreadcrumbParentItems(pageRoute, breadcrumbs, translator, overrideWebRouteParent, hasParentLinkParameters);

            if (!string.IsNullOrWhiteSpace(pageRoute.ExtraBreadcrumb))
            {
                var extra = translator != null ? translator.Translate(pageRoute.ExtraBreadcrumb) : pageRoute.ExtraBreadcrumb;
                breadcrumbs.Add(new BreadcrumbItem(extra, null));
            }

            if (string.IsNullOrEmpty(linkTitle))
                linkTitle = translator != null ? translator.Translate(pageRoute.LinkTitle) : pageRoute.LinkTitle;

            breadcrumbs.Add(new BreadcrumbItem(linkTitle, null, null, "active"));

            return breadcrumbs;
        }

        public static void AddBreadcrumbParentItems(
            IWebRoute route,
            List<BreadcrumbItem> breadcrumbs,
            ITranslator translator,
            IOverrideWebRouteParent overrideWebRouteParent,
            IHasParentLinkParameters hasParentLinkParameters
            )
        {
            var parentRoute = overrideWebRouteParent?.GetParent() ?? route.GetParent();
            var parentParams = hasParentLinkParameters?.GetParentLinkParameters(parentRoute);

            var links = new Stack<BreadcrumbItem>();

            while (parentRoute != null)
                parentRoute = AddParentItem(parentParams, links, parentRoute, translator);

            while (links.Count > 0)
                breadcrumbs.Add(links.Pop());
        }

        private static IWebRoute AddParentItem(string parentParams, Stack<BreadcrumbItem> links, IWebRoute parentRoute, ITranslator translator)
        {
            var href = "/" + parentRoute.Name;
            if (!string.IsNullOrEmpty(parentParams))
                href += "?" + parentParams;

            var linkTitle = translator != null ? translator.Translate(parentRoute.LinkTitle) : parentRoute.LinkTitle;

            links.Push(new BreadcrumbItem(linkTitle, href));

            if (!string.IsNullOrWhiteSpace(parentRoute.ExtraBreadcrumb))
            {
                var extra = translator != null ? translator.Translate(parentRoute.ExtraBreadcrumb) : parentRoute.ExtraBreadcrumb;
                links.Push(new BreadcrumbItem(extra, null));
            }

            return parentRoute.GetParent();
        }
    }
}