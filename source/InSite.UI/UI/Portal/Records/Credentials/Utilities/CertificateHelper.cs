using System;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI.Certificates;
using InSite.Persistence;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.UI.Portal.Records.Credentials.Utilities
{
    public static class CertificateHelper
    {
        public static BaseCertificate TryCreateCredential(VCredential credential, TimeZoneInfo tz, string defaultLayoutCode = null)
        {
            var code = credential.AchievementCertificateLayoutCode.IfNullOrEmpty(defaultLayoutCode);
            if (code.IsEmpty())
                return null;

            var layout = TCertificateLayoutSearch.Select(code);
            if (layout == null)
                return null;

            var cert = JsonConvert.DeserializeObject<BaseCertificate>(layout.CertificateLayoutData);

            cert.Variables["Certificate.TenantIdentifier"] = layout.OrganizationIdentifier.ToString();
            cert.Variables["Certificate.Code"] = layout.CertificateLayoutCode;
            cert.Variables["Certificate.Title"] = cert.Title;
            cert.Variables["Asset.Title"] = credential.AchievementTitle;
            cert.Variables["Assignment.CompletedOn"] = credential.CredentialGranted.FormatDateOnly(tz);
            cert.Variables["Assignment.ExpiresOn"] = credential.CredentialExpirationExpected.FormatDateOnly(tz);
            cert.Variables["Assignment.IsPassing"] = (credential.CredentialStatus == "Valid").ToString();
            cert.Variables["User.ID"] = credential.UserIdentifier.ToString();
            cert.Variables["User.Name"] = credential.UserFullName;

            return cert;
        }

        public static BaseCertificate CreateCertificate(TCertificateLayout entity)
        {
            if (entity == null)
                throw new ApplicationError("The certificate is not configured for current resource.");

            BaseCertificate result;

            try
            {
                result = JsonConvert.DeserializeObject<BaseCertificate>(entity.CertificateLayoutData);
            }
            catch (Exception ex)
            {
                throw new ApplicationError("The certificate has incorrect layout description: " + ex.Message);
            }

            result.Variables["Certificate.TenantIdentifier"] = entity.OrganizationIdentifier.ToString();
            result.Variables["Certificate.Code"] = entity.CertificateLayoutCode;
            result.Variables["Certificate.Title"] = result.Title;

            return result;
        }
    }
}
