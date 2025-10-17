using Microsoft.Web.Administration;

namespace Shift.Hub.Integration.IIS;

public class AddBinding
{
    public void Execute(string? siteName, string? ipAddress, string? host, string? certificateStoreName, string? certificateHash)
    {
        if (string.IsNullOrEmpty(siteName))
        {
            Console.Error.WriteLine("SiteName is a required input parameter for this command.");
            return;
        }

        if (string.IsNullOrEmpty(ipAddress))
        {
            Console.Error.WriteLine("ipAddress is a required input parameter for this command.");
            return;
        }

        if (string.IsNullOrEmpty(host))
        {
            Console.Error.WriteLine("host is a required input parameter for this command.");
            return;
        }

        if (string.IsNullOrEmpty(certificateStoreName))
        {
            Console.Error.WriteLine("certificateStoreName is a required input parameter for this command.");
            return;
        }

        if (string.IsNullOrEmpty(certificateHash))
        {
            Console.Error.WriteLine("certificateHash is a required input parameter for this command.");
            return;
        }

        var certificateHashBytes = Convert.FromHexString(certificateHash);
        var bindingInformation = $"{ipAddress}:443:{host}";

        Console.WriteLine($"Adding a new binding '{bindingInformation}' for website {siteName}...");

        var serverManager = new ServerManager();
        var site = serverManager.Sites[siteName];

        if (site == null)
        {
            Console.Error.WriteLine($"Website is not found: {siteName}");
            return;
        }

        if (site.Bindings.Any(x => string.Equals(x.Host, host, StringComparison.OrdinalIgnoreCase) && x.EndPoint.Port == 443))
        {
            Console.Error.WriteLine($"Binding already exists");
            return;
        }

        var binding = site.Bindings.Add(bindingInformation, certificateHashBytes, certificateStoreName);

        binding.SslFlags = SslFlags.Sni;

        serverManager.CommitChanges();

        Console.WriteLine($"Binding has been created");
    }
}
