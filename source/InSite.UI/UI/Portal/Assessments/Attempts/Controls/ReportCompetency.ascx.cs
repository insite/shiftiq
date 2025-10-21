using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

using InSite.Admin.Assets.Contents.Utilities;
using InSite.Application.Attempts.Read;
using InSite.Application.Contacts.Read;
using InSite.Common;
using InSite.Domain.Banks;
using InSite.Persistence;
using InSite.Web.Helpers;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Assessments.Attempts.Controls
{
    public partial class ReportCompetency : UserControl
    {
        public class DataSource
        {
            public string Title => BankForm.Content.Title?.Get(Language) ?? "N/A";

            public BankState Bank { get; set; }
            public Form BankForm { get; set; }
            public QAttempt Attempt { get; set; }
            public VPerson Learner { get; set; }
            public IEnumerable<QAttemptQuestion> Questions { get; set; }

            public Guid OrganizationIdentifier { get; internal set; }
            public string LogoImageUrl { get; set; }
            public string CurrentUrl { get; set; }
            public string Language { get; set; }
            public TimeZoneInfo TimeZone { get; set; }
        }

        private class FieldInfo
        {
            public string Label { get; set; }
            public string Value { get; set; }
        }

        private class FrameworkInfo
        {
            public Guid FrameworkId { get; set; }
            public string FrameworkTitleEn { get; set; }
            public string FrameworkTitleLang { get; set; }
        }

        protected class AreaInfo
        {
            public Guid StandardId { get; set; }
            public string StandardTitle { get; set; }
            public string StandardCode { get; set; }
            public decimal AnswerPoints { get; internal set; }
            public decimal QuestionsPoints { get; internal set; }
        }

        protected AreaInfo OverallInfo => _overallInfo;

        private DataSource _data;
        private CultureInfo _culture;
        private bool _isDefaultLang;
        private AreaInfo _overallInfo;

        public static byte[] RenderPdf(DataSource data)
        {
            var culture = new CultureInfo(data.Language);

            using (var page = new Page())
            {
                page.EnableEventValidation = false;
                page.EnableViewState = false;

                var report = (ReportCompetency)page.LoadControl("~/UI/Portal/Assessments/Attempts/Controls/ReportCompetency.ascx");

                report.LoadData(data);

                var htmlBuilder = new StringBuilder();

                using (var writer = new StringWriter(htmlBuilder))
                {
                    using (var htmlWriter = new HtmlTextWriter(writer))
                        report.RenderControl(htmlWriter);
                }

                var htmlString = HtmlHelper.ResolveRelativePaths(data.CurrentUrl, htmlBuilder);

                var settings = new HtmlConverterSettings(ServiceLocator.AppSettings.Application.WebKitHtmlToPdfExePath)
                {
                    EnableSmartShrinking = false,
                    Dpi = 350,
                    PageSize = PageSizeType.Letter,
                    Viewport = new HtmlConverterSettings.ViewportSize(1280, 1652),
                    Zoom = 0.8f,

                    MarginRight = 13f,
                    MarginLeft = 13f,

                    MarginBottom = 22,
                    FooterTextLeft = data.Title,
                    FooterTextCenter = DateTimeOffset.Now.FormatDateOnly(data.TimeZone, culture: culture),
                    FooterTextRight = LabelHelper.GetTranslation("[Attempts.ReportCompetency].[Paging]", data.Language),
                    FooterFontName = "arial",
                    FooterFontSize = 10,
                    FooterSpacing = 8.1f,
                };

                return HtmlConverter.HtmlToPdf(htmlString, settings);
            }
        }

        private void LoadData(DataSource data)
        {
            _data = data;
            _isDefaultLang = data.Language.IsEmpty() || data.Language == Language.Default;
            _culture = _isDefaultLang ? null : new CultureInfo(data.Language);

            ReportTitle.InnerText = PageTitle.InnerText = data.Title;

            if (data.LogoImageUrl.IsEmpty())
                LogoContainer.InnerText = ServiceLocator.Partition.GetPlatformName();
            else
                LogoContainer.InnerHtml = $"<img alt='' src='{HttpUtility.UrlEncode(data.LogoImageUrl)}' />";

            BindFields();
            BindCompetencies();

            Footer.Text = GetLabel("[Attempts.ReportCompetency].[Footer]", true);
        }

        private void BindFields()
        {
            var framework = FindFramework(_data);
            var attemptLang = _data.Attempt.AttemptLanguage.IfNullOrEmpty(Language.Default);
            var attemptLangName = Language.GetEnglishName(attemptLang);

            if (!_isDefaultLang)
            {
                var translator = new InputTranslator(_data.OrganizationIdentifier, _data.Language);
                attemptLangName = translator.Translate(attemptLangName);
            }

            var fields = new[]
            {
                GetField("[Attempts.ReportCompetency].[UserName]", _data.Learner?.UserFullName ?? "N/A"),
                GetField("[Attempts.ReportCompetency].[PersonCode]", _data.Learner?.PersonCode),
                GetField("[Attempts.ReportCompetency].[AttemptDate]", _data.Attempt.AttemptSubmitted.FormatDateOnly(_data.TimeZone, culture: _culture)),
                GetField("[Attempts.ReportCompetency].[FrameworkName]", framework?.FrameworkTitleLang.IfNullOrEmpty(framework.FrameworkTitleEn) ?? "N/A"),
                GetField("[Attempts.ReportCompetency].[AttemptLanguage]", attemptLangName),
            };

            FieldsRepeater.DataSource = fields.Where(x => x.Value.IsNotEmpty());
            FieldsRepeater.DataBind();
        }

        private void BindCompetencies()
        {
            _overallInfo = new AreaInfo();

            var items = new Dictionary<Guid, AreaInfo>();

            foreach (var question in _data.Questions)
            {
                var bankQuestion = _data.Bank.FindQuestion(question.QuestionIdentifier);
                if (bankQuestion != null && bankQuestion.Classification.Tag.IsNotEmpty())
                {
                    var tags = bankQuestion.Classification.Tag.Split(',');
                    if (tags.Any(x => string.Equals(x.Trim(), "Pilot", StringComparison.OrdinalIgnoreCase)))
                        continue;
                }

                var areaId = question.CompetencyAreaIdentifier ?? Guid.Empty;
                var area = items.GetOrAdd(areaId, () => new AreaInfo
                {
                    StandardId = areaId,
                    StandardCode = question.CompetencyAreaCode,
                    StandardTitle = question.CompetencyAreaTitle
                });

                area.AnswerPoints += question.AnswerPoints ?? 0;
                area.QuestionsPoints += question.QuestionPoints ?? 0;

                _overallInfo.AnswerPoints += question.AnswerPoints ?? 0;
                _overallInfo.QuestionsPoints += question.QuestionPoints ?? 0;
            }

            if (!_isDefaultLang)
            {
                var areaIds = items.Keys.Where(x => x != Guid.Empty).ToArray();
                if (areaIds.IsNotEmpty())
                {
                    var standards = StandardSearch.Bind(
                        x => new
                        {
                            StandardIdentifier = x.StandardIdentifier,
                            StandardTitle = CoreFunctions.GetContentText(x.StandardIdentifier, ContentLabel.Title, _data.Language),
                        },
                        x => areaIds.Contains(x.StandardIdentifier));

                    foreach (var standard in standards)
                    {
                        if (standard.StandardTitle.IsNotEmpty())
                            items[standard.StandardIdentifier].StandardTitle = standard.StandardTitle;
                    }
                }
            }

            AreaRepeater.DataSource = items.Values.OrderBy(x => x.StandardCode).ThenBy(x => x.StandardTitle);
            AreaRepeater.DataBind();
        }

        private FieldInfo GetField(string label, string value)
        {
            return new FieldInfo
            {
                Label = GetLabel(label),
                Value = value
            };
        }

        private static FrameworkInfo FindFramework(DataSource data)
        {
            var areaIds = data.Questions
                .Where(x => x.CompetencyAreaIdentifier.HasValue)
                .Select(x => x.CompetencyAreaIdentifier.Value)
                .ToArray();

            if (areaIds.IsEmpty())
                return null;

            return StandardSearch.BindFirst(
                x => new FrameworkInfo
                {
                    FrameworkId = x.Parent.StandardIdentifier,
                    FrameworkTitleEn = CoreFunctions.GetContentTextEn(x.Parent.StandardIdentifier, ContentLabel.Title),
                    FrameworkTitleLang = CoreFunctions.GetContentText(x.Parent.StandardIdentifier, ContentLabel.Title, data.Language)
                },
                x => areaIds.Contains(x.StandardIdentifier) && x.Parent != null);
        }

        protected string GetLabel(string label, bool markdown = false)
        {
            return LabelHelper.GetTranslation(label, _data.Language, markdown);
        }

        protected string GetAreaScore(decimal points)
        {
            var value = points == 0 ? 0 : Calculator.GetPercentDecimal(points, _overallInfo.QuestionsPoints);
            return value.ToString("p0");
        }
    }
}