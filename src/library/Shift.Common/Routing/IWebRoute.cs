using System;

namespace Shift.Common
{
    public interface IWebRoute
    {
        Guid RouteIdentifier { get; }
        Guid? ParentRouteIdentifier { get; }
        string ToolkitName { get; set; }
        Guid ToolkitNumber { get; set; }
        string HelpUrl { get; }
        string DesktopControl { get; }
        string Icon { get; }
        string LinkTitle { get; }
        string Name { get; }
        string Title { get; }
        string Category { get; }
        bool IsModal { get; set; }
        string DesktopType { get; }
        string CssClass { get; }

        IWebRoute GetParent();
    }
}
