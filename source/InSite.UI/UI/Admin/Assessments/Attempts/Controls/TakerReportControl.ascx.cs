using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Admin.Assessments.Attempts.Models;
using InSite.Admin.Assets.Contents.Utilities;
using InSite.Application.Attempts.Read;
using InSite.Common;
using InSite.Domain.Organizations;
using InSite.Web.Helpers;

using PdfSharp.Pdf;

using Shift.Common;
using Shift.Constant;

using UserModel = InSite.Domain.Foundations.User;

namespace InSite.UI.Admin.Assessments.Attempts.Controls
{
    public partial class TakerReportControl : UserControl
    {
        public enum Language { English, French }

        public class AttemptItem
        {
            public string PersonCode { get; set; }
            public string FullName { get; set; }
            public string Birthdate { get; set; }
            public string ExamDate { get; set; }
            public Language Language { get; set; }

            public List<FrameworkItem> Frameworks { get; set; }
        }

        public class FrameworkItem
        {
            public string FrameworkTitle { get; set; }
            public bool IsPass { get; set; }
        }

        protected static UserModel User => CurrentSessionState.Identity.User;
        private static OrganizationState Organization => CurrentSessionState.Identity.Organization;

        private InputTranslator _translator;

        private Language _attemptLanguage;

        public static byte[] GetPdf(Page page, Guid userId, Guid[] attemptIds, Language language)
        {
            var attempts = GetAttempts(userId, attemptIds, language);
            return GetPdf(page, attempts);
        }

        public static byte[] GetPdf(Page page, List<AttemptItem> attempts)
        {
            var report = (TakerReportControl)page.LoadControl("~/UI/Admin/Assessments/Attempts/Controls/TakerReportControl.ascx");
            report.LoadReport(attempts);

            var siteContent = new StringBuilder();
            using (var stringWriter = new StringWriter(siteContent))
            {
                using (var htmlWriter = new HtmlTextWriter(stringWriter))
                    report.RenderControl(htmlWriter);
            }

            var date = DateTimeOffset.Now.FormatDateOnly(User.TimeZone);

            var settings = new HtmlConverterSettings(ServiceLocator.AppSettings.Application.WebKitHtmlToPdfExePath)
            {
                PageOrientation = PageOrientationType.Portrait,
                PageSize = PageSizeType.Letter,
                Viewport = new HtmlConverterSettings.ViewportSize(980, 1400),
                Dpi = 240,
                MarginTop = 5,
                MarginBottom = 15,

                FooterTextLeft = "High Stakes Test Taker Report",
                FooterTextCenter = date,
                FooterTextRight = "Page [page] of [topage]",
                FooterFontName = "Arial",
                FooterFontSize = 10,
                FooterSpacing = 8.1f,
            };

            var data = HtmlConverter.HtmlToPdf(siteContent.ToString(), settings);

            return PdfHelper.Process(data, ProcessPdf);
        }

        private static void ProcessPdf(PdfDocument doc)
        {
            var logoUrl = GetLogoUrl();

            var watermark = PdfHelper.LoadImageByUrl(logoUrl, greyscale: true, opacity: 0.1);
            try
            {
                if (watermark != null)
                    PdfHelper.AddWatermark(doc, watermark, PdfHelper.WatermarkPosition.Diagonal);
            }
            finally
            {
                if (watermark != null)
                    watermark.Dispose();
            }

            doc.Info.Title = LabelHelper.GetTranslation("TakerReport.Title", Shift.Common.Language.Default);
            doc.Info.Author = Organization.LegalName;
            doc.Info.CreationDate = DateTime.Now;

            var release = ServiceLocator.AppSettings.Release;
            var partition = ServiceLocator.AppSettings.Partition;
            if (partition != null && release != null)
                doc.Info.Creator = $"{partition.Brand} v{release.Version}";

            PdfHelper.SetReadOnly(doc);

            var hashBytes = EncryptionHelper.ComputeHashSha256(UniqueIdentifier.Create().ToString());
            var hash = StringHelper.ByteArrayToHex(hashBytes);

            doc.SecuritySettings.OwnerPassword = hash;
        }

        private void LoadReport(List<AttemptItem> attempts)
        {
            _translator = new InputTranslator("en", Organization.Identifier);

            AttemptRepeater.ItemDataBound += AttemptRepeater_ItemDataBound;
            AttemptRepeater.DataSource = attempts;
            AttemptRepeater.DataBind();
        }

        private void AttemptRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var logo = (HtmlImage)e.Item.FindControl("Logo");
            logo.Src = GetLogoUrl();

            var attempt = (AttemptItem)e.Item.DataItem;

            var noDataRow = e.Item.FindControl("NoDataRow");
            noDataRow.Visible = attempt.Frameworks.Count == 0;

            _attemptLanguage = attempt.Language;

            var frameworkRepeater = (Repeater)e.Item.FindControl("FrameworkRepeater");
            frameworkRepeater.DataSource = attempt.Frameworks;
            frameworkRepeater.DataBind();
        }

        private static string GetLogoUrl()
        {
            var request = HttpContext.Current.Request;
            var serverUrl = $"{request.Url.Scheme}://{request.Url.Host}";
            var logo = CurrentSessionState.Identity.Organization.PlatformCustomization.PlatformUrl.Logo;

            // The logo is open text: a site-relative path in some organizations, a fully-qualified
            // URL in others. GetAbsoluteUrl handles both.
            return UrlHelper.GetAbsoluteUrl(serverUrl, "/", logo);
        }

        private static List<AttemptItem> GetAttempts(Guid userId, Guid[] attemptIds, Language language)
        {
            var person = ServiceLocator.PersonSearch.GetPerson(userId, Organization.Identifier, x => x.User);
            var attempts = ServiceLocator.AttemptSearch.GetAttempts(new QAttemptFilter { AttemptIdentifiers = attemptIds }, x => x.Form);
            var result = new List<AttemptItem>();

            var languageCode = language == Language.English ? "en" : "fr";
            var culture = CultureInfo.GetCultureInfo(languageCode);

            var translator = new InputTranslator(languageCode, Organization.Identifier);
            var na = translator.Translate("N/A");

            foreach (var attempt in attempts)
            {
                if (attempt.Form == null)
                    continue;

                var bank = ServiceLocator.BankSearch.GetBankState(attempt.Form.BankIdentifier);
                var form = bank.FindForm(attempt.FormIdentifier);

                if (form == null)
                    continue;

                var item = new AttemptItem
                {
                    PersonCode = person.PersonCode,
                    FullName = person.User.FullName,
                    Language = language,
                    Birthdate = person.Birthdate.HasValue
                        ? TimeZones.FormatDateOnly(person.Birthdate.Value, User.TimeZone, culture, "{0:MMMM d, yyyy}")
                        : na,
                    ExamDate = attempt.AttemptStarted.HasValue
                        ? TimeZones.FormatDateOnly(attempt.AttemptStarted.Value, User.TimeZone, culture, "{0:MMMM d, yyyy}")
                        : na,

                    Frameworks = GetFrameworks(attempt.AttemptIdentifier, languageCode)
                };

                result.Add(item);
            }

            return result;
        }

        private static List<FrameworkItem> GetFrameworks(Guid attemptId, string language)
        {
            var result = new List<FrameworkItem>();

            var settings = new AttemptAnalysis.Settings(ServiceLocator.AttemptSearch, ServiceLocator.BankSearch);
            settings.Filter = new QAttemptFilter { AttemptIdentifier = attemptId };

            var analysis = AttemptAnalysis.Create(settings);
            if (!analysis.HasData)
                return result;

            var summary = StandardSummary.GetData(analysis, false, language);

            foreach (var occupation in summary.OrderBy(x => x.Sequence).ThenBy(x => x.Name))
            {
                foreach (var framework in occupation.Frameworks.OrderBy(x => x.Sequence).ThenBy(x => x.FrameworkTitle))
                {
                    if (framework.ID == Guid.Empty)
                        continue;

                    var item = new FrameworkItem
                    {
                        FrameworkTitle = framework.FrameworkTitle,
                        IsPass = framework.Score >= framework.PassingScore
                    };

                    result.Add(item);
                }
            }

            return result;
        }

        protected string GetLanguageName()
        {
            var attempt = (AttemptItem)Page.GetDataItem();
            return attempt.Language == Language.English
                ? Translate("English")
                : Translate("French");
        }

        protected string GetPassOrFail()
        {
            var framework = (FrameworkItem)Page.GetDataItem();
            var text = framework.IsPass ? "Pass" : "Fail";
            return _translator.Translate(text, _attemptLanguage == Language.English ? "en" : "fr", Organization.Identifier);
        }

        protected string Translate(string text)
        {
            var attempt = (AttemptItem)Page.GetDataItem();
            return _translator.Translate(text, attempt.Language == Language.English ? "en" : "fr", Organization.Identifier);
        }

        protected string GetAddress()
        {
            var location = CurrentSessionState.Identity.Organization.PlatformCustomization?.TenantLocation;
            if (location == null)
                return string.Empty;

            var address = new StringBuilder();
            address.Append(location.Street);
            address.Append("<br/>");
            address.Append(location.City);
            address.Append(" ");
            address.Append(location.Province);
            address.Append(" ");
            address.Append(location.PostalCode);
            address.Append("<br/>");
            address.Append("Email: ");
            address.Append(location.Email);

            return address.ToString();
        }
    }
}
