using System;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace InSite.UI.Admin.Records.Credentials.Utilities
{
    public static class BadgeHelper
    {
        public static readonly string BadgeVerifyUrl = "/api/openbadges/credentials";
        public static readonly string OpenbadgesNameSpaceUrl = "https://w3id.org/openbadges/v2";

        public static byte[] GetBadgeSVGFile(string badgeUrl, Guid credentialIdentifier)
        {
            if (!Uri.TryCreate(badgeUrl, UriKind.RelativeOrAbsolute, out var parsedUrl))
                return null;

            var (fileIdentifier, _) = ServiceLocator.StorageService.ParseFileUrl(parsedUrl.AbsolutePath);
            if (!fileIdentifier.HasValue)
                return null;

            var (_, stream) = ServiceLocator.StorageService.GetFileStream(fileIdentifier.Value);

            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                var svgContent = reader.ReadToEnd();

                if (svgContent == null || svgContent.Length == 0)
                    return null;

                XDocument svgDocument = XDocument.Parse(svgContent);
                var openbadgesNs_xsd = XNamespace.Get(OpenbadgesNameSpaceUrl);

                var badgeVerifyUrl = PathHelper.GetOrganizationUrl(
                    ServiceLocator.AppSettings.Environment,
                    CurrentSessionState.Identity.Organization.OrganizationCode,
                    $"{BadgeVerifyUrl}/{credentialIdentifier.ToString()}");

                XElement assertionElement = new XElement(openbadgesNs_xsd + "assertion",
                            new XAttribute("verify", badgeVerifyUrl),
                            new XAttribute(XNamespace.Xmlns + "openbadges", openbadgesNs_xsd)
                        );

                svgDocument.Root.AddFirst(assertionElement);
                var result = svgDocument.ToString();

                byte[] svgBytes;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    svgDocument.Save(memoryStream);
                    svgBytes = memoryStream.ToArray();

                    return svgBytes;
                }
            }
        }
    }
}