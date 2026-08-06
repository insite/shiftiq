using System;

using InSite.Common.Web;
using InSite.Common.Web.Infrastructure;
using InSite.Common.Web.UI.Certificates;
using InSite.Persistence;
using InSite.UI.Layout.Portal;
using InSite.UI.Portal.Records.Credentials.Utilities;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Records.Credentials
{
    public partial class Certificate : PortalBasePage
    {
        private class CertificateException : Exception
        {
            public CertificateException(string message)
                : base(message)
            {
            }
        }

        public string Achievement => Request["achievement"];

        public string Credential => Request["credential"];

        public string EntityID => Request["id"];

        public string Platform => Request["platform"];

        public string UserID => Request["user"];

        public string Type => Request["type"];
        public string CourseName => Request["course-name"];

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            BaseCertificate certificate;

            try
            {
                certificate = LoadCertificate();
            }
            catch (CertificateException cex)
            {
                ErrorAlert.AddMessage(AlertType.Error, cex.Message);
                return;
            }
            catch (ApplicationError apperr)
            {
                ErrorAlert.AddMessage(AlertType.Error, apperr.Message);
                return;
            }

            if (certificate == null)
            {
                ErrorAlert.AddMessage(AlertType.Error, "The certificate is not configured for current resource.");
                return;
            }

            if (!certificate.IsValid())
            {
                ErrorAlert.AddMessage(AlertType.Error,
                    (certificate.Variables["User.Name"] ?? "The user") +
                    " has not successfully completed the requirements for this certificate.");
                return;
            }

            if (string.Equals(Type, "html", StringComparison.OrdinalIgnoreCase))
            {
                LoadHtml(certificate);
                return;
            }

            var organizationCertificateFileNameTemplate = Organization.Toolkits?.Achievements?.CertificateFileNameTemplate;
            string filename;

            if (!string.IsNullOrWhiteSpace(organizationCertificateFileNameTemplate))
            {
                filename = organizationCertificateFileNameTemplate;

                if (filename.Contains("{course-name}", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(CourseName))
                    filename = ReplaceCaseInsensitive(filename, "{course-name}", CourseName);

                filename = FileHelper.AdjustFileName(filename, true, false);
            }
            else
            {
                var code = certificate.Variables["Certificate.Code"];
                filename = !string.IsNullOrEmpty(code)
                    ? FileHelper.AdjustFileName(code, true, false) + "_certificate"
                    : "certificate";
            }

            if (string.IsNullOrEmpty(Type) || string.Equals(Type, "png", StringComparison.OrdinalIgnoreCase))
            {
                certificate.DownloadPng(Session, filename + ".png");
            }
            else if (string.Equals(Type, "pdf", StringComparison.OrdinalIgnoreCase))
            {
                certificate.DownloadPdf(Session, filename + ".pdf");
            }
            else
            {
                ErrorAlert.AddMessage(AlertType.Error, "Invalid certificate type.");
            }
        }

        private void LoadHtml(BaseCertificate certificate)
        {
            CertificatePanel.Visible = true;

            var imageData = certificate.CreatePng(Response, Guid.NewGuid().ToString(), 1000, 1000);

            CertificateImage.Text = $"<img alt=\"\" src=\"data:image/png;base64,{Convert.ToBase64String(imageData)}\" />";

            var imageUrl = HttpRequestHelper.GetCurrentWebUrl();
            imageUrl.QueryString.Remove("type");

            DownloadCard.NavigateUrl = imageUrl.ToString();

            var pdfUrl = imageUrl.Copy();
            pdfUrl.QueryString["type"] = "pdf";

            DownloadPDF.NavigateUrl = pdfUrl.ToString();
        }

        private BaseCertificate LoadCertificate()
        {
            if (string.IsNullOrEmpty(UserID) || !Guid.TryParse(UserID, out var userId))
                userId = User.UserIdentifier;

            if (Guid.TryParse(Achievement, out var achievementIdentifier))
            {
                var credential = ServiceLocator.AchievementSearch.GetCredential(achievementIdentifier, userId)
                    ?? throw new CertificateException("Credential Not Found");

                if (credential.AchievementCertificateLayoutCode.IsEmpty())
                    throw new CertificateException(
                        $"The credential for this Achievement ({credential.AchievementTitle}) is not specified.");

                return CertificateHelper.TryCreateCredential(credential, User.TimeZone)
                    ?? throw new CertificateException("The certificate is not configured for current resource.");
            }
            else if (Guid.TryParse(Credential, out Guid credentialId))
            {
                var credential = ServiceLocator.AchievementSearch.GetCredential(credentialId)
                    ?? throw new CertificateException($"Invalid Credential Identifier: {Credential}");

                return CertificateHelper.TryCreateCredential(credential, User.TimeZone, "Keyera-CMDS")
                    ?? throw new CertificateException("The certificate is not configured for current resource.");
            }
            else
            {
                var entity = TCertificateLayoutSearch.Select(EntityID, Organization.OrganizationIdentifier)
                    ?? throw new CertificateException("Invalid certificate code.");

                var certificate = CertificateHelper.CreateCertificate(entity);

                var user = UserSearch.Select(userId)
                    ?? throw new CertificateException("Invalid user identifier");

                certificate.Variables["User.ID"] = user.UserIdentifier.ToString();
                certificate.Variables["User.Name"] = user.FullName;

                return certificate;
            }
        }

        private static string ReplaceCaseInsensitive(string source, string search, string replacement)
        {
            return System.Text.RegularExpressions.Regex.Replace(
                source,
                System.Text.RegularExpressions.Regex.Escape(search),
                replacement.Replace("$", "$$"),
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );
        }
    }
}
