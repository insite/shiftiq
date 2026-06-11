using Shift.Sdk.Service.Security.Cookies;

namespace Shift.Api;

public class CookieService(
    IHttpContextAccessor httpContextAccessor,
    AppSettings appSettings
) : ICookieService
{
    private static readonly CookieTokenEncoder _cookieTokenDecoder = new CookieTokenEncoder();

    private HttpResponse Response
    {
        get => httpContextAccessor.HttpContext?.Response ?? throw new ArgumentNullException("httpContextAccessor.HttpContext.Response");
    }

    private HttpRequest Request
    {
        get => httpContextAccessor.HttpContext?.Request ?? throw new ArgumentNullException("httpContextAccessor.HttpContext.Request");
    }

    public bool Debug => appSettings.Security.Cookie.Debug;
    public string Path => appSettings.Security.Cookie.Path;

    public string GetSecurityDomain()
    {
        return string.Equals(Request.Host.Host, "localhost", StringComparison.OrdinalIgnoreCase)
            ? Request.Host.Host
            : appSettings.Partition.Domain;
    }

    public CookieToken? GetCookieToken()
    {
        var cookieSettings = appSettings.Security.Cookie;
        var cookie = Request.Cookies[cookieSettings.Name];
        if (cookie == null)
            return null;

        try
        {
            return _cookieTokenDecoder.Deserialize(cookie, cookieSettings.Encrypt, cookieSettings.Secret, false);
        }
        catch (CookieSerializationException)
        {
            return null;
        }
    }

    public void AppendSecurityCookie(CookieToken token)
    {
        token.ResetCreated();
        token.ResetID();

        var domain = GetSecurityDomain();
        var settings = appSettings.Security.Cookie;
        var idSettings = appSettings.Security.IdCookie;
        var now = DateTimeOffset.UtcNow;
        var expiry = now.AddMinutes(settings.Lifetime);
        var sameSite = settings.Encrypt ? SameSiteMode.Lax : SameSiteMode.None;

        token.ValidationKey = CookieToken.CreateValidationKey(token, settings.Secret);

        var serializedToken = _cookieTokenDecoder.Serialize(token, settings.Encrypt, settings.Secret, false);

        Response.Cookies.Append(
            settings.Name,
            serializedToken,
            new CookieOptions
            {
                Domain = domain,
                Expires = expiry,
                Path = settings.Path,
                SameSite = sameSite,
                Secure = true,
                HttpOnly = true
            }
        );

        Response.Cookies.Append(
            idSettings.Name,
            token.ID.ToString().ToLower(),
            new CookieOptions
            {
                Domain = domain,
                Expires = expiry,
                Path = settings.Path,
                SameSite = sameSite,
                Secure = true,
                HttpOnly = true
            }
        );

        Response.Headers.Append("X-Session-Refreshed", token.GetModified().UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss") + " UTC");
    }

    public void DeleteSecurityCookie()
    {
        var settings = appSettings.Security.Cookie;
        var idSettings = appSettings.Security.IdCookie;
        var domain = GetSecurityDomain();

        var sameSite = settings.Encrypt ? SameSiteMode.Lax : SameSiteMode.None;

        Response.Cookies.Delete(
            settings.Name,
            new CookieOptions
            {
                Domain = domain,
                Path = settings.Path,
                SameSite = sameSite,
                Secure = true,
                HttpOnly = true
            }
        );

        Response.Cookies.Delete(
            idSettings.Name,
            new CookieOptions
            {
                Domain = domain,
                Path = settings.Path,
                SameSite = sameSite,
                Secure = true,
                HttpOnly = true
            }
        );

        Response.Headers.Append("X-Session-Refreshed", DateTimeOffset.UtcNow.AddYears(-1).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss") + " UTC");
    }
}