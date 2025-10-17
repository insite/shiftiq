using Shift.Common;
using Shift.Constant;
using Shift.Contract;
using Shift.Contract.Presentation;
using Shift.Service.Content;
using Shift.Service.Security;

using TimeZones = Shift.Common.TimeZones;

namespace Shift.Service.Presentation;

public class ReactService : IReactService
{
    private readonly ReleaseSettings _releaseSettings;
    private readonly ReactStartupOptions _startupOptions;
    private readonly Platform _platform;
    private readonly AuthorizerFactory _authorizer;
    private readonly IActionService _actionService;
    private readonly OrganizationService _organizationService;
    private readonly QOrganizationAdapter _organizationAdapter;
    private readonly UserService _userService;
    private readonly INavigationService _navigationService;
    private readonly ILabelService _labelService;
    private readonly IPageService _pageService;
    private readonly TInputReader _inputReader;
    private readonly PartitionFieldService _partitionService;

    public ReactService(
        ReleaseSettings releaseSettings,
        ReactStartupOptions startupOptions,
        Platform platform,
        AuthorizerFactory authorizer,
        IActionService routeService,
        OrganizationService organizationService,
        QOrganizationAdapter organizationAdapter,
        UserService userService,
        INavigationService navigationService,
        ILabelService labelService,
        IPageService pageService,
        TInputReader inputReader,
        PartitionFieldService partitionService
        )
    {
        _releaseSettings = releaseSettings;
        _startupOptions = startupOptions;
        _platform = platform;
        _authorizer = authorizer;
        _actionService = routeService;
        _organizationService = organizationService;
        _organizationAdapter = organizationAdapter;
        _userService = userService;
        _navigationService = navigationService;
        _labelService = labelService;
        _pageService = pageService;
        _inputReader = inputReader;
        _partitionService = partitionService;
    }

    private static MemoryCache<string, SiteSettings> SiteSettingsCache = new MemoryCache<string, SiteSettings>();

    public async Task<SiteSettings> RetrieveSiteSettingsAsync(IShiftPrincipal principal, bool refresh)
    {
        // Check the in-memory cache before building a new SiteSettings object. If the caller has explicitly requested a
        // cache refresh then skip this intial check.

        var identityName = principal.Name ?? "Someone";

        if (!refresh && SiteSettingsCache.TryGet(identityName, out SiteSettings cachedSettings))
        {
            return cachedSettings;
        }

        bool isCmds = StringHelper.Equals(principal.Partition?.Slug, "e03");

        const string CmdsLogo = "/library/images/logos/cmds-dark.png";
        const string ShiftLogo = "/library/images/logos/shift-dark.png";

        var settings = new SiteSettings();

        var navigationMenus = SiteSettings.FromNavigationLists(await _navigationService.SearchMenusAsync(principal, isCmds));
        var navigationShortcuts = SiteSettings.FromNavigationItems(await _navigationService.SearchShortcutsAsync(principal));

        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(principal.User.TimeZone);

        var organization = await _organizationService.RetrieveAsync(principal.Organization.Identifier);
        var organizationData = organization != null ? _organizationAdapter.ToData(organization) : null;
        var userFullName = (await _userService.RetrieveAsync(principal.User.Identifier))?.FullName;

        settings.TimeZoneId = TimeZones.GetAbbreviation(timeZone).Moment;
        settings.OrganizationCode = organizationData?.OrganizationCode;
        settings.CompanyName = organizationData?.CompanyName;
        settings.IsCmds = isCmds;
        settings.CmdsHomeLink = Urls.CmdsHomeUrl;
        settings.UserName = userFullName;
        settings.IsAdministrator = principal.Authority >= AuthorityAccess.Administrator;
        settings.IsOperator = principal.Authority >= AuthorityAccess.Operator;
        settings.IsMultiOrganization = false;
        settings.ImpersonatorName = principal.Proxy?.Agent != null ? principal.Proxy.Agent.Name : null;

        if (organizationData != null && organizationData.Toolkits.Portal.ShowMyDashboard)
        {
            settings.MyDashboard = new SiteSettings.LinkModel
            {
                Text = _labelService.GetTranslation(_startupOptions.DashboardHeadingText, principal.User.Language, principal.Organization.Identifier, false, true),
                Url = _startupOptions.DashboardPageUrl
            };
        }

        var authorizer = await _authorizer.CreateAsync();

        settings.Permissions = new SiteSettings.PermissionsModel
        {
            Accounts = authorizer.IsGranted(PermissionIdentifiers.Admin_Accounts, principal),
            Integrations = authorizer.IsGranted(PermissionIdentifiers.Admin_Integrations, principal),
            Settings = authorizer.IsGranted(PermissionIdentifiers.Admin_Settings, principal)
        };

        var environment = _releaseSettings.GetEnvironment();
        settings.Environment = new SiteSettings.EnvironmentModel
        {
            Name = environment.Name.ToString(),
            Version = _releaseSettings.Version
        };

        settings.PlatformLogoSrc = isCmds ? CmdsLogo : ShiftLogo;
        settings.AdminNavigationLogo = _startupOptions.AdminLogoUrl;
        settings.UserHostAddress = principal.IPAddress;
        settings.SessionTimeoutMinutes = _startupOptions.SessionTimeoutMinutes;
        settings.NavigationGroups = navigationMenus;
        settings.ShortcutGroups = navigationShortcuts;

        settings.PlatformSearchDownloadMaximumRows = _platform.Search.Download.MaximumRows;
        settings.PartitionEmail = (await _partitionService.RetrieveModelAsync()).Email;
        settings.SupportedLanguages = organizationData != null ? [.. organizationData.Languages.Select(x => x.TwoLetterISOLanguageName)] : ["en"];

        SiteSettingsCache.Add(identityName, settings);

        return settings;
    }

    public async Task<PageSettings> RetrievePageSettingsAsync(IShiftPrincipal principal, string actionUrl)
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

        var authorizer = await _authorizer.CreateAsync();

        if (!authorizer.IsGranted(route.ActionIdentifier, principal))
        {
            settings.AddError("Access Denied", 403);
            return settings;
        }

        settings.ActionId = route.ActionIdentifier;
        settings.ActionTitle = route.ActionName;
        settings.DisplayCalendar = false;
        settings.FullWidth = true;

        settings.Breadcrumbs = CollectBreadcrumbs(route);

        var (pageId, pageBody) = await RetrieveHelpTopicAsync(principal, actionUrl);
        settings.PageId = pageId;
        settings.CoreAbout = new PageSettings.AboutModel { Body = "Test", Heading = "Test" };
        settings.CustomAbout = new PageSettings.AboutModel { Body = pageBody, Heading = "Test" };

        return settings;
    }

    private List<PageSettings.BreadcrumbModel> CollectBreadcrumbs(ActionModel action)
    {
        var breadcrumbs = _navigationService.CollectBreadcrumbs(action);

        return breadcrumbs.Select(x => new PageSettings.BreadcrumbModel
        {
            Text = x.Text,
            Url = x.Href
        }).ToList();
    }

    private async Task<(Guid? pageId, string? body)> RetrieveHelpTopicAsync(IShiftPrincipal principal, string actionUrl)
    {
        var topic = new HelpTopic();

        var criteria = new SearchPages()
        {
            OrganizationIdentifier = principal.Organization.Identifier,
            ParentPageSlug = "in-help",
            PageSlug = actionUrl.Replace("/", "-"),
        };

        var page = (await _pageService.SearchAsync(criteria)).FirstOrDefault();
        if (page == null)
            return (null, null);

        var contents = await _inputReader.CollectAsync(new CollectInputs
        {
            ContainerIdentifier = page.PageIdentifier,
            ContentLabel = "Body",
            ContentLanguage = "en"
        });

        var content = contents.FirstOrDefault();

        var body = content?.ContentText != null ? Markdown.ToHtml(content.ContentText) : null;

        return (page.PageIdentifier, body);
    }
}