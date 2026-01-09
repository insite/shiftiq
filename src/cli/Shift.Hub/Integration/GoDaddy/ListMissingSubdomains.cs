namespace Shift.Hub.Integration.GoDaddy;

public class ListMissingSubdomains
{
    public void Execute(string? connectionString, string? env, string? apiKey, string? apiSecret)
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

        ListSubdomains(missingSubdomains);
    }

    private static void ListSubdomains(List<DomainHelper.MissingSubdomainInfo> missingSubdomains)
    {
        if (missingSubdomains.Count == 0)
        {
            Console.WriteLine("No missing subdomains found.");
            return;
        }

        Console.WriteLine($"Found {missingSubdomains.Count} missing subdomains:");

        for (int i = 0; i < missingSubdomains.Count; i++)
        {
            var item = missingSubdomains[i];

            Console.WriteLine($"{i + 1}. {item.Subdomain} ({item.CompanyName})");
        }
    }
}
