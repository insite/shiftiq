namespace Shift.Hub.Integration.GoDaddy;

public class AddSubdomains
{
    private const int ApiThrottleDelayMs = 3000;

    public void Execute(string? env, string? apiKey, string? apiSecret, string? domain, string? inputFile, string? data, string? lifetimeText)
    {
        if (!ValidateInputs(apiKey, apiSecret, domain, inputFile, data, lifetimeText, out var lifetime))
            return;

        var subdomains = File.ReadAllLines(inputFile!)
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrWhiteSpace(line));

        var isProduction = string.Equals(env, "prod", StringComparison.OrdinalIgnoreCase);

        var existingCnames = ApiClient.ListSubdomains(isProduction, apiKey!, apiSecret!, domain!, "CNAME")
            .ToHashSet();

        foreach (var subdomain in subdomains)
        {
            if (existingCnames.Any(c => c.Name.Equals(subdomain, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine($"Skipping CNAME '{subdomain}' because it already exists");
                continue;
            }

            Console.WriteLine($"Adding CNAME for {subdomain}.{domain} with TTL {lifetime} seconds");

            ApiClient.AddSubdomain(isProduction, apiKey!, apiSecret!, domain!, subdomain, data!, lifetime);

            Thread.Sleep(ApiThrottleDelayMs);
        }
    }

    private bool ValidateInputs(
        string? apiKey, string? apiSecret,
        string? domain, string? inputFile,
        string? data, string? lifetimeText, out int lifetime)
    {
        lifetime = 0;

        var missingParams = new List<string>();

        if (string.IsNullOrWhiteSpace(apiKey)) missingParams.Add(nameof(apiKey));
        if (string.IsNullOrWhiteSpace(apiSecret)) missingParams.Add(nameof(apiSecret));
        if (string.IsNullOrWhiteSpace(domain)) missingParams.Add(nameof(domain));
        if (string.IsNullOrWhiteSpace(inputFile)) missingParams.Add(nameof(inputFile));
        if (string.IsNullOrWhiteSpace(data)) missingParams.Add(nameof(data));

        if (missingParams.Any())
        {
            Console.Error.WriteLine("Missing required parameters: " + string.Join(", ", missingParams));
            return false;
        }

        if (!File.Exists(inputFile!))
        {
            Console.Error.WriteLine($"Input file not found: {inputFile}");
            return false;
        }

        if (!int.TryParse(lifetimeText, out lifetime) || lifetime < 600)
        {
            Console.Error.WriteLine("Lifetime must be a valid numeric value greater than or equal to 600.");
            return false;
        }

        return true;
    }
}
