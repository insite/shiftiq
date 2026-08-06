using System;
using System.Text.Encodings.Web;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI.Certificates;
using InSite.UI.Portal.Records.Credentials.Utilities;

using Shift.Common;

namespace InSite.UI.Lobby
{
    public partial class CeritifcateVerify : Page
    {
        private BaseCertificate _certificate;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Guid.TryParse(Request.QueryString["cid"], out var credentialId))
                HttpResponseHelper.SendHttp404();

            var credential = ServiceLocator.AchievementSearch.GetCredential(credentialId);
            if (credential == null || credential.UserTimeZone == null)
                HttpResponseHelper.SendHttp404();

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(credential.UserTimeZone.IfNullOrEmpty("UTC"));

            _certificate = CertificateHelper.TryCreateCredential(credential, timeZone);

            if (TryHandleFormat(credentialId, Request.QueryString["format"]))
                return;

            var brand = ServiceLocator.AppSettings.Partition.Brand;
            var ogImageUrl = _certificate == null
                ? $"{Request.Url.Authority}/UI/Layout/Common/Parts/img/cert2.jpg"
                : $"https://{Request.Url.Authority}/ui/lobby/certificate?cid={credentialId}&format=image";

            SetOpenGraphTags(brand, credential, ogImageUrl);
            SetSidebarValues(credential);
            RenderCertificate(credential);
            SetupSharePanel(credential, brand);
        }

        private bool TryHandleFormat(Guid credentialId, string format)
        {
            if (format.IsEmpty())
                return false;

            if (!string.Equals(format, "image", StringComparison.OrdinalIgnoreCase))
                HttpResponseHelper.SendHttp404();

            if (_certificate == null)
                HttpResponseHelper.SendHttp404();

            var png = _certificate.CreatePng(Response, Guid.NewGuid().ToString(), 1000, 1000);
            Response.Clear();
            Response.ContentType = "image/png";
            Response.BinaryWrite(png);
            Response.End();
            return true;
        }

        private void SetOpenGraphTags(string brand, VCredential credential, string ogImageUrl)
        {
            Page.Title = $"{brand}: {credential.UserFullName}'s certificate of completion for {credential.AchievementTitle}.";

            AddMeta("og:title", Page.Title);
            AddMeta("og:description", $"{credential.UserFullName} has successfully finished {credential.AchievementTitle} on {brand}.");
            AddMeta("og:url", Request.Url.ToString());
            AddMeta("og:image", ogImageUrl);
        }

        private void AddMeta(string property, string content)
        {
            var meta = new HtmlMeta();
            meta.Attributes.Add("property", property);
            meta.Content = content;
            Header.Controls.Add(meta);
        }

        private void RenderCertificate(VCredential credential)
        {
            if (_certificate == null)
            {
                name.InnerText = credential.UserFullName;
                coursTitle.InnerText = credential.AchievementTitle;
                ccid.InnerText = $"certificate Identifier: {credential.CredentialIdentifier}";
                datetime.InnerText = credential.CredentialGranted.ToDateString();

                if (credential.CredentialExpirationExpected.HasValue)
                    exdatetime.InnerText = credential.CredentialExpirationExpected.ToDateString();
                else
                    certificateData.Attributes["class"] = "certificateContainer certificateImageC2";
            }
            else
            {
                var png = _certificate.CreatePng(Response, Guid.NewGuid().ToString(), 1000, 1000);
                customCertificateImage.Text =
                    $"<img alt=\"\" style=\"max-width:900px;width:100%;height:auto;\" " +
                    $"src=\"data:image/png;base64,{Convert.ToBase64String(png)}\" />";
                customCertificateImage.Visible = true;
                certificateData.Visible = false;
            }
        }

        private void SetSidebarValues(VCredential credential)
        {
            name2.InnerText = credential.UserFullName;
            name3.InnerText = credential.UserFullName;

            course2.InnerText = string.Equals(credential.AchievementTitle, credential.AchievementLabel)
                ? credential.AchievementTitle
                : $"{credential.AchievementTitle}:{credential.AchievementLabel}";
            course3.InnerText = credential.AchievementTitle;

            if (!string.Equals(credential.AchievementTitle, credential.AchievementLabel))
                course4.InnerText = credential.AchievementLabel;
            else
                course4.Visible = false;

            if (credential.CredentialHours == null)
                course5.Visible = false;
            else
                course5.InnerText = $"Total Hours:{credential.CredentialHours ?? 0}";

            if (credential.CredentialGrantedScore == null)
                course6.Visible = false;
            else
                course6.InnerText = $"Credential Score:{credential.CredentialGrantedScore ?? 0}";

            fingerPrint.Value = credential.CertificateFingerPrint;
            date.InnerText = credential.CredentialGranted.ToDateString();

            if (credential.CredentialExpirationExpected.HasValue)
                exp.InnerText = credential.CredentialExpirationExpected.ToDateString();
            else
                ExpSentence.Visible = false;
        }

        private void SetupSharePanel(VCredential credential, string brand)
        {
            var isOwner = CurrentSessionState.Identity.IsAuthenticated
                       && CurrentSessionState.Identity.User.UserIdentifier == credential.UserIdentifier;

            SharePanel.Visible = isOwner;

            if (!isOwner)
                return;

            FaceBook.NavigateUrl = GetFacebookLink(credential.CredentialIdentifier, Request);
            Twitter.NavigateUrl = GetTwitterLink(credential.CredentialIdentifier, credential.AchievementTitle, Request);
            LinkedIn.NavigateUrl = GetLinkedInLink(credential.CredentialIdentifier, Request);
            Mail.NavigateUrl = GetEmailContent(brand, credential.CredentialIdentifier, credential.AchievementTitle, Request);
        }

        public static string GetVerificationLink(Guid certificateId, HttpRequest request, bool encode = true)
        {
            return encode
                ? UrlEncoder.Default.Encode($"https://{request.Url.Authority}/ui/lobby/certificate?cid={certificateId}")
                : $"https://{request.Url.Authority}/ui/lobby/certificate?cid={certificateId}";
        }

        public static string GetTwitterLink(Guid certificateId, string courseName, HttpRequest request)
        {
            return $"https://twitter.com/intent/tweet?text={UrlEncoder.Default.Encode($"I have received a new certificate for finishing the \"{(courseName.Length > 30 ? courseName.Substring(0, 25) + "..." : courseName)}\" on @shiftiq")} {GetVerificationLink(certificateId, request)}";
        }

        public static string GetLinkedInLink(Guid certificateId, HttpRequest request)
        {
            return $"https://www.linkedin.com/sharing/share-offsite?url={GetVerificationLink(certificateId, request, true)}";
        }

        public static string GetFacebookLink(Guid certificateId, HttpRequest request)
        {
            return $"https://www.facebook.com/dialog/feed?app_id=853724959053613&display=popup&link={GetVerificationLink(certificateId, request)}&redirect_uri=https%3A%2F%2Fwww.facebook.com&hashtag={UrlEncoder.Default.Encode("#")}ShiftiQ";
        }

        public static string GetEmailContent(string brand, Guid certificateId, string courseName, HttpRequest request)
        {
            return $"mailto:?Subject=I have successfully completed {courseName} on {brand}!&body=I have received a new certificate for finishing the \"{(courseName.Length > 30 ? courseName.Substring(0, 25) + "..." : courseName)}\" on {brand}.%0D%0AView my certificate here: {GetVerificationLink(certificateId, request)}";
        }
    }
}
