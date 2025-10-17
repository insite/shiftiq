using Microsoft.AspNetCore.Authorization;

namespace Shift.Api;

public class CookieAuthorizeAttribute : AuthorizeAttribute
{
    public CookieAuthorizeAttribute()
    {
        AuthenticationSchemes = AuthenticationSchemeNames.Cookie;
    }

    public CookieAuthorizeAttribute(string policy) : base(policy)
    {
        Policy = policy;
        AuthenticationSchemes = AuthenticationSchemeNames.Cookie;
    }
}
