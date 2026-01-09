using Microsoft.AspNetCore.Mvc;

using Shift.Common;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Platform API")]
public class HealthController : ControllerBase
{
    private readonly ReleaseSettings _releaseSettings;

    public HealthController(ReleaseSettings releaseSettings, SecuritySettings securitySettings,
        IClaimConverter claimConverter, IPrincipalSearch principalSearch)
    {
        _releaseSettings = releaseSettings;
    }

    [HttpGet("platform/health")]
    [HttpGet("platform/status")]
    public IActionResult Health()
    {
        var environment = _releaseSettings.GetEnvironment();

        var version = _releaseSettings.Version;

        var model = new Dictionary<string, object>
        {
            ["Status"] = $"Shift API (v2) version {version} is online. The {environment} environment says hello.",
            ["Version"] = version,
            ["Environment"] = environment
        };

        if (environment.IsLocal() && _releaseSettings.ConfigurationProviders?.Any() == true)
            model["Configuration"] = new { Providers = _releaseSettings.ConfigurationProviders };

        return Ok(model);
    }

    [HttpPost("platform/health/error")]
    public IActionResult HealthError()
    {
        throw new Exception("Health check on monitoring unhandled exceptions");
    }
}