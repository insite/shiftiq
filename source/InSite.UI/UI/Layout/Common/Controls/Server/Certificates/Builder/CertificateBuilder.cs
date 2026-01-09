using System.Web;
using System.Web.SessionState;

using Newtonsoft.Json;

using Shift.Sdk.UI;

namespace InSite.Common.Web.UI.Certificates.Builder
{
    public class CertificateBuilder
    {
        public CertificateVariables Variables { get; }

        public static byte[] CreatePng(
            HttpResponse http,
            string filename,
            string imagePath,
            CertificateBaseElement[] elements,
            CertificateVariables variables = null,
            int? maxWidth = null, int? maxHeight = null)
        {
            var request = new CertificateRequest
            {
                ImagePath = imagePath,
                Elements = elements,
                Variables = variables.Items,
                MaxWidth = maxWidth,
                MaxHeight = maxHeight
            };

            var json = JsonConvert.SerializeObject(request);

            return InSite.UI.Portal.Records.Certificates.Certificate.CreateImage(json);
        }

        public static void DownloadPng(
            HttpSessionState session,
            string imagePath,
            CertificateBaseElement[] elements,
            string fileName = null,
            CertificateVariables variables = null,
            int? maxWidth = null, int? maxHeight = null)
        {
            var request = new CertificateRequest
            {
                FileType = "png",
                FileName = fileName,
                ImagePath = imagePath,
                Elements = elements,
                Variables = variables.Items,
                MaxWidth = maxWidth,
                MaxHeight = maxHeight
            };

            var json = JsonConvert.SerializeObject(request);

            CurrentSessionState.AchievementCertificateRequest = json;

            HttpResponseHelper.Redirect($"~/UI/Portal/Records/Certificates/Certificate.ashx");
        }

        public static void DownloadPdf(
            HttpSessionState session,
            string imagePath,
            CertificateBaseElement[] elements,
            CertificateVariables variables = null)
        {
            var request = new CertificateRequest
            {
                FileType = "pdf",
                ImagePath = imagePath,
                Elements = elements,
                Variables = variables.Items
            };

            var json = JsonConvert.SerializeObject(request);

            CurrentSessionState.AchievementCertificateRequest = json;

            HttpResponseHelper.Redirect($"~/UI/Portal/Records/Certificates/Certificate.ashx");
        }
    }
}