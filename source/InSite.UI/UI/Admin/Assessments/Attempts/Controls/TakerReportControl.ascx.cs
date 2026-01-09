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
using InSite.Application.Attempts.Read;
using InSite.Common;
using InSite.Web.Helpers;

using Shift.Common;
using Shift.Constant;

using UserModel = InSite.Domain.Foundations.User;

namespace InSite.UI.Admin.Assessments.Attempts.Controls
{
    public partial class TakerReportControl : UserControl
    {
        public enum Language { English, French }

        private class AttemptItem
        {
            public string PersonCode { get; set; }
            public string FullName { get; set; }
            public string Birthdate { get; set; }
            public string ExamTitle { get; set; }
            public string ExamDate { get; set; }

            public List<FrameworkItem> Frameworks { get; set; }
        }

        private class FrameworkItem
        {
            public string FrameworkTitle { get; set; }
            public string PassOrFail { get; set; }
        }

        private Language CurrentLanguage
        {
            get => (Language)ViewState[nameof(CurrentLanguage)];
            set => ViewState[nameof(CurrentLanguage)] = value;
        }

        protected string CurrentLanguageName
        {
            get
            {
                return CurrentLanguage == Language.English
                    ? Translate("English")
                    : Translate("French");
            }
        }

        protected static UserModel User => CurrentSessionState.Identity.User;

        public static byte[] GetPdf(Page page, Guid userId, Guid organizationId, Guid[] attemptIds, Language language)
        {
            var report = (TakerReportControl)page.LoadControl("~/UI/Admin/Assessments/Attempts/Controls/TakerReportControl.ascx");
            report.LoadReport(userId, organizationId, attemptIds, language);

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
                Viewport = new HtmlConverterSettings.ViewportSize(980, 1400),
                Dpi = 240,
                MarginTop = 5,
                MarginBottom = 15,

                FooterTextLeft = "High Stakes Test Taker Report",
                FooterTextCenter = date,
                FooterTextRight = "Page [page] of [topage]",
                FooterFontName = "Calibri",
                FooterFontSize = 10,
                FooterSpacing = 8.1f,
            };

            var data = HtmlConverter.HtmlToPdf(siteContent.ToString(), settings);

            return PdfHelper.Process(data, doc =>
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

                var organization = CurrentSessionState.Identity.Organization;
                doc.Info.Title = LabelHelper.GetTranslation("TakerReport.Title", Shift.Common.Language.Default);
                doc.Info.Author = organization.LegalName;
                doc.Info.CreationDate = DateTime.Now;

                var appRelease = ServiceLocator.AppSettings.Release;
                if (appRelease != null)
                    doc.Info.Creator = $"{appRelease.Brand} v{appRelease.Version}";

                PdfHelper.SetReadOnly(doc);

                doc.SecuritySettings.OwnerPassword = "9v![EDs8U|o*Uw.o\"+ibK!~}\\V*ec-Y8COc/mg|W3X7?^@+)~7";
            });
        }

        protected string Translate(string text) => LabelHelper.GetTranslation(text, CurrentLanguage == Language.English ? "en" : "fr");

        private void LoadReport(Guid userId, Guid organizationId, Guid[] attemptIds, Language language)
        {
            CurrentLanguage = language;

            var attempts = GetAttempts(userId, organizationId, attemptIds);

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

            var frameworkRepeater = (Repeater)e.Item.FindControl("FrameworkRepeater");
            frameworkRepeater.DataSource = attempt.Frameworks;
            frameworkRepeater.DataBind();
        }

        private static string GetLogoUrl()
        {
            var request = HttpContext.Current.Request;
            return $"{request.Url.Scheme}://{request.Url.Host}{CurrentSessionState.Identity.Organization.PlatformCustomization.PlatformUrl.Logo}";
        }

        private List<AttemptItem> GetAttempts(Guid userId, Guid organizationId, Guid[] attemptIds)
        {
            var person = ServiceLocator.PersonSearch.GetPerson(userId, organizationId, x => x.User);
            var attempts = ServiceLocator.AttemptSearch.GetAttempts(new QAttemptFilter { AttemptIdentifiers = attemptIds }, x => x.Form);
            var result = new List<AttemptItem>();

            var language = CurrentLanguage == Language.English ? "en" : "fr";
            var culture = CultureInfo.GetCultureInfo(language);

            foreach (var attempt in attempts)
            {
                if (attempt.Form == null)
                    continue;

                var bank = ServiceLocator.BankSearch.GetBankState(attempt.Form.BankIdentifier);
                var form = bank.FindForm(attempt.FormIdentifier);

                if (form == null)
                    continue;

                var examTitle = form.Content?.Title != null
                    ? (form.Content.Title.Get(language) ?? form.Content.Title.Default)
                    : attempt.Form.FormTitle;

                var item = new AttemptItem
                {
                    PersonCode = person.PersonCode,
                    FullName = person.User.FullName,
                    Birthdate = person.Birthdate.HasValue
                        ? TimeZones.FormatDateOnly(person.Birthdate.Value, User.TimeZone, culture, "{0:MMMM d, yyyy}")
                        : Translate("N/A"),
                    ExamTitle = examTitle,
                    ExamDate = attempt.AttemptStarted.HasValue
                        ? TimeZones.FormatDateOnly(attempt.AttemptStarted.Value, User.TimeZone, culture, "{0:MMMM d, yyyy}")
                        : Translate("N/A"),
                };

                item.Frameworks = GetFrameworks(attempt.AttemptIdentifier, language);

                result.Add(item);
            }

            return result;
        }

        private List<FrameworkItem> GetFrameworks(Guid attemptId, string language)
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
                        PassOrFail = framework.Score >= framework.PassingScore ? Translate("Pass") : Translate("Fail")
                    };

                    result.Add(item);
                }
            }

            return result;
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