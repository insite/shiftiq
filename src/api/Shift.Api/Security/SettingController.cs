using Microsoft.AspNetCore.Mvc;

using Shift.Common;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Security API: Settings")]
[HybridAuthorize]
public class SettingController : ControllerBase
{
    private readonly SecuritySettings _securitySettings;

    public SettingController(SecuritySettings securitySettings)
    {
        _securitySettings = securitySettings;
    }

    [HttpGet("security/settings/permissions")]
    public IActionResult Permissions()
    {
        var permissions = new
        {
            _securitySettings.Permissions
        };

        return Ok(permissions);
    }

    [HttpGet("security/settings/policies")]
    public IActionResult Policies()
    {
        var policies = _securitySettings.Permissions
            .SelectMany(x => x.Policies)
            .OrderBy(x => x)
            .ToArray();

        return Ok(policies);
    }

    [HttpGet("security/settings/requirements")]
    public IActionResult Requirements()
    {
        var requirements = new RequirementList();

        return Ok(requirements.Items);
    }

    [HttpGet("security/settings/roles")]
    public IActionResult Roles()
    {
        var roles = _securitySettings.Permissions
            .SelectMany(x => x.Roles)
            .OrderBy(x => x)
            .ToArray();

        return Ok(roles);
    }
}