using Microsoft.AspNetCore.Authorization;

namespace Shift.Api;

public class SecretAuthorizeAttribute : AuthorizeAttribute
{
    public SecretAuthorizeAttribute()
    {
        AuthenticationSchemes = AuthenticationSchemeNames.Secret;
    }

    public SecretAuthorizeAttribute(string policy) : base(policy)
    {
        Policy = policy;
        AuthenticationSchemes = AuthenticationSchemeNames.Secret;
    }
}
