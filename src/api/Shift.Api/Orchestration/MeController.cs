using Microsoft.AspNetCore.Mvc;

using Shift.Common;
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
[Route("me")]
[ApiExplorerSettings(GroupName = "Orchestration API")]
public class MeController : ControllerBase
{
    private readonly IShiftIdentityService _identityService;

    private readonly IReactService _reactService;

    /// <summary>
    /// Initializes a new instance
    /// </summary>
    /// <param name="identityService">Service for retrieving user identity and principal information.</param>
    /// <param name="reactService">Service for building React-specific site settings and configuration.</param>
    /// <exception cref="ArgumentNullException">Thrown when any required service is null.</exception>
    public MeController(IShiftIdentityService identityService, IReactService reactService)
    {
        _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));

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
    /// <response code="401">User is not authenticated or session has expired.</response>
    [HttpGet("context")]
    [ProducesResponseType(typeof(SiteSettings), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Problem), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RetrieveContextAsync(
        [FromQuery] bool refresh = false,
        CancellationToken cancellationToken = default)
    {
        var principal = _identityService.GetPrincipal();

        var settings = await _reactService.RetrieveSiteSettingsAsync(principal, refresh);

        return Ok(settings);
    }
}