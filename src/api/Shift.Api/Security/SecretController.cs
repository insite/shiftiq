using Microsoft.AspNetCore.Mvc;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Security API")]
public class SecretController : ControllerBase
{
    private readonly IShiftIdentityService _identityService;

    public SecretController(IShiftIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpGet("security/secrets/introspect")]
    [EndpointName("introspectSecret")]
    [SecretAuthorize()]
    public IActionResult IntrospectAsync()
    {
        var principal = _identityService.GetPrincipal();

        return Ok(principal);
    }
}