using System;

namespace Shift.Common
{
    public interface IWebRoute
    {
        Guid RouteIdentifier { get; }
        Guid ToolkitNumber { get; set; }

        Guid? ParentRouteIdentifier { get; }

        string CssClass { get; }
        string DesktopControl { get; }
        string DesktopType { get; }
        string ExtraBreadcrumb { get; }
        string HelpUrl { get; }
        string Icon { get; }
        string LinkTitle { get; }
        string Name { get; }
        string Title { get; }
        string ToolkitName { get; set; }

        bool IsModal { get; set; }

        IWebRoute GetParent();
    }
}
