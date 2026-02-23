using Microsoft.AspNetCore.Mvc;

using Shift.Contract.Presentation;

namespace Shift.Api;

/// <summary>
/// Controller responsible for retrieving configuration settings associated with application routes (i.e., actions).
/// </summary>
/// <remarks>
/// This endpoint allows the frontend to query route-specific metadata, such as layout options, feature flags, display 
/// settings, and access rules, by providing a logical route path (e.g., "api/learning/modules").
/// 
/// Route: GET /api/setup/routes/settings?path={route}
/// 
/// The response is not user-specific and typically reflects partition-wide or platform-wide route behavior. These 
/// settings are used by the UI to dynamically configure page presentation and behavior at runtime.
///
/// Ownership: Setup subsystem
/// Consumed by: React UI during navigation and page layout initialization
/// </remarks>
[ApiController]
[ApiExplorerSettings(GroupName = "Setup API: Routes")]
public class RouteSettingController : ControllerBase
{
    private readonly IPrincipalProvider _identityService;
    private readonly IReactService _reactService;

    public RouteSettingController(IPrincipalProvider identity, IReactService react)
    {
        _identityService = identity;
        _reactService = react;
    }

    [HttpGet("api/setup/routes/settings")]
    [ProducesResponseType(typeof(PageSettings), StatusCodes.Status200OK)]
    public async Task<ActionResult<PageSettings>> RetrieveRouteSettingsAsync([FromQuery] string url, CancellationToken token)
    {
        var principal = _identityService.GetPrincipal();

        var settings = await _reactService.RetrievePageSettingsAsync(principal, url);

        return Ok(settings);
    }
}