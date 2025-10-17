using System.Collections.Generic;
using System.Threading.Tasks;

using Shift.Common;

namespace Shift.Contract.Presentation
{
    public interface INavigationService
    {
        List<BreadcrumbItem> CollectBreadcrumbs(ActionModel action);
        Task<List<NavigationList>> SearchMenusAsync(IShiftPrincipal principal, bool isCmds);
        Task<List<NavigationItem>> SearchShortcutsAsync(IShiftPrincipal principal);
    }
}
