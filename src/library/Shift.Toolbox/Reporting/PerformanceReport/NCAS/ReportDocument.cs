using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

using Shift.Toolbox.Reporting.PerformanceReport.Models;

namespace Shift.Toolbox.Reporting.PerformanceReport.NCAS
{
    internal class ReportDocument : IDocument
    {
        #region Constants

        private const string LogoResource = "Shift.Toolbox.Reporting.PerformanceReport.NCAS.Inspire_Logo.png";
        private const string FontFamilyName = "Calibri";

        private static TextStyle ReportTitle => TextStyle.Default.FontFamily(FontFamilyName).FontColor(Colors.Black).FontSize(14).SemiBold();
        private static TextStyle NormalText => TextStyle.Default.FontFamily(FontFamilyName).FontColor(Colors.Black).FontSize(10);
        private static TextStyle BoldText => TextStyle.Default.FontFamily(FontFamilyName).FontColor(Colors.Black).FontSize(10).Bold();
        private static TextStyle ItalicText => TextStyle.Default.FontFamily(FontFamilyName).FontColor(Colors.Black).FontSize(10).Italic();
        private static TextStyle SmallText => TextStyle.Default.FontFamily(FontFamilyName).FontColor(Colors.Black).FontSize(8);

        private static readonly CultureInfo _frenchCulture = CultureInfo.GetCultureInfo("fr-fr");

        #endregion

        #region Constructor and fields

        private readonly string _language;
        private readonly decimal _emergentScore;
        private readonly decimal _consistentScore;
        private readonly decimal _maxScore;
        private readonly UserReport _userReport;
        private readonly DocumentText.Text _text;

        public ReportDocument(
            string language,
            decimal emergentScore,
            decimal consistentScore,
            decimal maxScore,
            UserReport userReport
            )
        {
            if (language != "en" && language != "fr")
                throw new ArgumentException($"Unsupported language: {language}");

            if (userReport == null)
                throw new ArgumentNullException("userReport");

            if (string.IsNullOrEmpty(userReport.FullName))
                throw new ArgumentNullException("userReport.FullName");

            if (userReport.Areas == null)
                throw new ArgumentNullException("userReport.Areas");

            if (userReport.AreaScores == null)
                throw new ArgumentNullException("userReport.AreaScores");

            if (userReport.AreaScores.AreaScores == null)
                throw new ArgumentNullException("userReport.AreaScores.AreaScores");

            if (userReport.AreaScores.AssessmentTypeDates == null)
                throw new ArgumentNullException("userReport.AreaScores.AssessmentTypeDates");

            _language = language;
            _emergentScore = emergentScore;
            _consistentScore = consistentScore;
            _maxScore = maxScore;
            _userReport = userReport;
            _text = DocumentText.GetText(_userReport.ReportType);
        }

        #endregion

        #region IDocument

        DocumentMetadata IDocument.GetMetadata() => DocumentMetadata.Default;
        DocumentSettings IDocument.GetSettings() => DocumentSettings.Default;

        void IDocument.Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.MarginHorizontal(90);
                    page.MarginVertical(20);
                    page.Content().Element(ComposeContent);
                    page.Footer().AlignCenter().Text(text => text.CurrentPageNumber().Style(NormalText));
                });
        }

        #endregion

        #region Pdf

        void ComposeContent(IContainer container)
        {
            var chartItems = GetChartItems();

            container.Column(column =>
            {
                column.Spacing(10);
                column.Item().Element(ComposeLogo);
                column.Item().AlignCenter().Text(_text.TitleText).Style(ReportTitle);
                column.Item().Element(ComposeReportInfo1);
                column.Item().Element(ComposeReportInfo2);
                column.Item().Element(ComposeClarification);
                column.Item().PaddingTop(10).Text(_text.Figure1).Style(BoldText);
                column.Item().Component(new ReportChart(_text, _emergentScore, _consistentScore, _maxScore, chartItems, FontFamilyName));
                column.Item().PageBreak();
                column.Item().Element(ComposeAreaDescriptions);
                column.Item().Element(ComposeNextSteps);
            });
        }

        void ComposeLogo(IContainer container)
        {
            var logo = GetLogo();

            container.Column(column =>
            {
                column.Item().Width(150).Image(logo).FitWidth();
            });
        }

        Image GetLogo()
        {
            using (var stream = GetType().Assembly.GetManifestResourceStream(LogoResource))
            {
                stream.Position = 0;
                return Image.FromStream(stream);
            }
        }

        void ComposeReportInfo1(IContainer container)
        {
            var nursingRole = string.IsNullOrEmpty(_userReport.NursingRoleText)
                ? _text.NursingRole
                : _userReport.NursingRoleText;

            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Cell().Element(HeaderCellStyle).Text(_text.AssessmentTakerText).Style(ItalicText);
                table.Cell().Element(HeaderCellStyle).Text(_text.InspireIDText).Style(ItalicText);
                table.Cell().Element(HeaderCellStyle).Text(_text.NursingRoleText).Style(ItalicText);
                table.Cell().Element(HeaderCellStyle).Text(_text.ReportIDText).Style(ItalicText);

                table.Cell().Element(BodyCellStyle).Text(_userReport.FullName).Style(NormalText);
                table.Cell().Element(BodyCellStyle).Text(_userReport.PersonCode).Style(NormalText);
                table.Cell().Element(BodyCellStyle).Text(nursingRole).Style(NormalText);
                table.Cell().Element(BodyCellStyle).Text(_text.OccupationInterest + _userReport.PersonCode.TrimStart('0')).Style(NormalText);
            });

            IContainer HeaderCellStyle(IContainer c)
                => c.BorderBottom(0.5f).BorderTop(0.5f).BorderColor(Colors.Black).PaddingHorizontal(5).PaddingVertical(3);

            IContainer BodyCellStyle(IContainer c)
                => c.BorderBottom(0.5f).BorderColor(Colors.Black).PaddingHorizontal(5).PaddingVertical(2);
        }

        void ComposeReportInfo2(IContainer container)
        {
            var assessmentTypeDates = _userReport.AreaScores.AssessmentTypeDates;

            var cbaDate = assessmentTypeDates
                .Where(x => string.Equals(x.AssessmentType, "CBA", StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.Date)
                .FirstOrDefault()?.Date;

            var slaDate = assessmentTypeDates
                .Where(x => string.Equals(x.AssessmentType, "SLA", StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.Date)
                .FirstOrDefault()?.Date;

            var cbaDateText = GetDateText(cbaDate);
            var slaDateText = GetDateText(slaDate);

            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Cell().Element(HeaderCellStyle).Text(_text.DateReportIssuedText).Style(ItalicText);
                table.Cell().Element(HeaderCellStyle).Text(_text.CBAAdministrationDateText).Style(ItalicText);
                table.Cell().Element(HeaderCellStyle).Text(_text.SLAAdministrationDateText).Style(ItalicText);

                table.Cell().Element(BodyCellStyle).Text(GetDateText(_userReport.ReportIssued)).Style(NormalText);
                table.Cell().Element(BodyCellStyle).Text(cbaDateText).Style(NormalText);
                table.Cell().Element(BodyCellStyle).Text(slaDateText).Style(NormalText);
            });

            IContainer HeaderCellStyle(IContainer c)
                => c.BorderBottom(0.5f).BorderTop(0.5f).BorderColor(Colors.Black).PaddingHorizontal(5).PaddingVertical(3);

            IContainer BodyCellStyle(IContainer c)
                => c.BorderBottom(0.5f).BorderColor(Colors.Black).PaddingHorizontal(5).PaddingVertical(2);
        }

        void ComposeClarification(IContainer container)
        {
            var description = string.IsNullOrEmpty(_userReport.Description)
                ? _text.Description
                : _userReport.Description;

            container.Column(column =>
            {
                column.Spacing(5);
                RenderMarkdown(column, description);
            });
        }

        void ComposeAreaDescriptions(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().AlignCenter().Text(_text.Table1).Style(NormalText);

                column.Item().PaddingHorizontal(10)
                    .BorderBottom(0.5f)
                    .BorderColor(Colors.Black)
                    .PaddingBottom(2)
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(100);
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderCellStyle).Text(_text.DimensionColumnHeader);
                            header.Cell().Element(HeaderCellStyle).Text(_text.DescriptionColumnHeader);
                        });

                        foreach (var area in _userReport.Areas)
                        {
                            table.Cell().Element(c => c.PaddingTop(2).DefaultTextStyle(SmallText)).Text(area.Name);
                            table.Cell().Element(c => c.PaddingTop(2).PaddingLeft(4).DefaultTextStyle(SmallText)).Text(area.Description);
                        }
                    });
            });

            IContainer HeaderCellStyle(IContainer c)
                => c.BorderHorizontal(0.5f).BorderColor(Colors.Black).PaddingVertical(2).DefaultTextStyle(SmallText);
        }

        void ComposeNextSteps(IContainer container)
        {
            container.Column(column => RenderMarkdown(column, _text.NextSteps));
        }

        void RenderMarkdown(ColumnDescriptor column, string text)
        {
            var paragraphs = text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var paragraph in paragraphs)
                column.Item().Text(t => RenderMarkdownLine(t, paragraph));
        }

        void RenderMarkdownLine(TextDescriptor t, string text)
        {
            var index = 0;

            while (index < text.Length)
            {
                var start = text.IndexOf("**", index);
                if (start < 0)
                {
                    t.Span(text.Substring(index)).Style(NormalText);
                    break;
                }

                if (index != start)
                    t.Span(text.Substring(index, start - index)).Style(NormalText);

                start += "**".Length;

                var end = text.IndexOf("**", start);
                if (end < 0)
                {
                    t.Span(text.Substring(start)).Style(BoldText);
                    break;
                }

                t.Span(text.Substring(start, end - start)).Style(BoldText);

                index = end + "**".Length;
            }
        }

        ChartItem[] GetChartItems()
        {
            var chartItems = new List<ChartItem>();

            foreach (var area in _userReport.Areas)
            {
                var areaScore = _userReport.AreaScores.AreaScores.FirstOrDefault(x => x.AreaId == area.Id)
                    ?? throw new ArgumentException($"Area score {area.Id} is not found");

                var chartItem = new ChartItem
                {
                    Label = area.Name,
                    Score = areaScore.GetWeightedScore()
                };

                chartItems.Add(chartItem);
            }

            return chartItems.ToArray();
        }

        #endregion

        #region Helper methods

        private string GetDateText(DateTime? date)
        {
            if (date == null)
                return _text.None;

            switch (_language)
            {
                case "en":
                    return date.Value.ToString("MMM d, yyyy");
                case "fr":
                    return date.HasValue ? date.Value.ToString("MMM d, yyyy", _frenchCulture) : "None";
                default:
                    throw new ArgumentException($"Language: {_language}");
            }
        }

        #endregion
    }
}
