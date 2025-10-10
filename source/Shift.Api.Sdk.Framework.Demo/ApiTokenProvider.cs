using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Shift.Api.Sdk.Framework.Demo
{
    public class ApiTokenProvider
    {
        private readonly HttpClient _httpClient;
        private string _cachedToken = string.Empty;
        private static readonly SemaphoreSlim Lock = new SemaphoreSlim(1, 1);

        public ApiTokenProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetTokenAsync(string secret, string endpoint)
        {
            if (!string.IsNullOrEmpty(_cachedToken))
            {
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(_cachedToken);
                var expiryTimeText = jwt.Claims.Single(claim => claim.Type == "exp").Value;
                var expiryDateTime = UnixTimeStampToDateTime(int.Parse(expiryTimeText));
                if (expiryDateTime > DateTime.UtcNow)
                {
                    return _cachedToken;
                }
            }

            string newToken = string.Empty;
            try
            {
                await Lock.WaitAsync();
                var json = JsonSerializer.Serialize(new { Secret = secret });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{endpoint}/token", content);
                newToken = await response.Content.ReadAsStringAsync();
                _cachedToken = newToken;
            }
            finally
            {
                Lock.Release();
            }
            return newToken;
        }

        private static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
    }
}