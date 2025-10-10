using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;

namespace Shift.Api.Sdk.Core.Demo;

public class ApiTokenProvider
{
    private readonly HttpClient _httpClient;
    private string _cachedToken = string.Empty;
    private static readonly SemaphoreSlim Lock = new(1, 1);

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
            var response = await _httpClient.PostAsJsonAsync($"{endpoint}/token", new { Secret = secret });
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