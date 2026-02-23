using Microsoft.AspNetCore.Mvc;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Diagnostic API")]
public class HealthController : ControllerBase
{
    private readonly ReleaseSettings _releaseSettings;

    public HealthController(ReleaseSettings releaseSettings, SecuritySettings securitySettings,
        IClaimConverter claimConverter, IPrincipalSearch principalSearch)
    {
        _releaseSettings = releaseSettings;
    }

    [HttpGet("api/diagnostic/health")]
    [EndpointName("health")]
    public IActionResult Health()
    {
        var environment = _releaseSettings.GetEnvironment();

        var version = _releaseSettings.Version;

        var model = new Dictionary<string, object>
        {
            ["Status"] = $"Shift API version {version} is online. The {environment} environment says hello.",
            ["Version"] = version,
            ["Environment"] = environment
        };

        if (environment.IsLocal() && _releaseSettings.ConfigurationProviders?.Count > 0)
            model["Configuration"] = new { Providers = _releaseSettings.ConfigurationProviders };

        return Ok(model);
    }

    [HttpGet("health")]
    [HttpGet("platform/health")]
    [HttpGet("api/platform/health")]
    [HttpGet("status")]
    [HttpGet("platform/status")]
    [HttpGet("api/diagnostic/status")]
    [HttpGet("api/platform/status")]
    [AliasFor("health")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult HeathAlias()
    {
        return Health();
    }

    [HttpPost("api/diagnostic/health/error")]
    public IActionResult HealthError()
    {
        throw new InvalidOperationException("Health check on monitoring unhandled exceptions");
    }
}