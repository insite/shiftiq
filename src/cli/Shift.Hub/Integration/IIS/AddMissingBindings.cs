using Microsoft.Web.Administration;

namespace Shift.Hub.Integration.IIS
{
    public class AddMissingBindings
    {
        public void Execute(string? connectionString, string? siteName, string? protocolsText, string? ipAddress, string? certificateStoreName, string? certificateHash)
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

            if (string.IsNullOrEmpty(ipAddress))
            {
                Console.Error.WriteLine("ipAddress is a required input parameter for this command.");
                return;
            }

            if (protocols.Contains("https"))
            {
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
                missingBindings = BindingHelper.GetMissingBindings(site, db, protocols, true);
            }
            catch (InternalException intex)
            {
                Console.Error.WriteLine(intex.Message);
                return;
            }

            AddBindings(site, ipAddress, certificateStoreName, certificateHash, missingBindings);

            serverManager.CommitChanges();
        }

        private static void AddBindings(Site site, string? ipAddress, string? certificateStoreName, string? certificateHash, List<BindingHelper.MissingBindingInfo> missingBindings)
        {
            if (missingBindings.Count == 0)
            {
                Console.WriteLine("No missing bindings found.");
                return;
            }

            Console.WriteLine($"Found {missingBindings.Count} missing bindings.");

            for (int i = 0; i < missingBindings.Count; i++)
            {
                var item = missingBindings[i];
                var isHttps = item.Protocol == "https";
                var port = isHttps ? 443 : 80;

                var bindingInformation = $"{ipAddress}:{port}:{item.Host}";

                Console.WriteLine($"Adding a new binding '{bindingInformation}' for website {site.Name}...");

                if (isHttps)
                {
                    var certificateHashBytes = Convert.FromHexString(certificateHash!);
                    var binding = site.Bindings.Add(bindingInformation, certificateHashBytes, certificateStoreName);
                    binding.SslFlags = SslFlags.Sni;
                }
                else
                {
                    site.Bindings.Add(bindingInformation, item.Protocol);
                }
            }
        }
    }
}
