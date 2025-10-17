using System.Security.Claims;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

using Shift.Common;

namespace Shift.Api;

public class BearerAuthenticationHandler : AuthenticationHandler<BearerAuthenticationOptions>
{
    private readonly IClaimConverter _converter;
    private readonly SecuritySettings _security;

    public BearerAuthenticationHandler(
        IOptionsMonitor<BearerAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IClaimConverter converter,
        SecuritySettings security)

        : base(options, logger, encoder)
    {
        _converter = converter;
        _security = security;
    }

    /// <remarks>
    /// IMPORTANT NOTE!
    /// When supporting multiple authentication schemes, the handler MUST return values that allow the authentication
    /// system to try other schemes (when appropriate). This means:
    ///    1. Return NoResult() when the authentication data for this scheme is not present
    ///    2. Return Success() when authentication succeeds
    ///    3. Return Fail() when the authentication data for this scheme is present but invalid
    ///    4. Do NOT interfere with other schemes - only handle your own authentication type
    /// </remarks>
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var helper = new AuthenticationHelper(Context, AuthenticationSchemeNames.Bearer);

        if (helper.IsFailed())
            return helper.Fail();

        var encoder = new JwtEncoder();

        var token = encoder.Extract("Bearer", Request.Headers["Authorization"].ToString());

        if (token == null)
            return AuthenticateResult.NoResult();

        var isValidated = encoder.Validate(AuthenticationSchemeNames.Bearer, token,
            _security.Secret, _security.Token.Audience,
            _converter, out ClaimsPrincipal? principal, out ValidationFailure validation);

        if (!isValidated)
            return helper.Fail("Bearer authorization token validation failed", validation);

        return await Task.FromResult(helper.CreateTicket(principal));
    }
}
