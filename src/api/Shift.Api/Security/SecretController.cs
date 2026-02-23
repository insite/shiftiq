using Microsoft.AspNetCore.Mvc;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Security API")]
public class SecretController : ControllerBase
{
    private readonly IPrincipalProvider _identityService;

    public SecretController(IPrincipalProvider identityService)
    {
        _identityService = identityService;
    }

    [HttpGet("api/security/secrets/introspect")]
    [EndpointName("introspectSecret")]
    [SecretAuthorize]
    public IActionResult IntrospectAsync()
    {
        var principal = _identityService.GetPrincipal();

        return Ok(principal);
    }
}