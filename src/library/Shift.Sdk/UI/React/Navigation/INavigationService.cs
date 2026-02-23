using System.Collections.Generic;
using System.Threading.Tasks;

using Shift.Common;
using Shift.Sdk.UI.Navigation;

namespace Shift.Contract.Presentation
{
    public interface INavigationService
    {
        NavigationHome GetHome(IPrincipal principal);
        List<BreadcrumbItem> CollectBreadcrumbs(ActionModel action, IPrincipal principal);
        List<NavigationList> SearchMenus(IPrincipal principal, bool isCmds);
        List<NavigationList> SearchAdminMenus(IPrincipal principal, bool isCmds);
        Task<List<NavigationItem>> SearchShortcutsAsync(IPrincipal principal);
    }
}
