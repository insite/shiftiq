using System;

using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.Infrastructure;
using InSite.Common.Web.UI.Certificates;
using InSite.Persistence;
using InSite.UI.Layout.Portal;

using Newtonsoft.Json;

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

            if (certificate == null || !certificate.IsValid())
            {
                ErrorAlert.AddMessage(
                    AlertType.Error,
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
                var achievement = ServiceLocator.AchievementSearch.GetAchievement(achievementIdentifier);
                if (achievement != null)
                {
                    var credential = ServiceLocator.AchievementSearch.GetCredential(achievementIdentifier, userId);
                    if (credential != null)
                        return LoadCredentialCertificate(achievement, credential);
                    else
                        throw new CertificateException("Credential Not Found");
                }
                else
                {
                    throw new CertificateException("Credential Not Found");
                }
            }
            else if (Guid.TryParse(Credential, out Guid credential))
            {
                var c = ServiceLocator.AchievementSearch.GetCredential(credential);

                if (c != null)
                {
                    var a = ServiceLocator.AchievementSearch.GetAchievement(c.AchievementIdentifier);
                    if (a != null)
                        return LoadCmdsCertificate(a, c);
                }

                throw new CertificateException($"Invalid Credential Identifier: {Credential}");
            }
            else
            {
                var entity = TCertificateLayoutSearch.Select(EntityID, Organization.OrganizationIdentifier);
                if (entity == null)
                    throw new CertificateException("Invalid certificate code.");

                var certificate = CreateCertificate(entity);

                SetupUserVariables(userId, certificate);

                return certificate;
            }

            BaseCertificate LoadCertificate(string code)
            {
                var entity = !string.IsNullOrEmpty(code) ? TCertificateLayoutSearch.Select(code) : null;

                return CreateCertificate(entity);
            }

            BaseCertificate CreateCertificate(TCertificateLayout entity)
            {
                if (entity == null)
                    throw new CertificateException("The certificate is not configured for current resource.");

                BaseCertificate result;

                try
                {
                    result = JsonConvert.DeserializeObject<BaseCertificate>(entity.CertificateLayoutData);
                }
                catch (Exception ex)
                {
                    throw new CertificateException("The certificate has incorrect layout description.<br>" + ex.Message);
                }

                result.Variables["Certificate.TenantIdentifier"] = entity.OrganizationIdentifier.ToString();
                result.Variables["Certificate.Code"] = entity.CertificateLayoutCode;
                result.Variables["Certificate.Title"] = result.Title;
                return result;
            }

            BaseCertificate LoadCmdsCertificate(QAchievement achievement, VCredential credential)
            {
                var certificate = achievement.CertificateLayoutCode;
                if (certificate == null)
                    certificate = "Keyera-CMDS";

                var result = LoadCertificate(certificate);
                result.Variables["Asset.Title"] = achievement.AchievementTitle;
                result.Variables["Assignment.CompletedOn"] = credential.CredentialGranted.FormatDateOnly(User.TimeZone);
                result.Variables["Assignment.ExpiresOn"] = credential.CredentialExpirationExpected.FormatDateOnly(User.TimeZone);
                result.Variables["Assignment.IsPassing"] = (credential.CredentialStatus == "Valid").ToString();

                SetupUserVariables(credential.UserIdentifier, result);

                return result;
            }

            BaseCertificate LoadCredentialCertificate(QAchievement achievement, VCredential credential)
            {
                var tz = CurrentSessionState.Identity.User.TimeZone;

                var certificate = achievement.CertificateLayoutCode;
                if (certificate == null)
                    throw new CertificateException($"The credential for this Achievement ({achievement.AchievementTitle}) is not specified.");

                var result = LoadCertificate(certificate);
                result.Variables["Asset.Title"] = achievement.AchievementTitle;
                result.Variables["Assignment.CompletedOn"] = credential.CredentialGranted.FormatDateOnly(tz);
                result.Variables["Assignment.ExpiresOn"] = credential?.CredentialExpirationExpected.FormatDateOnly(tz);
                result.Variables["Assignment.IsPassing"] = (credential.CredentialStatus == "Valid").ToString();

                SetupUserVariables(credential.UserIdentifier, result);

                return result;
            }

            User SetupUserVariables(Guid uid, BaseCertificate certificate)
            {
                var entity = UserSearch.Select(uid);
                if (entity == null)
                    throw new CertificateException("Invalid user identifier");

                certificate.Variables["User.ID"] = entity.UserIdentifier.ToString();
                certificate.Variables["User.Name"] = entity.FullName;

                return entity;
            }
        }

        static DateTimeOffset? GetExpectedExpiration(Guid? achievementIdentifier, Guid userIdentifier)
        {
            var credential = achievementIdentifier.HasValue
                ? ServiceLocator.AchievementSearch.GetCredential(achievementIdentifier.Value, userIdentifier)
                : null;

            return credential?.CredentialExpirationExpected;
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