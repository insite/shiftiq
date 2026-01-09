using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Presentation
{
    internal class WebRoute : IWebRoute
    {
        private readonly IActionService _actionService;

        public Guid RouteIdentifier { get; private set; }
        public Guid ToolkitNumber { get; set; }

        public Guid? ParentRouteIdentifier { get; private set; }

        public string CssClass { get; private set; }
        public string DesktopControl { get; private set; }
        public string? DesktopType { get; private set; }
        public string? ExtraBreadcrumb { get; private set; }
        public string HelpUrl { get; private set; }
        public string Icon { get; private set; }
        public string LinkTitle { get; protected set; }
        public string Name { get; protected set; }
        public string Title { get; private set; }
        public string? ToolkitName { get; set; }

        public bool IsModal { get; set; }

        public static WebRoute Create(IActionService actionService, ActionModel action)
        {
            var permissionParentId = action.PermissionParentActionIdentifier ?? Guid.Empty;
            var permissionParent = permissionParentId != Guid.Empty ? actionService.Retrieve(permissionParentId) : null;

            return new WebRoute(actionService, action, permissionParent);
        }

        private WebRoute(IActionService actionService, ActionModel action, ActionModel? permissionParent)
        {
            _actionService = actionService;

            RouteIdentifier = action.ActionIdentifier;
            ParentRouteIdentifier = action.NavigationParentActionIdentifier;
            IsModal = action.ActionType == "Modal";
            Name = action.ActionUrl;
            Title = action.ActionName;
            LinkTitle = action.ActionNameShort.IfNullOrEmpty(action.ActionName);
            HelpUrl = action.HelpUrl;
            Icon = action.ActionIcon;
            DesktopControl = action.ControllerPath;
            CssClass = action.ActionType;

            ToolkitName = "None";
            ToolkitNumber = permissionParent?.ActionIdentifier ?? Guid.Empty;
            ToolkitName = permissionParent?.ActionName;
        }

        public IWebRoute? GetParent()
        {
            if (ParentRouteIdentifier.HasValue)
            {
                var route = _actionService.Retrieve(ParentRouteIdentifier.Value);
                if (route != null)
                    return Create(_actionService, route);
            }

            return null;
        }
    }
}
