namespace Shift.Api;

public static class AuthenticationSchemeNames
{
    public const string Hybrid = "HybridAuth"; // requires a JWT bearer token **OR** an HTTP authentication cookie
    public const string Bearer = "BearerAuth"; // requires a JWT bearer token
    public const string Cookie = "CookieAuth"; // requires an HTTP authentication cookie
    public const string Secret = "SecretAuth"; // requires a client secret (API key)
}
