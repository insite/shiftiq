using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Shift.Common
{
    public class MicrosoftEntra
    {
        #region Classes

        public class TokenResponse
        {
            [JsonProperty("token_type")]
            public string TokenType { get; set; }

            [JsonProperty("scope")]
            public string Scope { get; set; }

            [JsonProperty("expires_in")]
            public int ExpiresIn { get; set; }

            [JsonProperty("ext_expires_in")]
            public int ExtendedExpiresIn { get; set; }

            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            [JsonProperty("id_token")]
            public string IdToken { get; set; }
        }

        private class JwtHeader
        {
            [JsonProperty("typ")]
            public string Type { get; set; }

            [JsonProperty("alg")]
            public string Algorithm { get; set; }

            [JsonProperty("kid")]
            public string KeyId { get; set; }
        }

        private class IdTokenClaims
        {
            [JsonProperty("aud")]
            public string Audience { get; set; }

            [JsonProperty("iss")]
            public string Issuer { get; set; }

            [JsonProperty("iat")]
            public long IssuedAt { get; set; }

            [JsonProperty("nbf")]
            public long NotBefore { get; set; }

            [JsonProperty("exp")]
            public long ExpiresAt { get; set; }

            [JsonProperty("email")]
            public string Email { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("given_name")]
            public string FirstName { get; set; }

            [JsonProperty("family_name")]
            public string LastName { get; set; }

            [JsonProperty("oid")]
            public string ObjectId { get; set; }

            [JsonProperty("preferred_username")]
            public string PreferredUsername { get; set; }

            [JsonProperty("rh")]
            public string RefreshTokenHandle { get; set; }

            [JsonProperty("sid")]
            public string SessionId { get; set; }

            [JsonProperty("sub")]
            public string Subject { get; set; }

            [JsonProperty("tid")]
            public string TenantId { get; set; }

            [JsonProperty("uti")]
            public string TokenId { get; set; }

            [JsonProperty("ver")]
            public string Version { get; set; }
        }

        #endregion

        private const string MsTenantUrl = "https://login.microsoftonline.com/common";
        private const string AuthorizeUrl = MsTenantUrl + "/oauth2/v2.0/authorize";
        private const string TokenUrl = MsTenantUrl + "/oauth2/v2.0/token";

        private readonly AzureADSecret _secret;
        private readonly OAuthRedirectUrl _redirectUrl;

        public MicrosoftEntra(AzureADSecret secret, OAuthRedirectUrl redirectUrl)
        {
            _secret = secret;
            _redirectUrl = redirectUrl;
        }

        public WebUrl CreateAuthorizationRequest(Guid organizationId, string url)
        {
            var stateId = OAuthCacheService.Add(new OAuthCacheEntry
            {
                Method = OAuthMethod.Microsoft,
                OrganizationId = organizationId,
                Url = url
            });

            var result = new WebUrl(AuthorizeUrl);
            result.QueryString["response_type"] = "code";
            result.QueryString["response_mode"] = "query";
            result.QueryString["scope"] = "openid profile email";
            result.QueryString["state"] = $"MS:{stateId}";
            result.QueryString["client_id"] = _secret.ClientId;
            result.QueryString["redirect_uri"] = _redirectUrl.Get();
            return result;
        }

        public OAuthResult GetOAuthResult(string code)
        {
            return TaskRunner.RunSync(GetOAuthResultAsync, code);
        }

        private async Task<OAuthResult> GetOAuthResultAsync(string code)
        {
            var token = await GetTokenAsync(code);
            if (token == null || token.IdToken.IsEmpty())
                return null;

            var claims = DecodeIdToken(token.IdToken);
            if (claims == null)
                return null;

            var username = claims.PreferredUsername;
            if (username.IsEmpty())
                return null;

            return new OAuthResult
            {
                Authorized = true,
                UserName = username
            };
        }

        private IdTokenClaims DecodeIdToken(string data)
        {
            var parts = data.Split('.');
            if (parts.Length != 3)
                return null;

            JwtHeader header;
            IdTokenClaims claims;

            try
            {
                header = JsonConvert.DeserializeObject<JwtHeader>(Base64UrlDecode(parts[0]));
                claims = JsonConvert.DeserializeObject<IdTokenClaims>(Base64UrlDecode(parts[1]));
            }
            catch
            {
                return null;
            }

            if (header == null || claims == null)
                return null;

            if (header.Algorithm != "RS256" || header.KeyId.IsEmpty())
                return null;

            var timeNow = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var timeError = 5 * 60;

            if (claims.Audience != _secret.ClientId)
                return null;

            if (claims.ExpiresAt <= timeNow - timeError)
                return null;

            if (claims.NotBefore > timeNow + timeError)
                return null;

            return claims;
        }

        private static string Base64UrlDecode(string input)
        {
            var s = input.Replace('-', '+').Replace('_', '/');

            switch (s.Length % 4)
            {
                case 2:
                    s += "==";
                    break;
                case 3:
                    s += "=";
                    break;
            }

            return Encoding.UTF8.GetString(Convert.FromBase64String(s));
        }

        private async Task<TokenResponse> GetTokenAsync(string code)
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "authorization_code",
                ["scope"] = "openid profile email",

                ["code"] = code,
                ["client_id"] = _secret.ClientId,
                ["client_secret"] = _secret.ClientSecret,
                ["redirect_uri"] = _redirectUrl.Get(),
            });

            var response = await StaticHttpClient.Client.PostAsync(TokenUrl, content);
            if (!response.IsSuccessStatusCode)
                return null;

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResult = JsonConvert.DeserializeObject<TokenResponse>(responseContent);

            return tokenResult == null
                || tokenResult.TokenType != "Bearer"
                || string.IsNullOrWhiteSpace(tokenResult.AccessToken)
                ? null
                : tokenResult;
        }
    }
}
