using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Shift.Common.Integrations.Proview
{
    public class ProviewClient
    {
        private class PlaybackResponse
        {
            public string token { get; set; }
        }

        private class LoginResponse
        {
            public string access_token { get; set; }
            public string refresh_token { get; set; }
        }

        private class ProviewSessionDetails
        {
            public class Session
            {
                public int id { get; set; }
            }

            public Session session { get; set; }
        }

        private const string _authBaseUrl = "https://auth.talview.com";
        private const string _baseUrl = "https://proctoring.talview.com";

        private readonly HttpClient _client;
        private readonly ProviewSettings _settings;
        private string _accessToken;
        private string _refreshToken;

        public ProviewClient(HttpClient client, AppSettings appSettings)
        {
            _client = client;
            _settings = appSettings.Integration.Proview;
        }

        public async Task<string> GetPlaybackTokenAsync(string sessionUuid, CancellationToken ct = default)
        {
            await EnsureAuthenticated(false, ct);

            var (isAuthenticated, result) = await GetPlaybackTokenAsync(sessionUuid, false, ct);
            if (isAuthenticated)
                return result;

            await EnsureAuthenticated(true, ct);

            (_, result) = await GetPlaybackTokenAsync(sessionUuid, true, ct);

            return result;
        }

        public async Task<string> GetSessionDetailsAsync(string sessionUuid, CancellationToken ct = default)
        {
            await EnsureAuthenticated(false, ct);

            var (isAuthenticated, result) = await GetSessionDetailsAsync(sessionUuid, false, ct);
            if (isAuthenticated)
                return result;

            await EnsureAuthenticated(true, ct);

            (_, result) = await GetSessionDetailsAsync(sessionUuid, true, ct);

            return result;
        }

        public async Task<string> CompleteSessionAsync(string sessionUuid, CancellationToken ct = default)
        {
            var sessionId = await GetSessionIdAsync(sessionUuid);

            await EnsureAuthenticated(false, ct);

            var (isAuthenticated, result) = await CompleteSessionAsync(sessionId, false, ct);
            if (isAuthenticated)
                return result;

            await EnsureAuthenticated(true, ct);

            (_, result) = await CompleteSessionAsync(sessionId, true, ct);

            return result;
        }

        private async Task<int> GetSessionIdAsync(string sessionUuid, CancellationToken ct = default)
        {
            var result = await GetSessionDetailsAsync(sessionUuid, ct);

            var details = JsonConvert.DeserializeObject<ProviewSessionDetails>(result);

            return details.session.id;
        }

        private async Task EnsureAuthenticated(bool force, CancellationToken ct)
        {
            if (!string.IsNullOrEmpty(_accessToken) && !force)
                return;

            if (!string.IsNullOrEmpty(_refreshToken) && await RefreshAsync(ct))
                return;

            await LoginAsync(ct);
        }

        private async Task LoginAsync(CancellationToken ct)
        {
            var (_, result) = await PostAuthAsync("/v1/user/login", new
            {
                username = _settings.Username,
                password = _settings.Password
            }, true, ct);

            _accessToken = result.access_token;
            _refreshToken = result.refresh_token;
        }

        private async Task<bool> RefreshAsync(CancellationToken ct)
        {
            var (isAuthenticated, result) = await PostAuthAsync("/v1/user/token", new
            {
                refresh_token = _refreshToken
            }, false, ct);

            if (!isAuthenticated)
                return false;

            _accessToken = result.access_token;
            _refreshToken = result.refresh_token;

            return true;
        }

        private async Task<(bool, string)> GetPlaybackTokenAsync(string sessionUuid, bool throwAuthError, CancellationToken ct)
        {
            var json = JsonConvert.SerializeObject(new
            {
                proctor_token = _settings.Token,
                session_uuid = sessionUuid,
                validity = 1440
            });

            string content;

            using (var request = new HttpRequestMessage(new HttpMethod("POST"), $"{_baseUrl}/v1/token/playback"))
            {
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                request.Headers.Add("is-universal-proview", "true");
                request.Headers.Add("app-id", _settings.AppId);

                AddAccessToken(request);

                var response = await _client.SendAsync(request, ct);
                content = await response.Content.ReadAsStringAsync();

                if (!throwAuthError && response.StatusCode == HttpStatusCode.Unauthorized)
                    return (false, null);

                if (!response.IsSuccessStatusCode)
                    throw new InvalidOperationException($"Failed to request Proview. (POST) Url: /v1/token/playback. Error:\n" + content);
            }

            var result = JsonConvert.DeserializeObject<PlaybackResponse>(content);

            return (true, result.token);
        }

        private async Task<(bool, string)> GetSessionDetailsAsync(string sessionUuid, bool throwAuthError, CancellationToken ct)
        {
            string content;

            using (var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/v1/session/{sessionUuid}"))
            {
                request.Headers.Add("app-id", _settings.AppId);

                AddAccessToken(request);

                var response = await _client.SendAsync(request, ct);
                content = await response.Content.ReadAsStringAsync();

                if (!throwAuthError && response.StatusCode == HttpStatusCode.Unauthorized)
                    return (false, null);

                if (!response.IsSuccessStatusCode)
                    throw new InvalidOperationException($"Failed to request Proview. (GET) Url: /v1/session/{sessionUuid}. Error:\n" + content);
            }

            return (true, content);
        }

        private async Task<(bool, string)> CompleteSessionAsync(int sessionId, bool throwAuthError, CancellationToken ct)
        {
            var json = JsonConvert.SerializeObject(new
            {
                session_id = sessionId,
                status = "completed"
            });

            string content;

            using (var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"{_baseUrl}/v1/session/status"))
            {
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                request.Headers.Add("app-id", _settings.AppId);

                AddAccessToken(request);

                var response = await _client.SendAsync(request, ct);
                content = await response.Content.ReadAsStringAsync();

                if (!throwAuthError && response.StatusCode == HttpStatusCode.Unauthorized)
                    return (false, null);

                if (!response.IsSuccessStatusCode)
                    throw new InvalidOperationException($"Failed to request Proview. (PATCH) Url: /v1/session/status. Error:\n" + content);
            }

            return (true, content);
        }

        private async Task<(bool, LoginResponse)> PostAuthAsync(string relativeUrl, object input, bool throwAuthError, CancellationToken ct)
        {
            var json = JsonConvert.SerializeObject(input);

            string content;

            using (var request = new HttpRequestMessage(HttpMethod.Post, $"{_authBaseUrl}{relativeUrl}"))
            {
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                request.Headers.Add("app-id", _settings.AppId);

                var response = await _client.SendAsync(request, ct);
                content = await response.Content.ReadAsStringAsync();

                if (!throwAuthError && response.StatusCode == HttpStatusCode.Unauthorized)
                    return (false, null);

                if (!response.IsSuccessStatusCode)
                    throw new InvalidOperationException($"Failed to request Proview. Url: {relativeUrl}. Error:\n" + content);
            }

            var result = JsonConvert.DeserializeObject<LoginResponse>(content);

            return (true, result);
        }

        private void AddAccessToken(HttpRequestMessage request)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        }
    }
}