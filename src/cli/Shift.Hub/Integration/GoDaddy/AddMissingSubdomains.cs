namespace Shift.Hub.Integration.GoDaddy;

public class AddMissingSubdomains
{
    public void Execute(string? connectionString, string? env, string? apiKey, string? apiSecret, string? data, string? lifetimeText)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            Console.Error.WriteLine("ConnectionString is a required input parameter for this command.");
            return;
        }

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

        var db = new DbHelper(connectionString);

        List<DomainHelper.MissingSubdomainInfo> missingSubdomains;

        try
        {
            missingSubdomains = DomainHelper.GetMissingSubdomains(db, env, apiKey, apiSecret);
        }
        catch (InternalException intex)
        {
            Console.Error.WriteLine(intex.Message);
            return;
        }

        AddSubdomains(env, apiKey, apiSecret, data, lifetime, missingSubdomains);
    }

    private static void AddSubdomains(string? env, string apiKey, string apiSecret, string data, int lifetime, List<DomainHelper.MissingSubdomainInfo> missingSubdomains)
    {
        if (missingSubdomains.Count == 0)
        {
            Console.WriteLine("No missing subdomains found.");
            return;
        }

        Console.WriteLine($"Found {missingSubdomains.Count} missing subdomains.");

        for (var i = 0; i < missingSubdomains.Count; i++)
        {
            var item = missingSubdomains[i];

            Console.WriteLine($"Adding CNAME for {item.Domain} with TTL {lifetime}");

            var isProduction = string.Equals(env, "prod", StringComparison.OrdinalIgnoreCase);

            ApiClient.AddSubdomain(isProduction, apiKey, apiSecret, item.Domain, item.Subdomain, data, lifetime);

            Console.WriteLine("CNAME was successfully added");
        }
    }
}
