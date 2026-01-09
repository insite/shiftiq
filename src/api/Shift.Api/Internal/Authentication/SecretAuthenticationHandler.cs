using System.Security.Claims;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

using Shift.Common;

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
public class SecretAuthenticationHandler : AuthenticationHandler<SecretAuthenticationOptions>
{
    private readonly IClaimConverter _converter;
    private readonly IPrincipalSearch _search;

    public SecretAuthenticationHandler(
        IOptionsMonitor<SecretAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IClaimConverter converter,
        SecuritySettings security,
        IPrincipalSearch search)

        : base(options, logger, encoder)
    {
        _converter = converter;
        _search = search;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var helper = new AuthenticationHelper(Context, AuthenticationSchemeNames.Secret);

        if (helper.IsFailed())
            return helper.Fail();

        var encoder = new JwtEncoder();

        var secret = encoder.Extract("Secret", Request.Headers["Authorization"].ToString());

        if (secret == null)
            return AuthenticateResult.NoResult();

        var principal = _search.GetPrincipal(secret);

        if (principal == null)
            return helper.Fail("Invalid client secret: no matching security principal");

        var claims = _converter.ToClaims(principal);

        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, AuthenticationSchemeNames.Secret));

        return await Task.FromResult(helper.CreateTicket(claimsPrincipal));
    }
}
