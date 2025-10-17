using Shift.Common;
using Shift.Contract;
using Shift.Contract.Presentation;
using Shift.Service.Security;

namespace Shift.Service.Presentation
{
    public class NavigationService : INavigationService
    {
        private readonly IActionService _actionService;
        private readonly AuthorizerFactory _authorizer;
        private readonly ITranslator _translator;

        private readonly IPageService _pageService;
        private readonly SecuritySettings _securitySettings;

        public NavigationService(IActionService actionService, AuthorizerFactory authorizer, ITranslator translator,
            IPageService pageService, SecuritySettings securitySettings)
        {
            _actionService = actionService;
            _authorizer = authorizer;
            _translator = translator;

            _pageService = pageService;
            _securitySettings = securitySettings;
        }

        public List<BreadcrumbItem> CollectBreadcrumbs(ActionModel action)
        {
            var route = WebRoute.Create(_actionService, action);
            return BreadcrumbsHelper.CollectBreadcrumbs(route, null, _translator, null, null);
        }

        public async Task<List<NavigationList>> SearchMenusAsync(IShiftPrincipal principal, bool isCmds)
        {
            var authorizer = await _authorizer.CreateAsync();

            var isOperator = principal.Authority >= AuthorityAccess.Operator;
            Func<string, bool> isGrantedByName = name => authorizer.IsGranted(name, principal);
            Func<Guid?, bool> isGrantedById = id => id == null || authorizer.IsGranted(id.Value, principal);
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
                .Select(x => new NavigationItem
                {
                    Url = x.PageUrl,
                    Title = x.PageTitle,
                    Icon = x.PageIcon
                })
                .OrderBy(x => x.Title)
                .ToList();

            var authorizer = await _authorizer.CreateAsync();

            var result = new List<NavigationItem>();

            foreach (var tile in tiles)
            {
                if (tile.Url.StartsWith("http") || authorizer.IsActionAuthorized(tile.Url, principal))
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