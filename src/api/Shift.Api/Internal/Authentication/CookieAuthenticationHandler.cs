using System.Security.Claims;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Shift.Api;

/// <remarks>
/// IMPORTANT NOTE!
/// When supporting multiple authentication schemes, the handler MUST return values that allow the authentication
/// system to try other schemes (when appropriate). This means:
///    1. Return NoResult() when the authentication data for this scheme is not present
///    2. Return Success() when authentication succeeds
///    3. Return Fail() when the authentication data for this scheme is present but invalid
///    4. Do NOT interfere with other schemes - only handle your own authentication type
/// </remarks>
public class CookieAuthenticationHandler : AuthenticationHandler<CookieAuthenticationOptions>
{
    private readonly IClaimConverter _converter;
    private readonly SecuritySettings _security;

    public CookieAuthenticationHandler(IOptionsMonitor<CookieAuthenticationOptions> options,
        ILoggerFactory logger, UrlEncoder encoder, IClaimConverter converter,
        SecuritySettings security)

        : base(options, logger, encoder)
    {
        _converter = converter;
        _security = security;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var helper = new AuthenticationHelper(Context, AuthenticationSchemeNames.Cookie);

        if (helper.IsFailed())
            return helper.Fail();

        var cookieSettings = _security.Cookie;

        var cookie = Request.Cookies[cookieSettings.Name];

        if (cookie == null)
            return AuthenticateResult.NoResult();

        var encrypt = cookieSettings.Encrypt;

        var secret = cookieSettings.Secret;

        var encoder = new CookieTokenEncoder();

        CookieToken? token = null;

        try
        {
            token = encoder.Deserialize(cookie, encrypt, secret);
        }
        catch (CookieSerializationException ex)
        {
            return helper.Fail(ex.Message);
        }

        if (token == null)
            return helper.Fail("The authentication cookie failed to deserialize");

        if (token.IsExpired())
            return helper.Fail("The authentication cookie expired");

        var principal = ToClaimsPrincipal(token, "Cookie");

        if (principal == null)
            return helper.Fail("The authentication cookie failed to convert to a security claims principal");

        return await Task.FromResult(helper.CreateTicket(principal));
    }

    ClaimsPrincipal? ToClaimsPrincipal(CookieToken claims, string authenticationType)
    {
        if (string.IsNullOrEmpty(claims.OrganizationCode)
            || string.IsNullOrEmpty(claims.UserEmail))
        {
            return null;
        }

        var list = new List<Claim>
            {
                _converter.ToClaim(ClaimName.OrganizationCode, claims.OrganizationCode),
                _converter.ToClaim(ClaimName.UserEmail, claims.UserEmail),
                _converter.ToClaim(ClaimName.UserLanguage, claims.Language ?? ClaimConverter.DefaultLanguage),
                _converter.ToClaim(ClaimName.UserTimeZone, claims.TimeZoneId ?? ClaimConverter.DefaultLanguage),
                _converter.ToClaim(ClaimName.UserIp, Request.HttpContext.Connection.RemoteIpAddress?.ToString()),
            };

        if (claims.OrganizationIdentifier != null)
        {
            list.Add(_converter.ToClaim(ClaimName.OrganizationId, claims.OrganizationIdentifier.Value.ToString()));
        }

        if (claims.UserIdentifier != null)
        {
            list.Add(_converter.ToClaim(ClaimName.UserId, claims.UserIdentifier.Value.ToString()));
        }

        foreach (var role in claims.UserRoles)
        {
            list.Add(_converter.ToClaim(ClaimName.Role, role.ToString()));
        }

        var (access, systemRoles) = PrincipalSearch.GetAuthority(claims);

        list.Add(_converter.ToClaim(ClaimName.Authority, access.ToString()));

        foreach (var role in systemRoles)
            list.Add(_converter.ToClaim(ClaimName.Role, role.Name));

        return new ClaimsPrincipal(new ClaimsIdentity(list, authenticationType));
    }
}
