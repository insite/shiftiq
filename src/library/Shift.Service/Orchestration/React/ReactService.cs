using Shift.Common;
using Shift.Constant;
using Shift.Contract;
using Shift.Contract.Presentation;
using Shift.Service.Content;
using Shift.Service.Directory;
using Shift.Service.Security;

using TimeZones = Shift.Common.TimeZones;

namespace Shift.Service.Presentation;

public class ReactService : IReactService
{
    private readonly AppSettings _appSettings;
    private readonly ReactStartupOptions _startupOptions;
    private readonly Platform _platform;
    private readonly PermissionCache _permissions;
    private readonly IActionService _actionService;
    private readonly OrganizationService _organizationService;
    private readonly OrganizationAdapter _organizationAdapter;
    private readonly UserService _userService;
    private readonly PersonService _personService;
    private readonly INavigationService _navigationService;
    private readonly ILabelService _labelService;
    private readonly IPageService _pageService;
    private readonly TInputReader _inputReader;

    public ReactService(
        AppSettings appSettings,
        ReactStartupOptions startupOptions,
        Platform platform,
        PermissionCache permissions,
        IActionService routeService,
        OrganizationService organizationService,
        OrganizationAdapter organizationAdapter,
        UserService userService,
        PersonService personService,
        INavigationService navigationService,
        ILabelService labelService,
        IPageService pageService,
        TInputReader inputReader
        )
    {
        _appSettings = appSettings;
        _startupOptions = startupOptions;
        _platform = platform;
        _permissions = permissions;
        _actionService = routeService;
        _organizationService = organizationService;
        _organizationAdapter = organizationAdapter;
        _userService = userService;
        _personService = personService;
        _navigationService = navigationService;
        _labelService = labelService;
        _pageService = pageService;
        _inputReader = inputReader;
    }

    private static MemoryCache<(Guid OrgId, Guid UserId), (Guid Id, SiteSettings Settings)> SiteSettingsCache = new();

    public async Task<SiteSettings> RetrieveSiteSettingsAsync(IPrincipal principal, bool refresh)
    {
        // Check the in-memory cache before building a new SiteSettings object. If the caller has explicitly requested a
        // cache refresh then skip this intial check.

        var identityKey = GetIdentityKey(principal);
        if (!refresh && SiteSettingsCache.TryGet(identityKey, out var cachedValue) && principal.CookieId == cachedValue.Id)
        {
            return cachedValue.Settings;
        }

        bool isCmds = StringHelper.EqualsAny(principal.Partition.Slug, new[] { "e03", "cmds" });

        var settings = new SiteSettings();

        var navigationMenus = SiteSettings.FromNavigationLists(_navigationService.SearchMenus(principal, isCmds));
        var navigationShortcuts = SiteSettings.FromNavigationItems(await _navigationService.SearchShortcutsAsync(principal));
        var adminNavigationMenus = SiteSettings.FromNavigationLists(_navigationService.SearchAdminMenus(principal, isCmds));

        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(principal.User.TimeZone);

        var organization = await _organizationService.RetrieveAsync(principal.Organization.Identifier);
        var organizationData = organization != null ? _organizationAdapter.ToData(organization) : null;
        var userFirstName = (await _userService.RetrieveAsync(principal.User.Identifier))?.FirstName;

        var home = _navigationService.GetHome(principal);
        settings.Home = new SiteSettings.HomeSettings
        {
            Text = home?.Text ?? "Home",
            Icon = home?.Icon ?? "fa-solid fa-home",
            Image = home?.Image,
            Url = home?.Href ?? "/client/admin/home"
        };

        settings.TimeZoneId = TimeZones.GetAbbreviation(timeZone).Moment;
        settings.OrganizationCode = organizationData?.OrganizationCode;
        settings.CompanyName = organizationData?.CompanyName;
        settings.IsCmds = isCmds;
        settings.UserName = userFirstName;
        settings.IsAdministrator = principal.Authority >= AuthorityAccess.Administrator;
        settings.IsOperator = principal.Authority >= AuthorityAccess.Operator;
        settings.IsMultiOrganization = await IsMultiOrganization(principal.User.Identifier);
        settings.ImpersonatorName = principal.Proxy?.Agent != null ? principal.Proxy.Agent.Name : null;

        if (organizationData != null && organizationData.Toolkits.Portal.ShowMyDashboard)
        {
            settings.MyDashboard = new SiteSettings.LinkModel
            {
                Text = _labelService.GetTranslation(_startupOptions.DashboardHeadingText, principal.User.Language, principal.Organization.Identifier, false, true),
                Url = _startupOptions.DashboardPageUrl
            };
        }

        var permissionList = _permissions.Matrix.GetPermissions(principal.Organization.Slug);

        settings.Permissions = new SiteSettings.PermissionsModel
        {
            Accounts = permissionList.IsAllowed(PermissionNames.Admin_Accounts.ToLower(), principal.Roles),
            Integrations = permissionList.IsAllowed(PermissionNames.Admin_Integrations.ToLower(), principal.Roles),
            Settings = permissionList.IsAllowed(PermissionNames.Admin_Settings.ToLower(), principal.Roles),
        };

        var environment = _appSettings.Release.GetEnvironment();
        settings.Environment = new SiteSettings.EnvironmentModel
        {
            Name = environment.Name.ToString(),
            Version = _appSettings.Release.Version,
            Color = environment.Color
        };

        settings.StylePath = _appSettings.CssFileUrl;
        settings.AdminNavigationLogo = _startupOptions.AdminLogoUrl;
        settings.UserHostAddress = principal.IPAddress;
        settings.SessionTimeoutMinutes = _startupOptions.SessionTimeoutMinutes;
        settings.NavigationGroups = navigationMenus;
        settings.ShortcutGroups = navigationShortcuts;
        settings.AdminNavigationGroups = adminNavigationMenus;

        settings.PlatformSearchDownloadMaximumRows = _platform.Search.Download.MaximumRows;
        settings.PartitionEmail = _appSettings.Partition.Email;
        settings.CurrentLanguage = principal.User.Language;
        settings.SupportedLanguages = organizationData != null ? [.. organizationData.Languages.Select(x => x.TwoLetterISOLanguageName)] : ["en"];

        SiteSettingsCache.Add(identityKey, (principal.CookieId, settings));

        return settings;
    }

    private async Task<bool> IsMultiOrganization(Guid userId)
    {
        var people = await _personService.SearchAsync(new SearchPeople { UserId = userId });
        return people.Count(x => x.IsAdministrator || x.IsLearner) > 1;
    }

    private static (Guid, Guid) GetIdentityKey(IPrincipal principal)
    {
        if (principal != null)
        {
            var userId = principal.UserId;
            var orgId = principal.OrganizationId;
            if (userId != Guid.Empty && orgId != Guid.Empty)
                return (orgId, userId);
        }

        return (Guid.Empty, UserIdentifiers.Someone);
    }

    public async Task<PageSettings> RetrievePageSettingsAsync(IPrincipal principal, string actionUrl)
    {
        var settings = new PageSettings();

        if (!string.IsNullOrEmpty(actionUrl) && actionUrl.StartsWith("/"))
            actionUrl = actionUrl.Substring(1);

        var route = _actionService.Retrieve(actionUrl);
        if (route == null)
        {
            settings.AddError($"Url Not Found: {actionUrl}", 404);
            return settings;
        }

        var permissionList = _permissions.Matrix.GetPermissions(principal.Organization.Slug);

        if (!permissionList.IsAllowed(route.ActionUrl, principal.Roles))
        {
            settings.AddError("Access Denied", 403);
            return settings;
        }

        settings.ActionId = route.ActionId;
        settings.ActionTitle = route.ActionName;
        settings.DisplayCalendar = false;
        settings.FullWidth = true;

        settings.Breadcrumbs = CollectBreadcrumbs(route, principal);

        var (pageId, pageBody) = await RetrieveHelpTopicAsync(principal, actionUrl);
        settings.PageId = pageId;
        settings.CoreAbout = new PageSettings.AboutModel { Body = "Test", Heading = "Test" };
        settings.CustomAbout = new PageSettings.AboutModel { Body = pageBody, Heading = "Test" };

        return settings;
    }

    private List<PageSettings.BreadcrumbModel> CollectBreadcrumbs(ActionModel action, IPrincipal principal)
    {
        var breadcrumbs = _navigationService.CollectBreadcrumbs(action, principal);

        return breadcrumbs.Select(x => new PageSettings.BreadcrumbModel
        {
            Text = x.Text,
            Url = x.Href
        }).ToList();
    }

    private async Task<(Guid? pageId, string? body)> RetrieveHelpTopicAsync(IPrincipal principal, string actionUrl)
    {
        var criteria = new SearchPages()
        {
            OrganizationId = principal.Organization.Identifier,
            ParentPageSlug = "in-help",
            PageSlug = actionUrl.Replace("/", "-"),
        };

        var page = (await _pageService.SearchAsync(criteria)).FirstOrDefault();
        if (page == null)
            return (null, null);

        var contents = await _inputReader.CollectAsync(new CollectInputs
        {
            ContainerId = page.PageId,
            ContentLabel = "Body",
            ContentLanguage = "en"
        });

        var content = contents.FirstOrDefault();

        var body = content?.ContentText != null ? Markdown.ToHtml(content.ContentText) : null;

        return (page.PageId, body);
    }
}