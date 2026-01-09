using Shift.Common;
using Shift.Contract;
using Shift.Contract.Presentation;

namespace Shift.Service.Presentation
{
    public class NavigationService : INavigationService
    {
        private readonly IActionService _actionService;
        private readonly PermissionMatrixProvider _permissions;
        private readonly ITranslator _translator;

        private readonly IPageService _pageService;
        private readonly SecuritySettings _securitySettings;

        public NavigationService(IActionService actionService, PermissionMatrixProvider permissions, ITranslator translator,
            IPageService pageService, SecuritySettings securitySettings)
        {
            _actionService = actionService;
            _permissions = permissions;
            _translator = translator;

            _pageService = pageService;
            _securitySettings = securitySettings;
        }

        public List<BreadcrumbItem> CollectBreadcrumbs(ActionModel action)
        {
            var route = WebRoute.Create(_actionService, action);
            return BreadcrumbsHelper.CollectBreadcrumbs(route, null, _translator, null, null);
        }

        public List<NavigationList> SearchMenus(IShiftPrincipal principal, bool isCmds)
        {
            var permissions = isCmds
                ? _permissions.Matrix.GetPermissions("cmds")
                : _permissions.Matrix.GetPermissions(principal.Organization.Slug)
                ;

            var isOperator = principal.Authority >= AuthorityAccess.Operator;
            Func<string, bool> isGrantedByName = name => permissions.IsAllowed(name, principal.Roles);
            Func<Guid?, bool> isGrantedById = id => id == null || permissions.IsAllowed(id.Value, principal.Roles);
            Func<string, bool> isInGroup = group => principal.Roles.Any(x => string.Equals(x.Name, group, StringComparison.OrdinalIgnoreCase));

            Func<string, List<MenuHelper.ActionModel>> searchActions = startsWith =>
            {
                var startsWith1 = startsWith + "/";
                var startsWith2 = "ui/" + startsWith + "/";
                return _actionService
                    .Search(x => x.ActionUrl.StartsWith(startsWith1) || x.ActionUrl.StartsWith(startsWith2))
                    .Select(ToModel)
                    .ToList();
            };

            Func<string, MenuHelper.ActionModel?> retrieveActionByUrl = url => _actionService
                .Search(x => x.ActionUrl == url)
                .Select(ToModel)
                .FirstOrDefault();

            return new MenuHelper(
                isOperator,
                isGrantedByName,
                isGrantedById,
                isInGroup,
                searchActions,
                retrieveActionByUrl
            ).GetNavigationGroups(isCmds);
        }

        public async Task<List<NavigationItem>> SearchShortcutsAsync(IShiftPrincipal principal)
        {
            if (!principal.IsAuthenticated)
                return new List<NavigationItem>();

            var organizationCode = principal.Organization.Name;

            var domain = _securitySettings.Domain;

            var criteria = new SearchPages()
            {
                IsNullNavigateUrl = false,
                ParentPageSlug = "admin",
                ParentPageType = "Folder",
                SiteDomain = organizationCode + "." + domain
            };

            var pages = await _pageService.SearchAsync(criteria);

            var tiles = pages
                .Where(x => x.PageUrl != null)
                .Select(x => new NavigationItem
                {
                    Url = x.PageUrl,
                    Title = x.PageTitle,
                    Icon = x.PageIcon
                })
                .OrderBy(x => x.Title)
                .ToList();

            var permissionList = _permissions.Matrix.GetPermissions(principal.Organization.Slug);

            var result = new List<NavigationItem>();

            foreach (var tile in tiles)
            {
                if (tile.Url.StartsWith("http") || permissionList.IsAllowed(tile.Url, principal.Roles))
                    result.Add(tile);
            }

            var list = result.OrderBy(x => x.Title).ToList();

            return list;
        }

        private static MenuHelper.ActionModel ToModel(ActionModel action)
        {
            return new MenuHelper.ActionModel
            {
                ActionList = action.ActionList,
                ActionUrl = action.ActionUrl,
                ActionNameShort = action.ActionNameShort,
                ActionName = action.ActionName,
                ActionIcon = action.ActionIcon,
                PermissionParentActionIdentifier = action.PermissionParentActionIdentifier,
            };
        }
    }
}