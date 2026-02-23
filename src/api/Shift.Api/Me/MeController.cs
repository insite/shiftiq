using Microsoft.AspNetCore.Mvc;

using Shift.Contract.Presentation;

namespace Shift.Api;

/// <summary>
/// Controller responsible for providing a unified session context model for the currently authenticated user
/// </summary>
/// <remarks>
/// This endpoint aggregates data from multiple subsystems (such as security, directory, platform, and site layout) to
/// construct a runtime view model tailored for front-end applications (i.e., React). The response includes user 
/// identity, roles, permissions, environment details, navigation metadata, and other session-scoped properties relevant 
/// to UI rendering.
/// </remarks>
[ApiController]
[Route("api/me")]
[ApiExplorerSettings(GroupName = "Me API")]
public class MeController : ControllerBase
{
    private readonly IPrincipalProvider _principalProvider;
    private readonly PermissionCache _permissionCache;
    private readonly IReactService _reactService;

    /// <summary>
    /// Initializes a new instance
    /// </summary>
    /// <param name="principalProvider">Service for retrieving user identity and principal information.</param>
    /// <param name="permissionCache">Cached permissions for all organization accounts.</param>
    /// <param name="reactService">Service for building React-specific site settings and configuration.</param>
    /// <exception cref="ArgumentNullException">Thrown when any required service is null.</exception>
    public MeController(IPrincipalProvider principalProvider, PermissionCache permissionCache, IReactService reactService)
    {
        _principalProvider = principalProvider ?? throw new ArgumentNullException(nameof(principalProvider));
        _permissionCache = permissionCache;
        _reactService = reactService ?? throw new ArgumentNullException(nameof(reactService));
    }

    /// <summary>
    /// Retrieves the unified session context for the currently authenticated user
    /// </summary>
    /// <remarks>
    /// This endpoint aggregates user identity, permissions, environment settings, and UI configuration
    /// into a single response optimized for front-end application initialization.
    /// 
    /// The response is cached (in memory) per session unless the refresh parameter is set to true. Only authenticated 
    /// users will receive a successful response.
    /// </remarks>
    /// <param name="refresh">
    /// When true, bypasses cached data and rebuilds the context from source systems.
    /// Defaults to false for optimal performance.
    /// </param>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>
    /// A <see cref="SiteSettings"/> object containing the user's session context,
    /// or an Unauthorized response if the user is not authenticated.
    /// </returns>
    /// <response code="200">Successfully retrieved user context. Returns SiteSettings object.</response>
    [HttpGet("context")]
    [ProducesResponseType(typeof(SiteSettings), StatusCodes.Status200OK)]
    public async Task<IActionResult> RetrieveContextAsync(
        [FromQuery] bool refresh = false,
        CancellationToken cancellationToken = default)
    {
        var principal = _principalProvider.GetPrincipal();

        var settings = await _reactService.RetrieveSiteSettingsAsync(principal, refresh);

        return Ok(settings);
    }

    /// <summary>
    /// Retrieves the list of permissions granted (or denied) to me
    /// </summary>
    [HttpGet("permissions")]
    [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
    public IActionResult RetrievePermissions()
    {
        var me = _principalProvider.GetPrincipal();

        var myRoles = me.Roles.Select(x => x.Name).OrderBy(x => x).ToList();

        var allResources = _permissionCache.Matrix.Resources.GetResourceNames();

        var allPermissions = _permissionCache.Matrix.GetPermissions(me.Organization.Slug);

        var myPermissions = new PermissionResult();

        foreach (var resource in allResources)
        {
            foreach (var role in myRoles)
            {
                var permissions = allPermissions.Items
                    .Where(x => x.Resource.Path == resource && x.Role.Name == role)
                    .ToList();

                foreach (var permission in permissions)
                {
                    var granted = permission.Access.Granted;

                    if (granted.HasAny())
                        myPermissions.Granted.Add(new PermissionResultItem(resource, role, granted.ToString()));

                    var denied = permission.Access.Denied;

                    if (denied.HasAny())
                        myPermissions.Denied.Add(new PermissionResultItem(resource, role, denied.ToString()));
                }
            }
        }

        return Ok(myPermissions);
    }

    public class PermissionResult
    {
        public List<PermissionResultItem> Granted { get; set; } = new List<PermissionResultItem>();
        public List<PermissionResultItem> Denied { get; set; } = new List<PermissionResultItem>();
    }

    public class PermissionResultItem
    {
        public PermissionResultItem(string resource, string role, string access)
        {
            Resource = resource;
            Role = role;
            Access = access;
        }

        public string Resource { get; set; }
        public string Role { get; set; }
        public string Access { get; set; }
    }

    /// <summary>
    /// Retrieves the list of roles to which I am assigned
    /// </summary>
    [HttpGet("roles")]
    [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
    public IActionResult RetrieveRoles()
    {
        var principal = _principalProvider.GetPrincipal();

        var roles = principal.Roles.Select(x => x.Name).OrderBy(x => x).ToList();

        return Ok(roles);
    }
}