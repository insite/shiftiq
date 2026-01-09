using Microsoft.AspNetCore.Authorization;

namespace Shift.Api;

public class BearerAuthorizeAttribute : AuthorizeAttribute
{
    public BearerAuthorizeAttribute()
    {
        AuthenticationSchemes = AuthenticationSchemeNames.Bearer;
    }

    public BearerAuthorizeAttribute(string policy) : base(policy)
    {
        Policy = policy;
        AuthenticationSchemes = AuthenticationSchemeNames.Bearer;
    }
}
