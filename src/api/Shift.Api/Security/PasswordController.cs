using Microsoft.AspNetCore.Mvc;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Security API")]
public class PasswordController : ControllerBase
{
    [HttpPost("api/security/passwords/hash")]
    [EndpointName("generatePasswordHash")]
    public async Task<IActionResult> GenerateHashAsync()
    {
        var password = string.Empty;

        using (var reader = new StreamReader(Request.Body))
        {
            password = await reader.ReadToEndAsync();
        }

        var hash = new
        {
            Hash = PasswordHash.CreateHash(password)
        };

        return Ok(hash);
    }
}