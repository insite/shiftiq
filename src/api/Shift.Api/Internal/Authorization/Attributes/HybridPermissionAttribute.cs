namespace Shift.Api;

public class HybridPermissionAttribute : RequirePermissionAttribute
{
    public HybridPermissionAttribute(string resource)
        : base(AuthenticationSchemeNames.Hybrid, resource) { }

    public HybridPermissionAttribute(string resource, FeatureAccess access)
        : base(AuthenticationSchemeNames.Hybrid, resource, access) { }

    public HybridPermissionAttribute(string resource, DataAccess access)
        : base(AuthenticationSchemeNames.Hybrid, resource, access) { }

    public HybridPermissionAttribute(string resource, AuthorityAccess access)
        : base(AuthenticationSchemeNames.Hybrid, resource, access) { }
}
