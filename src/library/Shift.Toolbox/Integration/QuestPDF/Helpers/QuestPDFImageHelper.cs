using System.IO;

using QuestPDF.Infrastructure;

namespace Shift.Toolbox
{
    public static class QuestPDFImageHelper
    {

        private const string LogosPlaceholderUrl = "/Library/Images/Logos";

        public static string GetOrganizationLogoUrl(string organizationCode)
        {
            if (string.IsNullOrEmpty(organizationCode))
                return null;

            return $"{LogosPlaceholderUrl}/{organizationCode}.png";
        }

        public static Image GetLogo(string logoPath)
        {
            using (var stream = new FileStream(logoPath, FileMode.Open))
            {
                stream.Position = 0;
                return Image.FromStream(stream);
            }
        }
    }
}
