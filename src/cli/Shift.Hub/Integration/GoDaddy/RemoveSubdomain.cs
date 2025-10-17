namespace Shift.Hub.Integration.GoDaddy;

public class RemoveSubdomain
{
    public void Execute(string? env, string? apiKey, string? apiSecret, string? domain, string? subdomain)
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

        Console.WriteLine($"Removing CNAME for {subdomain}.{domain}");

        var isProduction = string.Equals(env, "prod", StringComparison.OrdinalIgnoreCase);

        ApiClient.RemoveSubdomain(isProduction, apiKey, apiSecret, domain, subdomain);

        Console.WriteLine("CNAME was successfully removed");
    }
}
