using Microsoft.AspNetCore.Authorization;

namespace Shift.Api;

public class SecretAuthorizeAttribute : AuthorizeAttribute
{
    public SecretAuthorizeAttribute()
    {
        AuthenticationSchemes = AuthenticationSchemeNames.Secret;
    }
}
