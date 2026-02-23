using Microsoft.AspNetCore.Mvc;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Diagnostic API")]
public class VersionController : ControllerBase
{
    [HttpGet("api/diagnostic/version")]
    [EndpointName("version")]
    public IActionResult Version()
    {
        var version = typeof(VersionController).Assembly.GetName().Version
            ?? new Version(0, 0, 0, 0);

        var body = new
        {
            Version = version,
            version.Major,
            version.Minor,
            version.Build,
            version.Revision
        };

        return Ok(body);
    }

    [HttpGet("version")]
    [HttpGet("platform/version")]
    [HttpGet("api/platform/version")]
    [AliasFor("version")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult VersionAlias()
    {
        return Version();
    }
}