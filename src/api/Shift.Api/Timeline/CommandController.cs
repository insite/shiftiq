using System.Net.Http.Headers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

using Shift.Common;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Timeline API")]
public class CommandController : ControllerBase
{
    private static MemoryCache<string, string> TokenCache = new MemoryCache<string, string>();

    private readonly ReleaseSettings _releaseSettings;
    private readonly SecuritySettings _securitySettings;
    private readonly ApiSettings _timelineServer;
    private readonly IPrincipalSearch _principalSearch;

    public CommandController(
        ReleaseSettings releaseSettings,
        SecuritySettings securitySettings,
        ShiftSettings apiSettings,
        IPrincipalSearch principalSearch)
    {
        _releaseSettings = releaseSettings;
        _securitySettings = securitySettings;
        _timelineServer = apiSettings.Api.Hosting.V1;
        _principalSearch = principalSearch;
    }

    [HttpPost("timeline/commands")]
    [HybridAuthorize(Policies.Timeline.Commands.Send)]
    public async Task<IActionResult> ProxyCommandAsync([FromQuery] string command)
    {
        // Relay the command to the v1 API (hosted by the main UI app).

        var environment = _releaseSettings.GetEnvironment();

        if (environment.Name != Common.EnvironmentName.Local
            && environment.Name != Common.EnvironmentName.Development)
        {
            var message = "Permission is granted only in Local and Development environments to"
                + " relay Timeline commands from the v2 API to the v1 API.";

            return Problem(message, null, 400);
        }

        try
        {
            var client = new Toolbox.TimelineClient(_timelineServer, _securitySettings);

            var commandData = await new StreamReader(Request.Body).ReadToEndAsync();

            var token = string.Empty;

            // If an authorization header is available then simply relay it directly to the v1 API endpoint. If there is
            // no authorization header then the default fallback is cookie authentication. In this case we can use the
            // security principal claims to determine the user and the organization; from this we can determine the
            // client secret for the person who submitted the command.

            if (Request.Headers.ContainsKey("Authorization"))
            {
                token = ExtractBearerToken(Request, "Authorization");
            }
            else
            {
                var secret = _principalSearch.GetSecret(HttpContext.User.Claims);

                if (TokenCache.Exists(secret))
                    token = TokenCache.Get(secret);

                else
                {
                    var tokenResult = await client.GetTokenAsync(secret);

                    if (tokenResult.Data == null)
                        return Problem();

                    token = tokenResult.Data.AccessToken;
                }

                TokenCache.Add(secret, token);
            }

            // Relay the request to the Timeline command server.

            var result = await client.QueueCommandAsync(command, commandData, token);

            if (!result.IsOK())
                return result.Problem.ToActionResult(this);

            return Ok(result);
        }
        catch (AccessDeniedException ad)
        {
            return Problem(ad.Message, null, 403);
        }
        catch (BadQueryException bq)
        {
            return Problem(bq.Message, null, 400);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, null, 500);
        }
    }

    private string? ExtractBearerToken(HttpRequest request, string headerName)
    {
        var header = request.Headers[headerName];

        if (StringValues.IsNullOrEmpty(header))
            return null;

        string headerValue = header.ToString();

        var parsed = AuthenticationHeaderValue.Parse(headerValue);

        if (parsed != null)
            return parsed.Parameter;

        return null;
    }
}
