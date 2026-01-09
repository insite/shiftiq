using Microsoft.Web.Administration;

namespace Shift.Hub.Integration.IIS;

public class ListBindings
{
    public void Execute(string? siteName)
    {
        if (string.IsNullOrEmpty(siteName))
        {
            Console.Error.WriteLine("SiteName is a required input parameter for this command.");
            return;
        }

        Console.WriteLine($"Listing bindings for website {siteName}...");

        var serverManager = new ServerManager();
        var site = serverManager.Sites[siteName];

        if (site == null)
        {
            Console.Error.WriteLine($"Website is not found: {siteName}");
            return;
        }

        foreach (var binding in site.Bindings)
        {
            Console.WriteLine($"- {binding.BindingInformation}");

            Console.WriteLine($"      Protocol: {binding.Protocol}");
            Console.WriteLine($"      IP Address: {binding.EndPoint.Address}");
            Console.WriteLine($"      Port: {binding.EndPoint.Port}");
            Console.WriteLine($"      Host: {binding.Host}");

            if (binding.Protocol == "https" && binding.CertificateHash != null)
            {
                Console.WriteLine($"      SslFlags: {binding.SslFlags}");
                Console.WriteLine($"      CertificateStoreName: {binding.CertificateStoreName}");
                Console.WriteLine($"      CertificateHash: {Convert.ToHexString(binding.CertificateHash).ToLower()}");
            }
        }
    }
}
