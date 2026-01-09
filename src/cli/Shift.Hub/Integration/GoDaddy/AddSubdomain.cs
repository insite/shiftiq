namespace Shift.Hub.Integration.GoDaddy;

public class AddSubdomain
{
    public void Execute(string? env, string? apiKey, string? apiSecret, string? domain, string? subdomain, string? data, string? lifetimeText)
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            Console.Error.WriteLine("ApiKey is a required input parameter for this command.");
            return;
        }

        if (string.IsNullOrEmpty(apiSecret))
        {
            Console.Error.WriteLine("ApiSecret is a required input parameter for this command.");
            return;
        }

        if (string.IsNullOrEmpty(domain))
        {
            Console.Error.WriteLine("Domain is a required input parameter for this command.");
            return;
        }

        if (string.IsNullOrEmpty(subdomain))
        {
            Console.Error.WriteLine("Subdomain is a required input parameter for this command.");
            return;
        }

        if (string.IsNullOrEmpty(data))
        {
            Console.Error.WriteLine("Data is a required input parameter for this command.");
            return;
        }

        if (!int.TryParse(lifetimeText, out var lifetime))
        {
            Console.Error.WriteLine("Lifetime is a required numeric input parameter for this command.");
            return;
        }

        Console.WriteLine($"Adding CNAME for {subdomain}.{domain} with TTL {lifetime}");

        var isProduction = string.Equals(env, "prod", StringComparison.OrdinalIgnoreCase);

        ApiClient.AddSubdomain(isProduction, apiKey, apiSecret, domain, subdomain, data, lifetime);

        Console.WriteLine("CNAME was successfully added");
    }
}
