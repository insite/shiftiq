using Microsoft.Web.Administration;

namespace Shift.Hub.Integration.IIS;

public class ListMissingBindings
{
    public void Execute(string? connectionString, string? siteName, string? protocolsText)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            Console.Error.WriteLine("ConnectionString is a required input parameter for this command.");
            return;
        }

        if (string.IsNullOrEmpty(siteName))
        {
            Console.Error.WriteLine("SiteName is a required input parameter for this command.");
            return;
        }

        if (string.IsNullOrEmpty(protocolsText))
        {
            Console.Error.WriteLine("Protocols is a required input parameter for this command.");
            return;
        }

        var protocols = BindingHelper.ParseProtocolsString(protocolsText);
        if (protocols.Length == 0)
        {
            Console.Error.WriteLine("Invalid protocols value.");
            return;
        }

        var serverManager = new ServerManager();
        var site = serverManager.Sites[siteName];

        if (site == null)
        {
            Console.Error.WriteLine($"Website is not found: {siteName}");
            return;
        }

        var db = new DbHelper(connectionString);

        List<BindingHelper.MissingBindingInfo> missingBindings;

        try
        {
            if (BindingHelper.IsWildcardDefined(site, db, protocols))
            {
                Console.WriteLine($"Bindings defined using a wildcard");
                return;
            }

            missingBindings = BindingHelper.GetMissingBindings(site, db, protocols, true);
        }
        catch (InternalException intex)
        {
            Console.Error.WriteLine(intex.Message);
            return;
        }

        ListBindings(missingBindings);
    }

    private static void ListBindings(List<BindingHelper.MissingBindingInfo> missingBindings)
    {
        if (missingBindings.Count == 0)
        {
            Console.WriteLine("No missing bindings found.");
            return;
        }

        Console.WriteLine($"Found {missingBindings.Count} missing bindings:");

        for (int i = 0; i < missingBindings.Count; i++)
        {
            var item = missingBindings[i];

            Console.WriteLine($"{i + 1}. {item.CompanyName}");
            Console.WriteLine("    Protocol: " + item.Protocol);
            Console.WriteLine("    Host: " + item.Host);
        }
    }
}
