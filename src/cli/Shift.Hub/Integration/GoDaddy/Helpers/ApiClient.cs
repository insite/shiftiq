using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shift.Hub.Integration.GoDaddy
{
    static class ApiClient
    {
        public class SubdomainRecord
        {
            [JsonPropertyName("name")]
            public required string Name { get; set; }

            [JsonPropertyName("data")]
            public required string Data { get; set; }

            [JsonPropertyName("type")]
            public required string Type { get; set; }

            [JsonPropertyName("ttl")]
            public int TTL { get; set; }
        }

        private const string OteUrl = "https://api.ote-godaddy.com";
        private const string ProductionUrl = "https://api.godaddy.com";

        private static readonly HttpClient _client = new HttpClient();

        public static SubdomainRecord[] ListSubdomains(bool isProduction, string apiKey, string apiSecret, string domain, string type)
        {
            var endpointUrl = $"/v1/domains/{domain}/records/{type}";
            var json = Send(HttpMethod.Get, isProduction, apiKey, apiSecret, endpointUrl, null);
            SubdomainRecord[]? records;

            try
            {
                records = JsonSerializer.Deserialize<SubdomainRecord[]>(json) ?? throw new ArgumentNullException("records");
            }
            catch
            {
                throw new ApplicationException($"Cannot deserialize result:\n{json}");
            }

            foreach (var record in records)
            {
                if (string.IsNullOrEmpty(record.Name) || string.IsNullOrEmpty(record.Data))
                    throw new ApplicationException($"Empty record:\n{json}");
            }

            return records;
        }

        public static void AddSubdomain(bool isProduction, string apiKey, string apiSecret, string domain, string subdomain, string data, int ttl)
        {
            var body = new SubdomainRecord[]
            {
                new SubdomainRecord
                {
                    Name = subdomain,
                    Data = data,
                    TTL = ttl,
                    Type = "CNAME"
                }
            };
            var bodyJson = JsonSerializer.Serialize(body);

            var endpointUrl = $"/v1/domains/{domain}/records";
            Send(HttpMethod.Patch, isProduction, apiKey, apiSecret, endpointUrl, bodyJson);
        }

        public static void RemoveSubdomain(bool isProduction, string apiKey, string apiSecret, string domain, string subdomain)
        {
            var endpointUrl = $"/v1/domains/{domain}/records/CNAME/{subdomain}";
            Send(HttpMethod.Delete, isProduction, apiKey, apiSecret, endpointUrl, null);
        }

        private static string Send(
            HttpMethod method,
            bool isProduction,
            string apiKey,
            string apiSecret,
            string endpointUrl,
            string? body
            )
        {
            var task = Task.Run(async () => await SendAsync(method, isProduction, apiKey, apiSecret, endpointUrl, body));

            return task.GetAwaiter().GetResult();
        }

        private static async Task<string> SendAsync(
            HttpMethod method,
            bool isProduction,
            string apiKey,
            string apiSecret,
            string endpointUrl,
            string? body
            )
        {
            var baseUrl = isProduction ? ProductionUrl : OteUrl;
            var url = $"{baseUrl}{endpointUrl}";

            var message = new HttpRequestMessage(method, url);
            message.Headers.Add("Authorization", $"sso-key {apiKey}:{apiSecret}");

            if (!string.IsNullOrEmpty(body))
                message.Content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(message);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new ApplicationException($"Error. HttStatus: {response.StatusCode}. Message:\n{responseContent}");

            return responseContent;
        }
    }
}
