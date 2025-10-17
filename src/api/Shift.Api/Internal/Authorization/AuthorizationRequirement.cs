using Microsoft.AspNetCore.Authorization;

namespace Shift.Api;

public class AuthorizationRequirement : IAuthorizationRequirement
{
    public string Policy { get; set; }

    public AuthorizationRequirement(string policy)
    {
        Policy = policy;
    }
}
