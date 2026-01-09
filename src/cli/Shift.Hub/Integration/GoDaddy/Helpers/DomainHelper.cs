namespace Shift.Hub.Integration.GoDaddy;

internal static class DomainHelper
{
    public static bool IsExplicitBindingRequiredForProduction { get; set; } = false;

    public class MissingSubdomainInfo
    {
        public string CompanyName { get; set; }
        public string OrganizationCode { get; set; }
        public string Domain { get; set; }
        public string Subdomain { get; set; }

        public MissingSubdomainInfo(DbHelper.OrganizationInfo organization, string domain, string subdomain)
        {
            CompanyName = organization.CompanyName;
            OrganizationCode = organization.OrganizationCode;
            Domain = domain;
            Subdomain = subdomain;
        }
    }

    public static List<MissingSubdomainInfo> GetMissingSubdomains(DbHelper db, string? env, string apiKey, string apiSecret)
    {
        var partition = GetPartition(db);

        var hostParts = partition.HostName.Split('.');
        if (hostParts.Length < 2)
            throw new InternalException($"Invalid Partition.HostName parameter value: " + partition.HostName);

        var organizations = db.GetOpenOrganizations();
        if (organizations.Count == 0)
            throw new InternalException("No open organizations found.");

        var domain = hostParts[^2] + "." + hostParts[^1];
        var isProduction = string.Equals(env, "prod", StringComparison.OrdinalIgnoreCase);

        var cnames = ApiClient.ListSubdomains(isProduction, apiKey, apiSecret, domain, "CNAME");
        if (cnames.Length == 0)
            throw new InternalException("No subdomain records found.");

        var result = new List<MissingSubdomainInfo>();

        foreach (var org in organizations)
        {
            var subdomains = new List<string>();

            if (IsExplicitBindingRequiredForProduction)
                subdomains.Add(org.OrganizationCode);

            subdomains.Add("sandbox-" + org.OrganizationCode);

            subdomains.Add("dev-" + org.OrganizationCode);

            foreach (var subdomain in subdomains)
                if (!cnames.Any(x => subdomain.Equals(x.Name, StringComparison.OrdinalIgnoreCase)))
                    result.Add(new MissingSubdomainInfo(org, domain, subdomain));
        }

        return result.OrderBy(x => x.Subdomain).ToList();
    }

    private static DbHelper.PartitionInfo GetPartition(DbHelper db)
    {
        var partition = db.GetPartition();

        if (partition.PartitionSlug.Length == 0)
            throw new InternalException("The value of the \"Partition:Slug\" parameter is null.");

        if (partition.HostName.Length == 0)
            throw new InternalException("The value of the \"Host:Name\" parameter is null.");

        return partition;
    }
}
