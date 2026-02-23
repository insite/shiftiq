namespace Shift.Api;

public class SecretPermissionAttribute : RequirePermissionAttribute
{
    public SecretPermissionAttribute(string resource)
        : base(AuthenticationSchemeNames.Secret, resource) { }

    public SecretPermissionAttribute(string resource, FeatureAccess access)
        : base(AuthenticationSchemeNames.Secret, resource, access) { }

    public SecretPermissionAttribute(string resource, DataAccess access)
        : base(AuthenticationSchemeNames.Secret, resource, access) { }

    public SecretPermissionAttribute(string resource, AuthorityAccess access)
        : base(AuthenticationSchemeNames.Secret, resource, access) { }
}