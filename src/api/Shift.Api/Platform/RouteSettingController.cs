using Microsoft.AspNetCore.Mvc;

using Shift.Contract.Presentation;

namespace Shift.Api;

/// <summary>
/// Controller responsible for retrieving configuration settings associated with application routes (i.e., actions).
/// </summary>
/// <remarks>
/// This endpoint allows the frontend to query route-specific metadata, such as layout options, feature flags, display 
/// settings, and access rules, by providing a logical route path (e.g., "/learning/modules").
/// 
/// Route: GET /platform/routes/settings?path={route}
/// 
/// The response is not user-specific and typically reflects partition-wide or platform-wide route behavior. These 
/// settings are used by the UI to dynamically configure page presentation and behavior at runtime.
///
/// Ownership: Platform Component
/// Consumed by: React UI during navigation and page layout initialization
/// </remarks>
[ApiController]
[ApiExplorerSettings(GroupName = "Platform API: Routes")]
public class RouteSettingController : ControllerBase
{
    private readonly IShiftIdentityService _identityService;
    private readonly IReactService _reactService;

    public RouteSettingController(IShiftIdentityService identity, IReactService react)
    {
        _identityService = identity;
        _reactService = react;
    }

    [HttpGet("platform/routes/settings")]
    [ProducesResponseType(typeof(PageSettings), StatusCodes.Status200OK)]
    public async Task<ActionResult<PageSettings>> RetrieveRouteSettingsAsync([FromQuery] string url, CancellationToken token)
    {
        var principal = _identityService.GetPrincipal();

        var settings = await _reactService.RetrievePageSettingsAsync(principal, url);

        return Ok(settings);
    }
}