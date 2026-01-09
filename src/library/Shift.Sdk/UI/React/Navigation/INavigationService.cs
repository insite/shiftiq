using System.Collections.Generic;
using System.Threading.Tasks;

using Shift.Common;

namespace Shift.Contract.Presentation
{
    public interface INavigationService
    {
        List<BreadcrumbItem> CollectBreadcrumbs(ActionModel action);
        List<NavigationList> SearchMenus(IShiftPrincipal principal, bool isCmds);
        Task<List<NavigationItem>> SearchShortcutsAsync(IShiftPrincipal principal);
    }
}
