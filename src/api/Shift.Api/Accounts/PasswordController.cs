using Microsoft.AspNetCore.Mvc;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Accounts API")]
public class PasswordController : ControllerBase
{
    [HttpPost("api/accounts/passwords/hash")]
    [EndpointName("generatePasswordHash")]
    public async Task<ActionResult<PasswordHashResponse>> GenerateHashAsync()
    {
        var password = string.Empty;

        using (var reader = new StreamReader(Request.Body))
        {
            password = await reader.ReadToEndAsync();
        }

        var hash = new PasswordHashResponse
        {
            Hash = PasswordHash.CreateHash(password)
        };

        return Ok(hash);
    }
}
