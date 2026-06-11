using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Newtonsoft.Json;

namespace Shift.Common
{
    public class AzureAD
    {
        const string AuthorizationRequestURL = "https://login.microsoftonline.com/common/oauth2/v2.0/authorize";
        const string GetTokenURL = "https://login.microsoftonline.com/common/oauth2/v2.0/token";
        const string GetProfileURL = "https://graph.microsoft.com/v1.0/me";

        private readonly AzureADSecret _secret;
        private readonly OAuthRedirectUrl _redirectUrl;

        public AzureAD(AzureADSecret secret, OAuthRedirectUrl redirectUrl)
        {
            _secret = secret;
            _redirectUrl = redirectUrl;
        }

        public string CreateAuthorizationRequest(Guid organizationId, string host, string url)
        {
            var stateId = OAuthCacheService.Add(new OAuthCacheEntry
            {
                Method = OAuthAuthenticationMethods.Microsoft,
                TenantId = organizationId,
                URL = url
            });
            var sb = new StringBuilder(AuthorizationRequestURL);
            sb.Append("?");
            sb.Append($"client_id={_secret.ClientId}&");
            sb.Append("response_type=code&");
            sb.Append($"redirect_uri={HttpUtility.UrlEncode(_redirectUrl.Get())}&");
            sb.Append("response_mode=query&");
            sb.Append("scope=user.read&");
            sb.Append($"state=MS:{stateId}");
            return sb.ToString();
        }

        class MicrosoftAuthResult
        {
            public string token_type { get; set; }
            public string scope { get; set; }
            public int expires_in { get; set; }
            public int ext_expires_in { get; set; }
            public string access_token { get; set; }
        }

        class MicrosoftProfileInfo
        {
            public string displayName { get; set; }
            public string givenName { get; set; }
            public string mail { get; set; }
            public string mobilePhone { get; set; }
            public string preferredLanguage { get; set; }
            public string surname { get; set; }
            public string userPrincipalName { get; set; }
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

            var userProfile = JsonConvert.DeserializeObject<MicrosoftProfileInfo>(responseContent);
            if (string.IsNullOrEmpty(userProfile?.userPrincipalName))
                return new OAuthAuthenticationResult();

            return new OAuthAuthenticationResult
            {
                Authorized = true,
                AuthenticationMethod = OAuthAuthenticationMethods.Microsoft,
                BearerToken = accessToken,
                EmailAddress = userProfile.mobilePhone,
                FirstName = userProfile.givenName,
                LastName = userProfile.surname,
                MobilePhone = userProfile.mobilePhone,
                UserPrincipalName = userProfile.userPrincipalName,
                PreferredLanguage = userProfile.preferredLanguage,
                CacheEntry = cacheEntry
            };
        }

        private async Task<string> GetAccessTokenAsync(string code)
        {
            var data = new StringBuilder();
            data.Append($"client_id={HttpUtility.UrlEncode(_secret.ClientId)}");
            data.Append($"&scope=user.read");
            data.Append($"&code={HttpUtility.UrlEncode(code)}");
            data.Append($"&redirect_uri={HttpUtility.UrlEncode(_redirectUrl.Get())}");
            data.Append($"&grant_type=authorization_code");
            data.Append($"&client_secret={HttpUtility.UrlEncode(_secret.ClientSecret)}");

            var content = new StringContent(data.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded");

            var response = await StaticHttpClient.Client.PostAsync(GetTokenURL, content);
            if (!response.IsSuccessStatusCode)
                return null;

            var responseContent = await response.Content.ReadAsStringAsync();
            var authResult = JsonConvert.DeserializeObject<MicrosoftAuthResult>(responseContent);

            return authResult == null || authResult.token_type != "Bearer" || string.IsNullOrWhiteSpace(authResult.access_token)
                ? null
                : authResult.access_token;
        }
    }
}
