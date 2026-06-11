using Shift.Common;
using Shift.Contract;
using Shift.Contract.Presentation;
using Shift.Sdk.UI.Navigation;

namespace Shift.Service.Orchestration
{
    public class NavigationService : INavigationService
    {
        private readonly IActionService _actionService;
        private readonly PermissionCache _permissions;
        private readonly ITranslatorService _translator;

        private readonly IPageService _pageService;
        private readonly AppSettings _appSettings;

        private MenuHelper? _menuHelper;

        public NavigationService(
            IActionService actionService,
            PermissionCache permissions,
            ITranslatorService translator,
            IPageService pageService,
            AppSettings appSettings
            )
        {
            _actionService = actionService;
            _permissions = permissions;
            _translator = translator;

            _pageService = pageService;
            _appSettings = appSettings;
        }

        public NavigationHome? GetHome(IPrincipal principal)
        {
            return NavigationRootFactory.Create(principal.Partition.Slug, GetResource)?.Home;
        }

        public List<BreadcrumbItem> CollectBreadcrumbs(ActionModel action, IPrincipal principal)
        {
            var route = WebRoute.Create(_actionService, action);
            return BreadcrumbsHelper.CollectBreadcrumbs(route, null, _translator, principal.User.Language, principal.Organization.Identifier, null, null);
        }

        public List<NavigationList> SearchMenus(IPrincipal principal, bool isCmds)
        {
            return CreateMenuHelper().GetNavigationGroups(new NavigationIdentity(_permissions, principal, isCmds), isCmds);
        }

        public List<NavigationList> SearchAdminMenus(IPrincipal principal, bool isCmds)
        {
            return CreateMenuHelper().GetAdminMenuGroups(new NavigationIdentity(_permissions, principal, isCmds));
        }

        public async Task<List<NavigationItem>> SearchShortcutsAsync(IPrincipal principal)
        {
            if (!principal.IsAuthenticated)
                return new List<NavigationItem>();

            var organizationCode = principal.Organization.Name;

            var domain = _appSettings.Partition.Domain;

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
                if (tile.Url.StartsWith("http") || permissionList.IsAllowedByRoute(GetRouteFromUrl(tile.Url), principal.Roles))
                    result.Add(tile);
            }

            var list = result.OrderBy(x => x.Title).ToList();

            return list;
        }

        private static string GetRouteFromUrl(string url)
        {
            if (url.StartsWith("/"))
                url = url.Substring(1);

            var paramIndex = url.IndexOf("?");
            if (paramIndex >= 0)
                url = url.Substring(0, paramIndex);

            return url;
        }

        private MenuHelper CreateMenuHelper()
        {
            if (_menuHelper != null)
                return _menuHelper;

            _menuHelper = new MenuHelper(SearchActions, RetrieveActionByUrl);

            return _menuHelper;
        }

        private List<MenuHelper.ActionModel> SearchActions(string startsWith)
        {
            var startsWith1 = startsWith + "/";
            var startsWith2 = "ui/" + startsWith + "/";
            return _actionService
                .Search(x => x.ActionUrl.StartsWith(startsWith1) || x.ActionUrl.StartsWith(startsWith2))
                .Select(ToModel)
                .ToList();
        }

        private MenuHelper.ActionModel? RetrieveActionByUrl(string url) => _actionService
            .Search(x => x.ActionUrl == url)
            .Select(ToModel)
            .FirstOrDefault();

        private string? GetResource(string url)
        {
            return RetrieveActionByUrl(url)?.PermissionParentActionUrl;
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
                PermissionParentActionUrl = action.PermissionParentActionUrl
            };
        }
    }
}