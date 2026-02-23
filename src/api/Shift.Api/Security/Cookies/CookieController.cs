using System.Web;

using Microsoft.AspNetCore.Mvc;

using Shift.Contract.Presentation;
using Shift.Service.Directory;

namespace Shift.Api;

/// <summary>
/// Provides endpoints for creating, validating, and managing authentication cookies.
/// </summary>
[ApiController]
[ApiExplorerSettings(GroupName = "Security API")]
[Route("api/security/cookies")]
public partial class CookieController : ShiftControllerBase
{
    private readonly AppSettings _appSettings;

    private readonly PersonService _personService;

    private readonly OrganizationService _organizationService;

    private readonly OrganizationAdapter _organizationAdapter;

    private readonly GroupService _groupService;

    private readonly IPrincipalProvider _identityService;

    private readonly IReactService _reactService;

    public CookieController(
        AppSettings appSettings,
        PersonService personService,
        OrganizationService organizationService,
        OrganizationAdapter organizationAdapter,
        GroupService groupService,
        IPrincipalProvider identityService,
        IReactService reactService
        )
    {
        _appSettings = appSettings;

        _personService = personService;

        _organizationService = organizationService;

        _organizationAdapter = organizationAdapter;

        _groupService = groupService;

        _identityService = identityService;

        _reactService = reactService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(string organizationCode, string email)
    {
        var domain = GetSecurityDomain();
        var settings = _appSettings.Security.Cookie;

        if (domain != "localhost" && !settings.Debug)
            return BadRequest("An untrusted cookie can be injected by the client only for debugging purposes.");

        var token = await new LoginHelper(_personService, _organizationService, _groupService, _identityService).LoginAsync(organizationCode, email);
        if (token == null)
            return BadRequest("Failed to create the token");

        AppendSecurityCookie(token);

        return Ok(new { token });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        var environment = _appSettings.Release.GetEnvironment();

        var settings = _appSettings.Security.Cookie;
        var idSettings = _appSettings.Security.IdCookie;

        var domain = environment.IsLocal() ? "localhost" : _appSettings.Partition.Domain;

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

        return Ok();
    }

    /// <summary>
    /// Creates a new authentication cookie with the specified name and lifetime using the raw 
    /// request body as the cookie value.
    /// </summary>
    /// <param name="name">The name of the cookie to create. If not provided, uses the default 
    /// cookie name from API configuration settings.</param>
    /// <param name="lifetime">The lifetime of the cookie in seconds. If not provided, uses the 
    /// default lifetime from API configuration settings.</param>
    /// <returns>An <see cref="IActionResult"/> containing the cookie value that was set, or a 
    /// BadRequest if not in local environment.</returns>
    /// <remarks>
    /// In contrast to security/cookies/generate endpoint, this endpoint makes no assumptions about the request body.
    /// This means you can POST whatever cookie name and cookie value you want (encoded/escaped or not) and this method
    /// tries to create a new cookie for you. This is useful for testing the behaviour of the API when it receives
    /// unexpected (or corrupted) cookie tokens. Please note this endpoint is available in localhost environments only!
    /// </remarks>
    [HttpPost("create")]
    public async Task<IActionResult> CreateAsync(string? name, int? lifetime)
    {
        var environment = _appSettings.Release.GetEnvironment();

        var settings = _appSettings.Security.Cookie;

        var domain = environment.IsLocal() ? "localhost" : _appSettings.Partition.Domain;

        if (domain != "localhost" && !settings.Debug)
            return BadRequest("An untrusted cookie can be injected by the client only for debugging purposes.");

        var value = string.Empty;

        using (var reader = new StreamReader(Request.Body))
        {
            value = await reader.ReadToEndAsync();
        }

        // 

        var now = DateTimeOffset.UtcNow;

        var expiry = lifetime.HasValue
            ? now.AddSeconds(lifetime.Value)
            : now.AddHours(settings.Lifetime);

        // Mimic the same-site functionality from the security/cookies/generate endpoint.

        var sameSite = settings.Encrypt ? SameSiteMode.Lax : SameSiteMode.None;

        Response.Cookies.Append(
            name ?? settings.Name,
            value,
            new CookieOptions
            {
                Domain = domain,
                Expires = expiry,
                Path = settings.Path,
                SameSite = sameSite,
                Secure = true,
                HttpOnly = true
            });

        return Ok(new { value });
    }

    /// <summary>
    /// Decodes and deserializes a cookie token from the request body.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> containing the decoded cookie data, or a BadRequest 
    /// if decoding fails.</returns>
    [HttpPost("decode")]
    public async Task<IActionResult> DecodeAsync()
    {
        var encoded = string.Empty;

        using (var reader = new StreamReader(Request.Body))
        {
            encoded = await reader.ReadToEndAsync();
        }

        var encoder = new CookieTokenEncoder();

        var cookie = encoder.Deserialize(encoded, _appSettings.Security.Cookie.Encrypt, _appSettings.Security.Cookie.Secret, false);

        if (cookie == null)
            return BadRequest($"Failed to{(_appSettings.Security.Cookie.Encrypt ? " decrypt and " : " ")}decode HTTP request body.");

        return Ok(cookie);
    }

    /// <summary>
    /// Generates a new authentication cookie using the provided token from the request body.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> containing the token that was used to create the 
    /// cookie, or a BadRequest if not in local environment.</returns>
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateAsync()
    {
        var environment = _appSettings.Release.GetEnvironment();

        var settings = _appSettings.Security.Cookie;

        var domain = environment.IsLocal() ? "localhost" : _appSettings.Partition.Domain;

        if (domain != "localhost" && !settings.Debug)
            return BadRequest("An untrusted cookie can be injected by the client only for debugging purposes.");

        var token = string.Empty;

        using (var reader = new StreamReader(Request.Body))
        {
            token = await reader.ReadToEndAsync();
        }

        var expiry = DateTimeOffset.UtcNow.AddHours(settings.Lifetime);

        // The client should send an encrypted cookie with "same-site" requests and "cross-site" top-level navigations.
        // The client should should disable same-site restrictions for plain-text (unencrypted) cookie values.

        var sameSite = settings.Encrypt ? SameSiteMode.Lax : SameSiteMode.None;

        // The Append method invokes Uri.EscapeDataString on the cookie value even if the value is already encoded. The
        // simplest hack for this is to decode it... and then let the Append method re-encode it. You can see this in
        // the source code for Microsoft.AspNetCore.Http.Internal.ResponseCookies.Append, and also discussed here:
        // https://github.com/dotnet/aspnetcore/issues/2906

        token = Uri.UnescapeDataString(token);

        Response.Cookies.Append(
            settings.Name,
            token,
            new CookieOptions
            {
                Domain = domain,
                Expires = expiry,
                Path = settings.Path,
                SameSite = sameSite,
                Secure = true,
                HttpOnly = true
            });

        return Ok(new { token });
    }

    /// <summary>
    /// Introspects the current authentication cookie and returns detailed information about the 
    /// authentication and authorization status.
    /// </summary>
    /// <returns>An <see cref="ActionResult"/> containing an introspection report with 
    /// authentication details, cookie information, and authorization status.</returns>
    [HttpGet("introspect")]
    [CookieAuthorize]
    public IActionResult IntrospectAsync()
    {
        var cookieSettings = _appSettings.Security.Cookie;

        var cookie = Request.Cookies[cookieSettings.Name];

        if (cookie == null)
            return BadRequest($"Cookie is missing: {cookieSettings.Name}");

        var encrypt = cookieSettings.Encrypt;

        var secret = cookieSettings.Secret;

        var report = new IntrospectionReport();

        report.Authentication.Scheme = AuthenticationSchemeNames.Cookie;
        report.Authentication.Status = "Cookie authentication succeeded";

        var encoder = new CookieTokenEncoder();
        report.Authentication.Cookie.Decoded = HttpUtility.UrlDecode(cookie);
        report.Authentication.Cookie.Deserialized = encoder.Deserialize(cookie, encrypt, secret, false);

        report.Authorization.Policy = "Security.Cookies.Introspect";
        report.Authorization.Status = "Security claims principal authorization succeeded";

        report.Cookies = Request.Cookies;

        return Ok(report);
    }

    /// <summary>
    /// Validates a cookie token from the request body without setting it as an authentication 
    /// cookie.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> containing the validated cookie data, or a 
    /// BadRequest if validation fails.</returns>
    [HttpPost("validate")]
    public async Task<IActionResult> ValidateAsync()
    {
        var token = string.Empty;

        using (var reader = new StreamReader(Request.Body))
        {
            token = await reader.ReadToEndAsync();
        }

        var encoder = new CookieTokenEncoder();

        var encrypt = _appSettings.Security.Cookie.Encrypt;

        var secret = _appSettings.Security.Cookie.Secret;

        try
        {
            var cookie = encoder.Deserialize(token, encrypt, secret, false);

            if (cookie == null)
                return BadRequest("Cookie validation failed.");

            return Ok(new { cookie });
        }
        catch (CookieSerializationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("change-language/{language}")]
    public async Task<IActionResult> ChangeLanguageAsync([FromRoute] string language)
    {
        var cookieSettings = _appSettings.Security.Cookie;
        var cookie = Request.Cookies[cookieSettings.Name];
        if (cookie == null)
            return BadRequest($"Cookie is missing: {cookieSettings.Name}");

        var principal = _identityService.GetPrincipal();
        if (principal.UserId == Guid.Empty)
            return BadRequest($"Invalid principal user");

        var organization = await _organizationService.RetrieveAsync(principal.Organization.Identifier);
        var organizationData = organization != null ? _organizationAdapter.ToData(organization) : null;

        if (organizationData == null || !organizationData.Languages.Any(x => x.TwoLetterISOLanguageName == language))
            return BadRequest($"Unsupported language: {language}");

        CookieToken token;

        try
        {
            token = new CookieTokenEncoder().Deserialize(cookie, cookieSettings.Encrypt, cookieSettings.Secret, false);
        }
        catch (CookieSerializationException)
        {
            return BadRequest($"Cookie is broken: {cookieSettings.Name}");
        }

        token.ResetCreated();
        token.ResetID();
        token.Language = language;

        AppendSecurityCookie(token);

        return Ok();
    }

    [HttpPost("change-theme/{theme}")]
    public IActionResult ChangeTheme([FromRoute] string theme)
    {
        var domain = GetSecurityDomain();
        var settings = _appSettings.Security.Cookie;

        if (domain != "localhost" && !settings.Debug)
            return BadRequest("This endpoint is available only for debugging purposes");

        string normalizedTheme;

        if (string.Equals(theme, "light", StringComparison.OrdinalIgnoreCase))
            normalizedTheme = "Light";
        else if (string.Equals(theme, "dark", StringComparison.OrdinalIgnoreCase))
            normalizedTheme = "Dark";
        else
            return BadRequest("Invalid theme");

        Response.Cookies.Append(
            "Shift.UI.ThemeMode",
            normalizedTheme,
            new CookieOptions
            {
                Domain = domain,
                Expires = DateTime.Now.AddDays(90),
                Path = settings.Path,
                SameSite = SameSiteMode.Lax,
                HttpOnly = false
            }
        );

        return Ok();
    }

    private void AppendSecurityCookie(CookieToken token)
    {
        var domain = GetSecurityDomain();
        var settings = _appSettings.Security.Cookie;
        var idSettings = _appSettings.Security.IdCookie;
        var now = DateTimeOffset.UtcNow;
        var expiry = now.AddMinutes(settings.Lifetime);
        var sameSite = settings.Encrypt ? SameSiteMode.Lax : SameSiteMode.None;

        token.ValidationKey = CookieToken.CreateValidationKey(token, settings.Secret);

        var serializedToken = new CookieTokenEncoder().Serialize(token, settings.Encrypt, settings.Secret, false);

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
    }

    private string GetSecurityDomain()
    {
        return string.Equals(Request.Host.Host, "localhost", StringComparison.OrdinalIgnoreCase)
            ? Request.Host.Host
            : _appSettings.Partition.Domain;
    }
}