using Microsoft.AspNetCore.Mvc;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Platform API")]
public class VersionController : ControllerBase
{
    [HttpGet("platform/version")]
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
}