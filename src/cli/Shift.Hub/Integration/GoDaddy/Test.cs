using System.Text;

namespace Shift.Hub.Integration.GoDaddy;

public class Test
{
    public void Execute(string apiKey, string apiSecret)
    {
        Console.WriteLine("API Key: {0}", apiKey);
        Console.WriteLine("API Secret: {0}", apiSecret);

        var task = Task.Run(async () => await ModifyDomainAsync(apiKey, apiSecret));

        task.GetAwaiter().GetResult();
    }

    private static async Task AddSubdomainAsync(string apiKey, string apiSecret)
    {
        var client = new HttpClient();

        var url = $"https://api.ote-godaddy.com/v1/domains/qqqddd112200.com/records";

        var body = @"
[{
  ""data"": ""@"",
  ""name"": ""local"",
  ""type"": ""CNAME""
}]
";

        var message = new HttpRequestMessage(HttpMethod.Patch, url);
        message.Headers.Add("Authorization", $"sso-key {apiKey}:{apiSecret}");
        message.Content = new StringContent(body, Encoding.UTF8, "application/json");

        var response = await client.SendAsync(message);
        var responseContent = await response.Content.ReadAsStringAsync();

        response.EnsureSuccessStatusCode();
    }

    private static async Task ListRecordsAsync(string apiKey, string apiSecret)
    {
        var client = new HttpClient();

        var url = $"https://api.ote-godaddy.com/v1/domains/qqqddd112200.com/records/CNAME";

        var message = new HttpRequestMessage(HttpMethod.Get, url);
        message.Headers.Add("Authorization", $"sso-key {apiKey}:{apiSecret}");

        var response = await client.SendAsync(message);
        var responseContent = await response.Content.ReadAsStringAsync();

        response.EnsureSuccessStatusCode();
    }

    private static async Task PurchaseDomainAsync(string apiKey, string apiSecret)
    {
        var client = new HttpClient();

        var url = $"https://api.ote-godaddy.com/v1/domains/purchase";

        var body = @"
{
  ""consent"": {
    ""agreedAt"": ""2024-09-25T09:09:09.9Z"",
    ""agreedBy"": ""string"",
    ""agreementKeys"": [
      ""DNRA""
    ]
  },
  ""contactAdmin"": {
    ""addressMailing"": {
      ""address1"": ""string"",
      ""address2"": ""string"",
      ""city"": ""string"",
      ""country"": ""US"",
      ""postalCode"": ""33601"",
      ""state"": ""CA""
    },
    ""email"": ""user@example.com"",
    ""fax"": """",
    ""jobTitle"": ""string"",
    ""nameFirst"": ""string"",
    ""nameLast"": ""string"",
    ""nameMiddle"": ""string"",
    ""organization"": ""string"",
    ""phone"": ""+1.23456789""
  },
  ""contactBilling"": {
    ""addressMailing"": {
      ""address1"": ""string"",
      ""address2"": ""string"",
      ""city"": ""string"",
      ""country"": ""US"",
      ""postalCode"": ""33601"",
      ""state"": ""CA""
    },
    ""email"": ""user@example.com"",
    ""fax"": """",
    ""jobTitle"": ""string"",
    ""nameFirst"": ""string"",
    ""nameLast"": ""string"",
    ""nameMiddle"": ""string"",
    ""organization"": ""string"",
    ""phone"": ""+1.23456789""
  },
  ""contactRegistrant"": {
    ""addressMailing"": {
      ""address1"": ""string"",
      ""address2"": ""string"",
      ""city"": ""string"",
      ""country"": ""US"",
      ""postalCode"": ""33601"",
      ""state"": ""CA""
    },
    ""email"": ""user@example.com"",
    ""fax"": """",
    ""jobTitle"": ""string"",
    ""nameFirst"": ""string"",
    ""nameLast"": ""string"",
    ""nameMiddle"": ""string"",
    ""organization"": ""string"",
    ""phone"": ""+1.23456789""
  },
  ""contactTech"": {
    ""addressMailing"": {
      ""address1"": ""string"",
      ""address2"": ""string"",
      ""city"": ""string"",
      ""country"": ""US"",
      ""postalCode"": ""33601"",
      ""state"": ""CA""
    },
    ""email"": ""user@example.com"",
    ""fax"": """",
    ""jobTitle"": ""string"",
    ""nameFirst"": ""string"",
    ""nameLast"": ""string"",
    ""nameMiddle"": ""string"",
    ""organization"": ""string"",
    ""phone"": ""+1.23456789""
  },
  ""domain"": ""qqqddd112200.com"",
  ""nameServers"": [
    ""godaddy.com"",
    ""godaddy2.com""
  ],
  ""period"": 1,
  ""privacy"": false,
  ""renewAuto"": true
}
";

        var message = new HttpRequestMessage(HttpMethod.Post, url);
        message.Headers.Add("Authorization", $"sso-key {apiKey}:{apiSecret}");
        message.Content = new StringContent(body, Encoding.UTF8, "application/json");

        var response = await client.SendAsync(message);
        var responseContent = await response.Content.ReadAsStringAsync();

        response.EnsureSuccessStatusCode();
    }

    private static async Task ModifyDomainAsync(string apiKey, string apiSecret)
    {
        var client = new HttpClient();

        var url = $"https://api.ote-godaddy.com/v1/domains/qqqddd112200.com";

        var body = @"
{
  ""nameServers"": [
    ""ns39.domaincontrol.com"",
    ""ns40.domaincontrol.com""
  ]
}
";

        var message = new HttpRequestMessage(HttpMethod.Patch, url);
        message.Headers.Add("Authorization", $"sso-key {apiKey}:{apiSecret}");
        message.Content = new StringContent(body, Encoding.UTF8, "application/json");

        var response = await client.SendAsync(message);
        var responseContent = await response.Content.ReadAsStringAsync();

        response.EnsureSuccessStatusCode();
    }

    private static async Task AvailableDomainAsync(string apiKey, string apiSecret)
    {
        var client = new HttpClient();

        var url = $"https://api.ote-godaddy.com/v1/domains/available?domain=qqqddd112200.com&checkType=FAST&forTransfer=false";

        var message = new HttpRequestMessage(HttpMethod.Get, url);
        message.Headers.Add("Authorization", $"sso-key {apiKey}:{apiSecret}");

        var response = await client.SendAsync(message);
        var responseContent = await response.Content.ReadAsStringAsync();

        response.EnsureSuccessStatusCode();
    }

    private static async Task<string> ListDomainsAsync(string apiKey, string apiSecret)
    {
        var client = new HttpClient();

        var url = $"https://api.ote-godaddy.com/v1/domains";

        var message = new HttpRequestMessage(HttpMethod.Get, url);
        message.Headers.Add("Authorization", $"sso-key {apiKey}:{apiSecret}");

        var response = await client.SendAsync(message);
        var responseContent = await response.Content.ReadAsStringAsync();

        response.EnsureSuccessStatusCode();

        return responseContent;
    }

    private static async Task DomainInfoAsync(string apiKey, string apiSecret)
    {
        var client = new HttpClient();

        var url = $"https://api.ote-godaddy.com/v1/domains/qqqddd112200.com";

        var message = new HttpRequestMessage(HttpMethod.Get, url);
        message.Headers.Add("Authorization", $"sso-key {apiKey}:{apiSecret}");

        var response = await client.SendAsync(message);
        var responseContent = await response.Content.ReadAsStringAsync();

        response.EnsureSuccessStatusCode();
    }
}
