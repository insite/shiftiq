using Microsoft.AspNetCore.Authorization;

namespace Shift.Api;

public class BearerAuthorizeAttribute : AuthorizeAttribute
{
    public BearerAuthorizeAttribute()
    {
        AuthenticationSchemes = AuthenticationSchemeNames.Bearer;
    }
}
