using Microsoft.AspNetCore.Mvc;

using Shift.Common;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Platform API: Routes")]
public class EndpointController : ControllerBase
{
    private readonly ShiftSettings _settings;

    public EndpointController(ShiftSettings settings)
    {
        _settings = settings;
    }

    [HttpGet("platform/endpoints")]
    public async Task<IActionResult> SearchAsync(string? path)
    {
        var baseUrl = _settings.Api.Hosting.V2.BaseUrl.TrimEnd('/');

        var openApiUrl = $"{baseUrl}/swagger/v2/swagger.json";

        var service = new OpenApiService();

        var endpoints = await service.GetEndpointsByRoutePathAsync(openApiUrl, path);

        return Ok(endpoints);
    }
}
