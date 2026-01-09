using System;
using System.Text.Encodings.Web;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

using InSite.Common.Web;

using Shift.Common;

namespace InSite.UI.Lobby
{
    public partial class CeritifcateVerify : Page
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var brand = ServiceLocator.AppSettings.Release.Brand;

            var cid = Request.QueryString["cid"];

            if (!string.IsNullOrEmpty(cid))
            {
                if (Guid.TryParse(cid, out var id))
                {
                    var x = ServiceLocator.AchievementSearch.GetCredential(id);
                    if (!(x is null))
                    {
                        Page.Title = $"{brand}: {x.UserFullName}'s certificate of completion for {x.AchievementTitle}.";
                        var ogtitle = new HtmlMeta();
                        ogtitle.Attributes.Add("property", "og:title");
                        ogtitle.Content = Page.Title;
                        Header.Controls.Add(ogtitle);
                        var ogdesc = new HtmlMeta();
                        ogdesc.Attributes.Add("property", "og:description");
                        ogdesc.Content = $"{x.UserFullName} has successfully finished {x.AchievementTitle} on {brand}.";
                        Header.Controls.Add(ogdesc);
                        var ogurl = new HtmlMeta();
                        ogurl.Attributes.Add("property", "og:url");
                        ogurl.Content = Request.Url.ToString();
                        Header.Controls.Add(ogurl);
                        var ogimage = new HtmlMeta();
                        ogimage.Attributes.Add("property", "og:image");
                        ogimage.Content = $"{Request.Url.Authority}/UI/Layout/Common/Parts/img/cert2.jpg";
                        Header.Controls.Add(ogimage);

                        name.InnerText = x.UserFullName;
                        name2.InnerText = x.UserFullName;
                        name3.InnerText = x.UserFullName;
                        coursTitle.InnerText = x.AchievementTitle;
                        course2.InnerText = string.Equals(x.AchievementTitle, x.AchievementLabel) ? x.AchievementTitle : $"{x.AchievementTitle}:{x.AchievementLabel}";
                        course3.InnerText = x.AchievementTitle;
                        if (!string.Equals(x.AchievementTitle, x.AchievementLabel))
                        {
                            course4.InnerText = x.AchievementLabel;
                        }
                        else course4.Visible = false;
                        ccid.InnerText = $"certificate Identifier: {cid}";
                        if (!(x.CredentialHours is null))
                        {
                            course5.InnerText = $"Total Hours:{x.CredentialHours ?? 0}";
                        }
                        else
                        {
                            course5.Visible = false;
                        }
                        if (!(x.CredentialGrantedScore is null))
                        {
                            course6.InnerText = $"Credential Score:{x.CredentialHours ?? 0}";
                        }
                        else
                        {
                            course6.Visible = false;
                        }
                        fingerPrint.Value = x.CertificateFingerPrint;
                        datetime.InnerText = x.CredentialGranted.ToDateString();
                        date.InnerText = x.CredentialGranted.ToDateString();
                        if (x.CredentialExpirationExpected.HasValue)
                        {
                            exp.InnerText = x.CredentialExpirationExpected.ToDateString();
                            exdatetime.InnerText = x.CredentialExpirationExpected.ToDateString();
                        }
                        else
                        {
                            ExpSentence.Visible = false;
                            certificateData.Attributes["class"] = "certificateContainer certificateImageC2";
                        }
                        if (CurrentSessionState.Identity.IsAuthenticated)
                        {
                            if (CurrentSessionState.Identity.User.UserIdentifier == x.UserIdentifier)
                            {
                                ControlPannel.Visible = true;
                                FaceBook.NavigateUrl = GenerateFaceBookLink(id, Request);
                                Twitter.NavigateUrl = GenerateTwitterLink(id, x.AchievementTitle, Request);
                                LinkedIn.NavigateUrl = GenerateLinkedInLink(id, Request);
                                Mail.NavigateUrl = GenerateEmailContent(brand, id, x.AchievementTitle, Request);
                            }
                            else
                            {
                                ControlPannel.Visible = false;
                            }
                        }
                        else
                        {
                            ControlPannel.Visible = false;
                        }
                        return;
                    }
                }
            }

            HttpResponseHelper.SendHttp404();
        }

        public static string GenerateVerificationLink(Guid certificateId, HttpRequest request, bool encode = true)
        {
            if (encode) return UrlEncoder.Default.Encode($"https://{request.Url.Authority}/ui/lobby/certificate?cid={certificateId}");
            else return $"https://{request.Url.Authority}/ui/lobby/certificate?cid={certificateId}";
        }

        public static string GenerateTwitterLink(Guid certificateId, string courseName, HttpRequest request)
        {
            return $"https://twitter.com/intent/tweet?text={UrlEncoder.Default.Encode($"I have received a new certificate for finishing the \"{(courseName.Length > 30 ? courseName.Substring(0, 25) + "..." : courseName)}\" on @shiftiq")} {GenerateVerificationLink(certificateId, request)}";
        }

        public static string GenerateLinkedInLink(Guid certificateId, HttpRequest request)
        {
            return $"https://www.linkedin.com/sharing/share-offsite?url={GenerateVerificationLink(certificateId, request, true)}";
        }

        public static string GenerateFaceBookLink(Guid certificateId, HttpRequest request)
        {
            return $"https://www.facebook.com/dialog/feed?app_id=853724959053613&display=popup&link={GenerateVerificationLink(certificateId, request)}&redirect_uri=https%3A%2F%2Fwww.facebook.com&hashtag={UrlEncoder.Default.Encode("#")}ShiftiQ";
        }

        public static string GenerateEmailContent(string brand, Guid certificateId, string courseName, HttpRequest request)
        {
            return $"mailto:?Subject=I have successfully completed {courseName} on {brand}!&body=I have received a new certificate for finishing the \"{(courseName.Length > 30 ? courseName.Substring(0, 25) + "..." : courseName)}\" on {brand}.%0D%0AView my certificate here: {GenerateVerificationLink(certificateId, request)}";
        }
    }
}