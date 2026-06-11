using Microsoft.AspNetCore.Authorization;

namespace Shift.Api;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public abstract class RequirePermissionAttribute : AuthorizeAttribute
{
    public string Resource { get; set; }

    protected RequirePermissionAttribute(string scheme, string resource)
    {
        AuthenticationSchemes = scheme;
        Resource = resource;
        Policy = resource;
    }

    protected RequirePermissionAttribute(string scheme, string resource, AuthorityAccess access)
    {
        AuthenticationSchemes = scheme;
        Resource = resource;
        Policy = $"{resource}:authority:{(int)access}";
    }

    protected RequirePermissionAttribute(string scheme, string resource, DataAccess access)
    {
        AuthenticationSchemes = scheme;
        Resource = resource;
        Policy = $"{resource}:data:{(int)access}";
    }

    protected RequirePermissionAttribute(string scheme, string resource, FeatureAccess access)
    {
        AuthenticationSchemes = scheme;
        Resource = resource;
        Policy = $"{resource}:feature:{(int)access}";
    }
}
