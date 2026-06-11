using Microsoft.AspNetCore.Mvc;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Diagnostics API")]
public class HealthController : ControllerBase
{
    private readonly ReleaseSettings _releaseSettings;

    public HealthController(ReleaseSettings releaseSettings, SecuritySettings securitySettings,
        IClaimConverter claimConverter, IPrincipalSearch principalSearch)
    {
        _releaseSettings = releaseSettings;
    }

    [HttpGet("api/diagnostics/health")]
    [EndpointName("health")]
    public ActionResult<HealthResponse> Health()
    {
        var environment = _releaseSettings.GetEnvironment();

        var version = _releaseSettings.Version;

        var model = new HealthResponse
        {
            Status = $"Shift API version {version} is online. The {environment} environment says hello.",
            Version = version,
            Environment = environment
        };

        if (environment.IsLocal() && _releaseSettings.ConfigurationProviders?.Count > 0)
            model.Configuration = new HealthConfiguration { Providers = _releaseSettings.ConfigurationProviders };

        return Ok(model);
    }

    [HttpGet("health")]
    [HttpGet("platform/health")]
    [HttpGet("api/platform/health")]
    [HttpGet("status")]
    [HttpGet("platform/status")]
    [HttpGet("api/diagnostics/status")]
    [HttpGet("api/platform/status")]
    [AliasFor("health")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public ActionResult<HealthResponse> HeathAlias()
    {
        return Health();
    }

    [HttpPost("api/diagnostics/health/error")]
    public IActionResult HealthError()
    {
        throw new InvalidOperationException("Health check on monitoring unhandled exceptions");
    }
}
