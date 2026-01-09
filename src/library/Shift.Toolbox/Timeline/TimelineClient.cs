using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace Shift.Toolbox
{
    public class TimelineClient
    {
        private readonly ApiSettings _api;
        private readonly SecuritySettings _security;
        private readonly int _lifetimeLimitInSecond;

        public TimelineClient(ApiSettings api, SecuritySettings security, int lifetimeLimitInSeconds = JwtRequest.DefaultLifetime)
        {
            _api = api;
            _security = security;
            _lifetimeLimitInSecond = lifetimeLimitInSeconds;
        }

        public async Task<ApiResult> QueueCommandAsync(string name, string data, string token)
        {
            var content = new StringContent(data, Encoding.UTF8, "application/json");

            var http = new HttpClient()
            {
                BaseAddress = new Uri(_api.BaseUrl)
            };

            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await http.PostAsync($"timeline/commands?command={name}", content);

            var result = new ApiResult<string>(response.StatusCode, response.Headers);

            try
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                if (result.IsOK())
                    result.Data = responseContent;
                else
                    result.Problem = Problem.Deserialize(responseContent);
            }
            catch
            {

            }

            return result;
        }

        public ApiResult QueueCommand(ICommand command, string token = null)
        {
            var commandName = command.GetType().Name.ToKebabCase();

            var endpoint = $"timeline/commands?command={commandName}";

            var responseContent = string.Empty;

            try
            {
                var bearer = token ?? CreateToken(command.OriginUser, command.OriginOrganization);

                var client = new HttpClient();

                client.BaseAddress = new Uri(_api.BaseUrl);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);

                var serializer = new JsonSerializer2();

                var serialized = serializer.Serialize(command);

                var content = new StringContent(serialized, Encoding.UTF8, "application/json");

                var response = TaskRunner.RunSync(async () => await client.PostAsync(endpoint, content));

                var status = new ApiResult<string>(response.StatusCode, response.Headers);

                try
                {
                    responseContent = TaskRunner.RunSync(response.Content.ReadAsStringAsync);

                    status.Data = responseContent;
                }

                catch { }

                return status;
            }
            catch (Exception ex)
            {
                var status = new ApiResult<string>(HttpStatusCode.InternalServerError, null);

                status.Problem = CreateError("An unexpected error occurred.", ex, endpoint, responseContent);

                return status;
            }
        }

        public string CreateToken(Guid user, Guid organization)
        {
            var secret = _security.Secret;

            var subject = user.ToString();

            var issuer = GetType().FullName;

            var audience = _security.Token.Audience;

            var expiry = DateTime.UtcNow.Add(TimeSpan.FromSeconds(_lifetimeLimitInSecond));

            var claims = new Dictionary<ClaimName, string>
            {
                { ClaimName.UserId, user.ToString() },
                { ClaimName.OrganizationId, organization.ToString() }
            };

            var jwt = new Jwt(claims, subject, issuer, audience, expiry);

            var encoder = new JwtEncoder();

            var token = encoder.Encode(jwt, secret);

            return token;
        }

        private Problem CreateError(string summary, Exception ex, string endpoint, string responseContent)
        {
            var problem = new Problem()
            {
                Title = summary,
                Detail = ex?.Message
            };

            problem.Extensions["Endpoint"] = endpoint;
            problem.Extensions["Response"] = responseContent;

            return problem;
        }

        public async Task<ApiResult<JwtResponse>> GetTokenAsync(string secret)
        {
            var secretBody = new JwtRequest { Secret = secret };

            var secretJson = JsonSerializer.Serialize(secretBody);

            var secretContent = new StringContent(secretJson, Encoding.UTF8, "application/json");

            var http = new HttpClient()
            {
                BaseAddress = new Uri(_api.BaseUrl)
            };

            var tokenResponse = await http.PostAsync("token", secretContent);

            var result = new ApiResult<JwtResponse>(tokenResponse.StatusCode, tokenResponse.Headers);

            if (!result.IsOK())
                return result;

            var tokenContent = await tokenResponse.Content.ReadAsStringAsync();

            result.Data = JsonSerializer.Deserialize<JwtResponse>(tokenContent);

            return result;
        }
    }
}