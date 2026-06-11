namespace Shift.Api;

public class BearerPermissionAttribute : RequirePermissionAttribute
{
    public BearerPermissionAttribute(string resource)
        : base(AuthenticationSchemeNames.Bearer, resource) { }

    public BearerPermissionAttribute(string resource, FeatureAccess access)
        : base(AuthenticationSchemeNames.Bearer, resource, access) { }

    public BearerPermissionAttribute(string resource, DataAccess access)
        : base(AuthenticationSchemeNames.Bearer, resource, access) { }

    public BearerPermissionAttribute(string resource, AuthorityAccess access)
        : base(AuthenticationSchemeNames.Bearer, resource, access) { }
}
