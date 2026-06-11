using Microsoft.AspNetCore.Mvc;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Diagnostics API")]
public class VersionController : ControllerBase
{
    [HttpGet("api/diagnostics/version")]
    [EndpointName("version")]
    public ActionResult<VersionResponse> Version()
    {
        var version = typeof(VersionController).Assembly.GetName().Version
            ?? new Version(0, 0, 0, 0);

        var body = new VersionResponse
        {
            Version = version.ToString(),
            Major = version.Major,
            Minor = version.Minor,
            Build = version.Build,
            Revision = version.Revision
        };

        return Ok(body);
    }

    [HttpGet("version")]
    [HttpGet("platform/version")]
    [HttpGet("api/platform/version")]
    [AliasFor("version")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public ActionResult<VersionResponse> VersionAlias()
    {
        return Version();
    }
}
