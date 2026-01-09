using Humanizer;

using Microsoft.AspNetCore.Mvc;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Security API")]
public class TokenController : ControllerBase
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

    [HttpPost("security/tokens/generate")]
    public IActionResult Generate([FromBody] JwtRequest request)
    {
        var ip = GetClientIPAddress();

        if (ip.IsEmpty())
            return ProblemFactory.Unauthorized("Missing IP address").ToActionResult(this);

        if (request.Secret.IsEmpty())
            return ProblemFactory.Unauthorized("Missing secret").ToActionResult(this);

        var errors = new List<string>();

        var tokenSettings = _securitySettings.Token;

        var isWhitelisted = IsWhitelisted(ip, tokenSettings.Whitelist);

        // Disallow a custom lifetime that exceeds 90 days.

        var ninetyDays = 7776000; // 90 days × 24 hours × 60 minutes × 60 seconds = 7,776,000 seconds

        if (request.Lifetime != null && request.Lifetime > ninetyDays)
            request.Lifetime = ninetyDays;

        var principal = _principalSearch.GetPrincipal(request, ip, isWhitelisted, tokenSettings.Lifetime, errors);

        if (principal == null)
        {
            var problem = errors.Count > 0
                ? ProblemFactory.Unauthorized(string.Join(". ", errors))
                : ProblemFactory.Unauthorized("Invalid secret");

            return problem.ToActionResult(this);
        }

        if (!principal.IsDeveloper)
        {
            var problem = errors.Count > 0
                ? ProblemFactory.Unauthorized(string.Join(". ", errors))
                : ProblemFactory.Unauthorized("Developer access is not granted to your account");

            return problem.ToActionResult(this);
        }

        var lifetimeInSeconds = principal.Claims.Lifetime ?? JwtRequest.DefaultLifetime;

        var lifetimeDescription = TimeSpan.FromSeconds(lifetimeInSeconds).Humanize();

        var lifetime = $"{lifetimeInSeconds:n0} seconds (~{lifetimeDescription})";

        var body = new
        {
            AccessToken = CreateToken(principal, lifetimeInSeconds, request.Debug),
            TokenType = "Bearer",
            ExpiresIn = lifetimeInSeconds,
            Lifetime = lifetime
        };

        return Ok(body);
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

    [HttpGet("security/tokens/introspect")]
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

    [HttpPost("security/tokens/validate")]
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

    private string CreateToken(IShiftPrincipal principal, int lifetime, bool debug)
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

    private void AddDebugClaims(IShiftPrincipal principal, Dictionary<ClaimName, List<string>> claims)
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