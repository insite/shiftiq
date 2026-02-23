using Humanizer;

using Microsoft.AspNetCore.Mvc;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Security API")]
public class TokenController : ShiftControllerBase
{
    private readonly SecuritySettings _securitySettings;

    private readonly IClaimConverter _claimConverter;
    private readonly IPrincipalSearch _principalSearch;

    public TokenController(SecuritySettings securitySettings,
        IClaimConverter claimConverter, IPrincipalSearch principalSearch)
    {
        _securitySettings = securitySettings;
        _principalSearch = principalSearch;
        _claimConverter = claimConverter;
    }

    [HttpPost("api/security/tokens/generate")]
    public IActionResult Generate([FromBody] JwtRequest request)
    {
        try
        {
            var ip = GetClientIPAddress();

            if (ip.IsEmpty())
                return Unauthorized("Your token request must originate from a recognized IP address");

            if (request.Secret.IsEmpty())
                return Unauthorized("Your token request is missing a valid client secret");

            ValidateLifetime(request);

            var tokenSettings = _securitySettings.Token;

            var isWhitelisted = IsWhitelisted(ip, tokenSettings.Whitelist);

            var errors = new List<string>();

            var principal = _principalSearch.GetPrincipal(request, ip, isWhitelisted, tokenSettings.Lifetime, errors);

            if (errors.Count > 0)
                return Unauthorized(string.Join(". ", errors));

            if (!principal.IsDeveloper)
                return Unauthorized("Developer access is not granted to your account");

            var lifetimeInSeconds = principal.Claims.Lifetime ?? JwtRequest.DefaultLifetime;

            var lifetimeDisplay = TimeSpan.FromSeconds(lifetimeInSeconds).Humanize();

            var body = new
            {
                AccessToken = CreateToken(principal, lifetimeInSeconds, request.Debug),
                TokenType = "Bearer",
                ExpiresIn = lifetimeInSeconds,
                Lifetime = $"{lifetimeInSeconds:n0} seconds (~{lifetimeDisplay})"
            };

            return Ok(body);
        }
        catch (SecretExpiredException)
        {
            return Unauthorized("Your client secret is expired");
        }
        catch (SecretNotFoundException)
        {
            return Unauthorized("Your client secret is not recognized");
        }
        catch (SentinelNotFoundException)
        {
            return Unauthorized("Sentinel not found");
        }
        catch (UserNotFoundException)
        {
            return Unauthorized("User not found");
        }
    }

    private void ValidateLifetime(JwtRequest request)
    {
        // Disallow a custom lifetime that exceeds 365 days.

        var oneYear = 365 * 24 * 60 * 60; // 365 days × 24 hours × 60 minutes × 60 seconds = 31,536,000 seconds

        if (request.Lifetime != null && request.Lifetime > oneYear)
            request.Lifetime = oneYear;
    }

    private bool IsWhitelisted(string ipAddress, string whitelist)
    {
        if (ipAddress.IsEmpty())
            return false;

        if (whitelist.IsEmpty())
            return true;

        var list = whitelist.Parse();

        return ipAddress.MatchesAny(list);
    }

    [HttpGet("api/security/tokens/introspect")]
    [BearerAuthorize()]
    public IActionResult Introspect()
    {
        var encoder = new Common.JwtEncoder();

        var token = encoder.Extract("Bearer", Request.Headers["Authorization"]);

        var jwt = encoder.Decode(token);

        var principal = _claimConverter.ToPrincipal(jwt);

        var result = new
        {
            Jwt = jwt,
            Principal = principal
        };

        return Ok(result);
    }

    [HttpPost("api/security/tokens/validate")]
    public async Task<IActionResult> ValidateAsync()
    {
        var token = string.Empty;

        using (var reader = new StreamReader(Request.Body))
        {
            token = await reader.ReadToEndAsync();
        }

        var encoder = new JwtEncoder();

        var jwt = encoder.Decode(token);

        var tokenSettings = _securitySettings.Token;

        var audience = tokenSettings.Audience;

        var issuer = $"{Request.Scheme}://{Request.Host}{Request.Path}";

        var result = new
        {
            jwt.Subject,

            SignatureVerification = encoder.VerifySignature(token, _securitySettings.Secret) ? "Passed" : "Failed",

            ExpiryVerification = !jwt.IsExpired() ? $"Not Expired ({jwt.GetMinutesUntilExpiry():n0} minutes remaining)" : "Expired",

            AudienceVerification = jwt.Audience == audience ? $"Passed" : $"Failed (expected {audience})",

            IssuerVerification = "Skipped (bearer tokens may be issued by API v1 or v2)"
        };

        return Ok(new { result });
    }

    private string CreateToken(IPrincipal principal, int lifetime, bool debug)
    {
        var securityClaims = _claimConverter.ToClaims(principal);

        var principalClaims = _claimConverter.ToDictionary(securityClaims);

        if (debug)
            AddDebugClaims(principal, principalClaims);

        var secret = _securitySettings.Secret;

        var subject = principal.User.Email;

        var tokenSettings = _securitySettings.Token;

        var audience = tokenSettings.Audience;

        var issuer = $"{Request.Scheme}://{Request.Host}{Request.Path}";

        var expiry = DateTime.UtcNow.Add(TimeSpan.FromMinutes(lifetime));

        var claims = new Jwt(principalClaims, subject, issuer, audience, expiry);

        var encoder = new JwtEncoder();

        var jwt = encoder.Encode(claims, secret);

        return jwt;
    }

    private void AddDebugClaims(IPrincipal principal, Dictionary<ClaimName, List<string>> claims)
    {
        if (principal.Roles != null && principal.Roles.Any())
        {
            var list = principal.Roles
                .Select(x => $"{x.Identifier} (v{UuidFactory.GetVersion(x.Identifier)} UUID) {x.Name}")
                .ToList();

            claims.Add(ClaimName.RoleDebug, list);
        }

        var user = principal.User;

        var userId = principal.User.Identifier;

        if (userId != Guid.Empty)
        {
            claims.Add(ClaimName.UserDebug, [$"{userId} (v{UuidFactory.GetVersion(userId)} UUID) {user.Name} <{user.Email}>"]);
        }
    }

    private string GetClientIPAddress()
        => Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1";
}