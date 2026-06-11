using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using HtmlToOpenXml;
using HtmlToOpenXml.IO;

using InSite.Admin.Assets.Contents.Utilities;
using InSite.Domain.Attempts;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;
using Shift.Toolbox;

using Document = DocumentFormat.OpenXml.Wordprocessing.Document;

namespace InSite.Admin.Assessments.Questions.Utilities
{
    public class QuestionDocxPrinter : IWebRequest
    {
        public class PrinterOptions
        {
            public Guid OrganizationId { get; set; }
            public Guid BankId { get; set; }
            public Guid? FormId { get; set; }
            public string Language { get; set; }
            public bool IncludeAdminComments { get; set; }
            public bool ExcludeHiddenComments { get; set; }
            public bool IncludeImages { get; set; }
            public TimeZoneInfo TimeZone { get; set; }
            public string AuthorName { get; set; }
            public QuestionPrintHelper.QuestionFilter QuestionFilter { get; set; }
            public string DefaultImagePath { get; set; }
            public string WebsiteUrl { get; set; }
        }

        private readonly InputTranslator _translator;
        private readonly bool _includeAdminComments;
        private readonly bool _excludeHiddenComments;
        private readonly bool _includeImages;
        private readonly TimeZoneInfo _timeZone;
        private readonly string _authorName;
        private readonly string _defaultImagePath;
        private readonly string _websiteUrl;
        private readonly IEnumerable<QuestionPrintHelper.IQuestionInfo> _questions;

        private WordprocessingDocument _word;
        private StringBuilder _html;

        private QuestionDocxPrinter(
            InputTranslator translator,
            bool includeAdminComments,
            bool excludeHiddenComments,
            bool includeImages,
            TimeZoneInfo timeZone,
            string authorName,
            string defaultImagePath,
            string websiteUrl,
            IEnumerable<QuestionPrintHelper.IQuestionInfo> questions
        )
        {
            _translator = translator;
            _includeAdminComments = includeAdminComments;
            _excludeHiddenComments = excludeHiddenComments;
            _includeImages = includeImages;
            _timeZone = timeZone;
            _authorName = authorName;
            _defaultImagePath = defaultImagePath;
            _websiteUrl = websiteUrl;
            _questions = questions;
        }

        public static PrintOutputFile RenderBankQuestions(PrinterOptions options)
        {
            var bank = ServiceLocator.BankSearch.GetBankState(options.BankId);
            if (bank == null || bank.Tenant != options.OrganizationId)
                return null;

            var title = bank.Name;
            var questions = QuestionPrintHelper.GetQuestions(bank, options.Language);
            var filteredQuestions = QuestionPrintHelper.FilterQuestions(questions, options.QuestionFilter);

            QuestionPrintHelper.InitProperties(filteredQuestions, options.OrganizationId);

            var printer = new QuestionDocxPrinter(
                new InputTranslator(options.Language, options.OrganizationId),
                options.IncludeAdminComments,
                options.ExcludeHiddenComments,
                options.IncludeImages,
                options.TimeZone,
                options.AuthorName,
                options.DefaultImagePath,
                options.WebsiteUrl,
                filteredQuestions
            );

            return printer.RenderDocx(title, bank.Asset);
        }

        public static PrintOutputFile RenderFormQuestions(PrinterOptions options)
        {
            var bank = ServiceLocator.BankSearch.GetBankState(options.BankId);
            if (bank == null || bank.Tenant != options.OrganizationId)
                return null;

            var form = bank.FindForm(options.FormId ?? throw new ArgumentNullException("options.FormId"));
            if (form == null)
                return null;

            var title = form.Name;
            var questions = QuestionPrintHelper.GetQuestions(form, options.Language);
            var filteredQuestions = QuestionPrintHelper.FilterQuestions(questions, options.QuestionFilter);

            QuestionPrintHelper.InitProperties(filteredQuestions, options.OrganizationId);

            var printer = new QuestionDocxPrinter(
                new InputTranslator(options.Language, options.OrganizationId),
                options.IncludeAdminComments,
                options.ExcludeHiddenComments,
                options.IncludeImages,
                options.TimeZone,
                options.AuthorName,
                options.DefaultImagePath,
                options.WebsiteUrl,
                filteredQuestions
            );

            return printer.RenderDocx(title, form.Asset);
        }

        private PrintOutputFile RenderDocx(string title, int assetNumber)
        {
            var fileName = $"{StringHelper.Sanitize(title, '_')}_internal_{DateTime.UtcNow:yyyyMMdd}-{DateTime.UtcNow:HHmmss}";

            using (var stream = new MemoryStream())
            {
                using (_word = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document))
                {
                    SetupDocx();

                    RenderContent(title, assetNumber);

                    _word.Save();
                }

                return new PrintOutputFile(fileName, "docx", stream.ToArray());
            }
        }

        private void SetupDocx()
        {
            _word.AddMainDocumentPart();
            var documentBody = new Body();
            var wordDocument = new Document(documentBody);
            wordDocument.Save(_word.MainDocumentPart);

            OxmlHelper.MicrosoftWordStyles.SetupDefault(_word.MainDocumentPart);

            var sectionProps = new SectionProperties(
                new PageSize
                {
                    Width = OxmlHelper.InchesToDxa<uint>(8.5),
                    Height = OxmlHelper.InchesToDxa<uint>(11),
                    Orient = PageOrientationValues.Portrait
                },
                new PageMargin
                {
                    Left = OxmlHelper.InchesToDxa<uint>(1),
                    Right = OxmlHelper.InchesToDxa<uint>(1),
                    Top = OxmlHelper.InchesToDxa<int>(1),
                    Bottom = OxmlHelper.InchesToDxa<int>(1),
                    Header = OxmlHelper.InchesToDxa<uint>(0.25),
                    Footer = OxmlHelper.InchesToDxa<uint>(0.25),
                    Gutter = OxmlHelper.InchesToDxa<uint>(0)
                });

            _word.MainDocumentPart.Document.Body.AppendChild(sectionProps);

            _word.PackageProperties.Created = DateTimeOffset.Now.DateTime;
            _word.PackageProperties.Creator = _authorName;
        }

        private void RenderContent(string title, int assetNumber)
        {
            _html = new StringBuilder();

            _html.AppendLine($"<div style='font-size:28pt'>{title}</div>");
            _html.AppendLine($"<div style='font-size:11pt; color:#666666;'>{CustomTranslate("Asset #")}{assetNumber}</div>");

            var isFirst = true;
            foreach (var info in _questions)
            {
                if (isFirst)
                    isFirst = false;
                else
                    _html.AppendLine("<br/>");

                RenderQuestion(info);
            }

            var htmlConverter = new HtmlConverter(_word.MainDocumentPart, this);

            try
            {
                Shift.Common.TaskRunner.RunSync(() => htmlConverter.ParseBody(_html.ToString()));
            }
            catch (Exception)
            {
                Shift.Common.TaskRunner.RunSync(() => htmlConverter.ParseBody("<p style='font-size:30pt; font-weight:bold; color:red;'>UNABLE TO PARSE HTML CODE</p>"));
            }
        }

        private void RenderQuestion(QuestionPrintHelper.IQuestionInfo info)
        {
            _html.AppendLine($"<div style='font-size:12pt;'>{info.PrimarySequence}.</div>");
            
            if (info.SecondarySequence.HasValue)
                _html.AppendLine($"<div style='font-size:10pt; color:#666666;'>{info.SecondarySequence}.</div>");

            var props = EnumerateProperties(info);
            foreach (var (name, value) in props)
                _html.AppendLine($"<div style='font-size:12pt;'><b>{name}:</b> {value}</div>");

            RenderQuestionTitle(info);

            switch (info.AttemptQuestion.Type)
            {
                case QuestionItemType.Matching:
                    RenderQuestionMatching((AttemptQuestionMatch)info.AttemptQuestion);
                    break;
                case QuestionItemType.Likert:
                    RenderQuestionLikert((AttemptQuestionLikert)info.AttemptQuestion);
                    break;
                case QuestionItemType.Ordering:
                    RenderQuestionOrdering((AttemptQuestionOrdering)info.AttemptQuestion);
                    break;
                default:
                    if (info.BankQuestion.Type.IsHotspot())
                        RenderQuestionHotspot((AttemptQuestionHotspot)info.AttemptQuestion);
                    else
                        RenderQuestionOptions(info.AttemptQuestion as AttemptQuestionDefault);
                    break;
            }

            RenderQuestionComments(info);
        }

        private void RenderQuestionTitle(QuestionPrintHelper.IQuestionInfo info)
        {
            var titleHtml = ToHtml(info.AttemptQuestion.Text);

            if (info.BankQuestion.Type.IsHotspot())
            {
                var data = HotspotImage.FromString(((AttemptQuestionHotspot)info.AttemptQuestion).Image);
                titleHtml += $"<div><img width='500' src='{data.Url}' /></div>";
            }

            _html.AppendLine($"<div style='font-size:12pt;'>{titleHtml}</div>");
        }

        private void RenderQuestionOptions(AttemptQuestionDefault questionDefault)
        {
            if (questionDefault == null)
                return;

            for (int i = 0; i < questionDefault.Options.Length; i++)
            {
                var option = questionDefault.Options[i];
                var optionTextHtml = ToHtml(option.Text);
                var letter = Calculator.ToBase26(i + 1);

                if (option.Points > 0)
                    letter = "*" + letter;

                _html.AppendLine($"<div style='font-size:12pt; padding-left:32px;'>{letter}. {optionTextHtml}</div>");
            }
        }

        private void RenderQuestionMatching(AttemptQuestionMatch question)
        {
            foreach (var pair in question.Pairs)
            {
                var left = ToHtml(pair.LeftText);
                var right = ToHtml(pair.RightText);

                _html.AppendLine($"<div style='font-size:12pt; padding-left:32px;'>{left}: {right}</div>");
            }
        }

        private void RenderQuestionLikert(AttemptQuestionLikert question)
        {
            _html.AppendLine($"<div style='font-size:12pt;'>{CustomTranslate("Columns")}:</div>");

            foreach (var column in question.Questions[0].Options)
            {
                var text = ToHtml(column.Text);
                _html.AppendLine($"<div style='font-size:12pt; padding-left:32px;'>{text}</div>");
            }

            _html.AppendLine($"<div style='font-size:12pt;'>{CustomTranslate("Rows")}:</div>");

            for (int i = 0; i < question.Questions.Length; i++)
            {
                var row = question.Questions[i];
                var text = ToHtml(row.Text);
                var letter = Calculator.ToBase26(i + 1);

                _html.AppendLine($"<div style='font-size:12pt; padding-left:32px;'>{letter}. {text}</div>");
            }
        }

        private void RenderQuestionHotspot(AttemptQuestionHotspot question)
        {
            for (int i = 0; i < question.Options.Length; i++)
            {
                var option = question.Options[i];
                var optionTextHtml = ToHtml(option.Text);
                var letter = Calculator.ToBase26(i + 1);

                if (option.Points > 0)
                    letter = "*" + letter;

                _html.AppendLine($"<div style='font-size:12pt; padding-left:32px;'>{letter}. {optionTextHtml}</div>");
            }
        }

        private void RenderQuestionOrdering(AttemptQuestionOrdering question)
        {
            for (int i = 0; i < question.Options.Length; i++)
            {
                var option = question.Options[i];
                var optionTextHtml = ToHtml(option.Text);

                _html.AppendLine($"<div style='font-size:12pt; padding-left:32px;'>{i + 1}. {optionTextHtml}</div>");
            }
        }

        private void RenderQuestionComments(QuestionPrintHelper.IQuestionInfo info)
        {
            if (!_includeAdminComments)
                return;

            var comments = info.GetAdminComments();
            if (_excludeHiddenComments)
                comments = comments.Where(x => !x.IsHidden).ToArray();

            if (comments.Count == 0)
                return;

            _html.AppendLine($"<div style='font-size:14pt; color:#0f4761;'>{CustomTranslate("Administrator Comments")}</div>");

            for (int i = 0; i < comments.Count; i++)
            {
                var comment = comments[i];

                if (i != 0)
                    _html.AppendLine("<hr/>");

                _html.AppendLine($"<div style='font-size:12pt;'><b>{comment.AuthorName}</b></div>");
                _html.AppendLine($"<div style='font-size:10pt; color:#666666;'>{FormatDateTime(comment.PostedOn)}</div>");

                if (comment.HasFlag)
                    _html.AppendLine($"<div style='font-size:12pt; color:#7a4e00;'>{CustomTranslate("Flag")}: {comment.FlagName}</div>");

                _html.AppendLine($"<div style='font-size:12pt;'>{ToHtml(comment.Text)}</div>");
            }
        }

        private string FormatDateTime(DateTimeOffset value)
        {
            value = TimeZoneInfo.ConvertTime(value, _timeZone);
            var tz = TimeZones.GetAbbreviation(_timeZone)?.GetAbbreviation(value) ?? _timeZone.Id;

            return string.Format("{0:MMM d, yyyy} at {0:h:mm tt} {1}", value, tz);
        }

        private static Regex ImagesRegex = new Regex("<img [^>]+>");
        private string ToHtml(string markdown)
        {
            var html = Markdown.ToHtml(markdown);

            if (!_includeImages)
                return StringHelper.StripHtml(html);

            var images = ImagesRegex.Matches(html);
            for (int i = 0; i < images.Count; i++)
                html = html.Replace(images[i].Value, $"[!!!image_{i}!!!]");

            var html2 = StringHelper.StripHtml(html);

            for (int i = 0; i < images.Count; i++)
                html2 = html2.Replace($"[!!!image_{i}!!!]", $"<div>{images[i].Value}</div>");

            return html2;
        }

        private IEnumerable<(string name, string value)> EnumerateProperties(QuestionPrintHelper.IQuestionInfo info)
        {
            var q = info.BankQuestion;

            yield return (CustomTranslate("Asset #"), $"{q.Asset}.{q.AssetVersion}");

            if (info.CompetencyName != null)
            {
                var index = info.CompetencyName.IndexOf('<');
                var value = index >= 0 ? info.CompetencyName.Substring(0, index) : info.CompetencyName;
                yield return (CustomTranslate("Competency"), value);
            }

            if (info.TaxonomyName != null)
                yield return (CustomTranslate("Taxonomy"), info.TaxonomyName);

            if (q.Classification.LikeItemGroup != null)
                yield return (CustomTranslate("LIG"), WebUtility.HtmlDecode(q.Classification.LikeItemGroup));

            if (q.Classification.Code != null)
                yield return (CustomTranslate("Code"), WebUtility.HtmlDecode(q.Classification.Code));

            if (q.Condition != null)
                yield return (CustomTranslate("Status"), WebUtility.HtmlDecode(q.Condition));

            if (q.Flag != FlagType.None)
                yield return (CustomTranslate("Flag"), q.Flag.GetName());

            if (q.Classification.Reference != null)
                yield return (CustomTranslate("Reference"), WebUtility.HtmlDecode(q.Classification.Reference));

            if (q.Classification.Tag != null)
                yield return (CustomTranslate("Tag"), WebUtility.HtmlDecode(q.Classification.Tag));
        }

        private string CustomTranslate(string text)
        {
            return _translator.Translate(text);
        }

        async Task<HtmlToOpenXml.IO.Resource> IWebRequest.FetchAsync(Uri requestUri, CancellationToken cancellationToken)
        {
            if (!requestUri.IsAbsoluteUri && requestUri.OriginalString.StartsWith("/"))
                requestUri = new Uri($"{_websiteUrl}/{requestUri.OriginalString}", UriKind.Absolute);

            Stream stream;

            try
            {
                stream = await StaticHttpClient.Client.GetStreamAsync(requestUri);
            }
            catch
            {
                stream = new FileStream(_defaultImagePath, FileMode.Open);
            }

            return new HtmlToOpenXml.IO.Resource
            {
                StatusCode = HttpStatusCode.OK,
                Content = stream
            };
        }

        bool IWebRequest.SupportsProtocol(string protocol) => true;
    }
}