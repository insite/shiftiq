using System;

using InSite.Persistence;

using Shift.Common;

namespace InSite
{
    [Serializable]
    public class WebRoute : IWebRoute
    {
        public Guid RouteIdentifier { get; private set; }
        public Guid ToolkitNumber { get; set; }

        public Guid? ParentRouteIdentifier { get; private set; }

        public string CssClass { get; private set; }
        public string DesktopControl { get; private set; }
        public string DesktopType { get; private set; }
        public string ExtraBreadcrumb { get; }
        public string HelpUrl { get; private set; }
        public string Icon { get; private set; }
        public string LinkTitle { get; protected set; }
        public string Name { get; protected set; }
        public string Title { get; private set; }
        public string ToolkitName { get; set; }

        public bool IsModal { get; set; }

        protected WebRoute() { }

        public WebRoute(TAction route)
        {
            RouteIdentifier = route.ActionIdentifier;
            ParentRouteIdentifier = route.NavigationParentActionIdentifier;
            IsModal = route.ActionType == "Modal";
            Name = route.ActionUrl;
            Title = route.ActionName;
            ExtraBreadcrumb = route.ExtraBreadcrumb;
            LinkTitle = route.ActionNameShort.IfNullOrEmpty(route.ActionName);
            HelpUrl = route.HelpUrl;
            Icon = route.ActionIcon;
            DesktopControl = route.ControllerPath;
            CssClass = route.ActionType;

            ToolkitName = "None";
            ToolkitNumber = route.PermissionParentActionIdentifier ?? Guid.Empty;
            if (ToolkitNumber != Guid.Empty)
            {
                var permission = TActionSearch.Get(ToolkitNumber);
                if (permission != null)
                    ToolkitName = permission.ActionName;
            }
        }

        public virtual IWebRoute GetParent()
        {
            if (ParentRouteIdentifier.HasValue)
            {
                var route = TActionSearch.Get(ParentRouteIdentifier.Value);
                if (route != null)
                    return new WebRoute(route);
            }

            return null;
        }

        public static WebRoute GetWebRoute(Guid id)
        {
            var entity = TActionSearch.Get(id);
            return entity != null ? new WebRoute(entity) : null;
        }

        public static WebRoute GetWebRoute(string action)
        {
            var entity = TActionSearch.Get(action);
            return entity != null ? new WebRoute(entity) : null;
        }
    }
}

