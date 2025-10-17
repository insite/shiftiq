using Microsoft.AspNetCore.Authorization;

namespace Shift.Api;

public class HybridAuthorizeAttribute : AuthorizeAttribute
{
    public HybridAuthorizeAttribute()
    {
        AuthenticationSchemes = AuthenticationSchemeNames.Hybrid;
    }

    public HybridAuthorizeAttribute(string policy) : base(policy)
    {
        Policy = policy;
        AuthenticationSchemes = AuthenticationSchemeNames.Hybrid;
    }
}
