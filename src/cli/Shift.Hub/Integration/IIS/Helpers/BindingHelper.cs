using Microsoft.Web.Administration;

namespace Shift.Hub.Integration.IIS;

internal class BindingHelper
{
    public class MissingBindingInfo
    {
        public string CompanyName { get; set; }
        public string OrganizationCode { get; set; }
        public string Host { get; set; }
        public string Protocol { get; set; }

        public MissingBindingInfo(DbHelper.OrganizationInfo organization, string host, string protocol)
        {
            CompanyName = organization.CompanyName;
            OrganizationCode = organization.OrganizationCode;
            Host = host;
            Protocol = protocol;
        }
    }

    public static bool IsWildcardDefined(Site site, DbHelper db, string[] protocols)
    {
        var (domain, organizations) = GetDomainAndOrganizations(db);
        var wildcardDomain = "*." + domain;

        return protocols.All(protocol => site.Bindings.Any(binding => binding.Host == wildcardDomain && binding.Protocol == protocol));
    }

    public static List<MissingBindingInfo> GetMissingBindings(Site site, DbHelper db, string[] protocols, bool excludeWidcard)
    {
        var (domain, organizations) = GetDomainAndOrganizations(db);
        var wildcardDomain = "*." + domain;
        var result = new List<MissingBindingInfo>();

        foreach (var org in organizations)
        {
            var subomain = org.OrganizationCode + "." + domain;
            foreach (var protocol in protocols)
            {
                if (excludeWidcard && site.Bindings.Any(binding => binding.Host == wildcardDomain && binding.Protocol == protocol))
                    continue;

                if (!site.Bindings.Any(x => x.Host == subomain && x.Protocol == protocol))
                    result.Add(new MissingBindingInfo(org, subomain, protocol));
            }
        }

        return result;
    }

    private static (string domain, DbHelper.OrganizationInfo[] organizations) GetDomainAndOrganizations(DbHelper db)
    {
        var partition = GetPartition(db);

        var hostParts = partition.HostName.Split('.');
        if (hostParts.Length < 2)
            throw new InternalException($"Invalid Partition.HostName parameter value: " + partition.HostName);

        var organizations = db.GetOpenOrganizations();
        if (organizations.Count == 0)
            throw new InternalException("No open organizations found.");

        return (hostParts[^2] + "." + hostParts[^1], organizations.ToArray());
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

    private static readonly HashSet<string> ValidProtocol = new HashSet<string> { "http", "https" };

    public static string[] ParseProtocolsString(string value)
    {
        return value.Split(',')
            .Select(x => x.ToLower())
            .Where(ValidProtocol.Contains)
            .Distinct()
            .ToArray();
    }
}
