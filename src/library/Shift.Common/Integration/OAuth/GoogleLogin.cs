using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Newtonsoft.Json;

namespace Shift.Common
{
    public class GoogleLogin
    {
        class GoogleAuthResult
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string scope { get; set; }
            public string token_type { get; set; }
            public string id_token { get; set; }
        }

        class GoogleProfileInfo
        {
            public string sub { get; set; }
            public string name { get; set; }
            public string given_name { get; set; }
            public string family_name { get; set; }
            public string picture { get; set; }
            public string email { get; set; }
            public bool email_verified { get; set; }
            public string locale { get; set; }
        }

        const string AuthorizationRequestURL = "https://accounts.google.com/o/oauth2/v2/auth";
        const string GetTokenURL = "https://oauth2.googleapis.com/token";
        const string GetProfileURL = "https://www.googleapis.com/oauth2/v3/userinfo";

        private readonly GoogleSettings _google;
        private readonly OAuthRedirectUrl _redirectUrl;

        private string ClientId => _google.ClientId;

        private string ClientSecret => _google.ClientSecret;

        public GoogleLogin(GoogleSettings google, OAuthRedirectUrl redirectUrl)
        {
            _google = google;
            _redirectUrl = redirectUrl;
        }

        public string CreateAuthorizationRequest(Guid organizationId, string url, string host)
        {
            var stateId = OAuthCacheService.Add(new OAuthCacheEntry
            {
                Method = OAuthAuthenticationMethods.Google,
                TenantId = organizationId,
                URL = url
            });
            var sb = new StringBuilder(AuthorizationRequestURL);
            sb.Append("?");
            sb.Append($"client_id={ClientId}&");
            sb.Append("response_type=code&");
            sb.Append($"redirect_uri={HttpUtility.UrlEncode(_redirectUrl.Get())}&");
            sb.Append("scope=openid profile email&");
            sb.Append("access_type=online&");
            sb.Append($"state=GA:{stateId}");
            return sb.ToString();
        }

        public OAuthAuthenticationResult Authenticate(string code, Guid cacheId, Guid organizationId, string host)
        {
            var cacheEntry = OAuthCacheService.Get(cacheId);
            if (cacheEntry == null)
                return new OAuthAuthenticationResult();

            if (cacheEntry.TenantId != organizationId)
            {
                return new OAuthAuthenticationResult
                {
                    TenantMismatch = true,
                    CacheEntry = cacheEntry,
                };
            }

            OAuthCacheService.Remove(cacheId);

            return Shift.Common.TaskRunner.RunSync(GetProfileAsync, code, cacheEntry);
        }

        private async Task<OAuthAuthenticationResult> GetProfileAsync(string code, OAuthCacheEntry cacheEntry)
        {
            var accessToken = await GetAccessTokenAsync(code);
            if (string.IsNullOrEmpty(accessToken))
                return new OAuthAuthenticationResult();

            var request = new HttpRequestMessage(HttpMethod.Get, GetProfileURL);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await StaticHttpClient.Client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return new OAuthAuthenticationResult();

            var responseContent = await response.Content.ReadAsStringAsync();

            var userProfile = JsonConvert.DeserializeObject<GoogleProfileInfo>(responseContent);
            if (string.IsNullOrEmpty(userProfile?.email))
                return new OAuthAuthenticationResult();

            return new OAuthAuthenticationResult
            {
                Authorized = true,
                AuthenticationMethod = OAuthAuthenticationMethods.Google,
                BearerToken = accessToken,
                EmailAddress = userProfile.email,
                FirstName = userProfile.given_name,
                LastName = userProfile.family_name,
                MobilePhone = "",
                UserPrincipalName = userProfile.email,
                PreferredLanguage = userProfile.locale,
                CacheEntry = cacheEntry
            };
        }

        private async Task<string> GetAccessTokenAsync(string code)
        {
            var data = new StringBuilder();
            data.Append($"client_id={HttpUtility.UrlEncode(ClientId)}");
            data.Append($"&code={HttpUtility.UrlEncode(code)}");
            data.Append($"&redirect_uri={HttpUtility.UrlEncode(_redirectUrl.Get())}");
            data.Append($"&grant_type=authorization_code");
            data.Append($"&client_secret={HttpUtility.UrlEncode(ClientSecret)}");

            var content = new StringContent(data.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded");

            var response = await StaticHttpClient.Client.PostAsync(GetTokenURL, content);
            if (!response.IsSuccessStatusCode)
                return null;

            var responseContent = await response.Content.ReadAsStringAsync();
            var authResult = JsonConvert.DeserializeObject<GoogleAuthResult>(responseContent);

            return authResult == null || authResult.token_type != "Bearer" || string.IsNullOrWhiteSpace(authResult.access_token)
                ? null
                : authResult.access_token;
        }
    }
}
