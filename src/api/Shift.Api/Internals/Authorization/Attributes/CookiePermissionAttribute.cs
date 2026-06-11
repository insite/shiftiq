namespace Shift.Api;

public class CookiePermissionAttribute : RequirePermissionAttribute
{
    public CookiePermissionAttribute(string resource)
        : base(AuthenticationSchemeNames.Cookie, resource) { }

    public CookiePermissionAttribute(string resource, FeatureAccess access)
        : base(AuthenticationSchemeNames.Cookie, resource, access) { }

    public CookiePermissionAttribute(string resource, DataAccess access)
        : base(AuthenticationSchemeNames.Cookie, resource, access) { }

    public CookiePermissionAttribute(string resource, AuthorityAccess access)
        : base(AuthenticationSchemeNames.Cookie, resource, access) { }
}
