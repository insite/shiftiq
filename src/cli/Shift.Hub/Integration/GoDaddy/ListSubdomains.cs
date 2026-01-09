namespace Shift.Hub.Integration.GoDaddy;

public class ListSubdomains
{
    public void Execute(string? env, string? apiKey, string? apiSecret, string? domain, string? type)
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

        if (string.IsNullOrEmpty(type))
        {
            Console.Error.WriteLine("Type is a required input parameter for this command. Use one of the following: A, CNAME, MX, TXT");
            return;
        }

        Console.WriteLine($"Listing {type} subdomains for {domain}...");

        var isProduction = string.Equals(env, "prod", StringComparison.OrdinalIgnoreCase);

        var records = ApiClient.ListSubdomains(isProduction, apiKey, apiSecret, domain, type);
        Console.WriteLine($"Found {records.Length} record(s):");

        for (int i = 0; i < records.Length; i++)
        {
            var record = records[i];

            Console.WriteLine($"{i + 1}. Name='{record.Name}', Data='{record.Data}', TTL={record.TTL}");
        }
    }
}
