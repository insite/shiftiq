using System;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using InSite.Admin.Standards.Documents.Utilities;
using InSite.Common.Web;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Toolbox;

namespace InSite.UI.Portal.Standards.Controls
{
    public partial class AnalysisReportStandards : BaseUserControl
    {
        #region Properties

        public AnalysisHelper.IReportDataStandardAnalysis ReportData
        {
            get => (AnalysisHelper.IReportDataStandardAnalysis)ViewState[nameof(ReportData)];
            set => ViewState[nameof(ReportData)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DownloadButton.Click += DownloadButton_Click;
        }

        #endregion

        #region Event handlers

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            if (ReportData == null)
                return;

            var fileName = string.Empty;

            var bytes = AnalysisHelper.CreateWordDocument((word, abstractNumId) =>
            {
                var data = ReportData;

                word.MainDocumentPart.Document.Body.AppendChild(
                    new Paragraph(
                        new ParagraphProperties(new ParagraphStyleId { Val = OxmlHelper.MicrosoftWordStyles.Paragraph.Heading1.Id }),
                        new Run(new Text { Text = data.Title })));

                if (data is AnalysisHelper.ReportDataStandardAnalysis1 data1)
                {
                    word.PackageProperties.Title = fileName = data1.Title;

                    Render(word, data1, abstractNumId);
                }
                else if (data is AnalysisHelper.ReportDataStandardAnalysis2 data2)
                {
                    word.PackageProperties.Title = fileName = data2.Title;

                    Render(word, data2, abstractNumId);
                }
                else if (data is AnalysisHelper.ReportDataStandardAnalysis3 data3)
                {
                    word.PackageProperties.Title = fileName = data3.Title;

                    Render(word, data3, abstractNumId);
                }
            });

            if (string.IsNullOrEmpty(fileName))
                return;

            fileName = StringHelper.Sanitize(fileName, '-') + ".docx";

            Response.SendFile(fileName, bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }

        private void Render(WordprocessingDocument word, AnalysisHelper.ReportDataStandardAnalysis1 data, int abstractNumId)
        {
            if (data.Overlap.HasValue)
            {
                word.MainDocumentPart.Document.Body.AppendChild(
                    new Paragraph(
                        new Run(
                            new RunProperties(
                                new Bold(),
                                new BoldComplexScript()),
                            new Text { Text = Common.LabelHelper.GetTranslation("Competency Overlap") }),
                        new Run(new Text { Text = $": {data.Overlap.Value:p0}" })));
            }

            if (data.Shared.IsNotEmpty())
            {
                word.MainDocumentPart.Document.Body.AppendChild(
                    new Paragraph(
                        new ParagraphProperties(new ParagraphStyleId { Val = OxmlHelper.MicrosoftWordStyles.Paragraph.Heading3.Id }),
                        new Run(new Text { Text = Common.LabelHelper.GetTranslation("Shared Competencies") })));

                AnalysisHelper.RenderGacs(word, abstractNumId, data.Shared);
            }

            if (data.Missing.IsNotEmpty())
            {
                word.MainDocumentPart.Document.Body.AppendChild(
                    new Paragraph(
                        new ParagraphProperties(new ParagraphStyleId { Val = OxmlHelper.MicrosoftWordStyles.Paragraph.Heading3.Id }),
                        new Run(new Text { Text = Common.LabelHelper.GetTranslation("Missing Competencies") })));

                AnalysisHelper.RenderGacs(word, abstractNumId, data.Missing);
            }
        }

        private void Render(WordprocessingDocument word, AnalysisHelper.ReportDataStandardAnalysis3 data, int abstractNumId)
        {
            if (data.Standards.IsNotEmpty())
            {
                word.MainDocumentPart.Document.Body.AppendChild(
                    new Paragraph(
                        new ParagraphProperties(new ParagraphStyleId { Val = OxmlHelper.MicrosoftWordStyles.Paragraph.Heading3.Id }),
                        new Run(new Text { Text = Common.LabelHelper.GetTranslation("Matches") })));

                var newNumInstance = OxmlHelper.InsertNumInstance(
                    word.MainDocumentPart,
                    new NumberingInstance(new AbstractNumId { Val = abstractNumId }));

                foreach (var standard in data.Standards)
                {
                    word.MainDocumentPart.Document.Body.AppendChild(
                        new Paragraph(
                            new ParagraphProperties(
                                new ParagraphStyleId { Val = "ListParagraph" },
                                new NumberingProperties(
                                    new NumberingLevelReference { Val = 0 },
                                    new NumberingId { Val = newNumInstance.NumberID })),
                            new Run(new Text { Text = standard.Title })));
                }
            }
        }

        private void Render(WordprocessingDocument word, AnalysisHelper.ReportDataStandardAnalysis2 data, int abstractNumId)
        {
            foreach (var report in data.Reports)
            {
                word.MainDocumentPart.Document.Body.AppendChild(
                    new Paragraph(
                        new ParagraphProperties(new ParagraphStyleId { Val = OxmlHelper.MicrosoftWordStyles.Paragraph.Heading2.Id }),
                        new Run(new Text { Text = report.Title })));

                if (report.Overlap.HasValue)
                {
                    word.MainDocumentPart.Document.Body.AppendChild(
                        new Paragraph(
                            new Run(
                                new RunProperties(
                                    new Bold(),
                                    new BoldComplexScript()),
                                new Text { Text = Common.LabelHelper.GetTranslation("Competency Overlap") }),
                            new Run(new Text { Text = $": {report.Overlap.Value:p0}" })));
                }

                if (report.Shared.IsNotEmpty())
                {
                    word.MainDocumentPart.Document.Body.AppendChild(
                        new Paragraph(
                            new ParagraphProperties(new ParagraphStyleId { Val = OxmlHelper.MicrosoftWordStyles.Paragraph.Heading3.Id }),
                            new Run(new Text { Text = Common.LabelHelper.GetTranslation("Shared Competencies") })));

                    AnalysisHelper.RenderGacs(word, abstractNumId, report.Shared);
                }

                if (report.Missing.IsNotEmpty())
                {
                    word.MainDocumentPart.Document.Body.AppendChild(
                        new Paragraph(
                            new ParagraphProperties(new ParagraphStyleId { Val = OxmlHelper.MicrosoftWordStyles.Paragraph.Heading3.Id }),
                            new Run(new Text { Text = Common.LabelHelper.GetTranslation("Missing Competencies") })));

                    AnalysisHelper.RenderGacs(word, abstractNumId, report.Missing);
                }
            }
        }

        #endregion

        #region Methods (data binding)

        public void LoadData(AnalysisHelper.IReportDataStandardAnalysis data)
        {
            Visible = true;

            ReportType1.Visible = false;
            ReportType2.Visible = false;
            ReportType3.Visible = false;

            if (data is AnalysisHelper.ReportDataStandardAnalysis1 data1)
            {
                ReportType1.Visible = true;
                DownloadButton.Visible = ReportType1.LoadData(data1);
            }
            else if (data is AnalysisHelper.ReportDataStandardAnalysis2 data2)
            {
                ReportType2.Visible = true;
                DownloadButton.Visible = ReportType2.LoadData(data2);
            }
            else if (data is AnalysisHelper.ReportDataStandardAnalysis3 data3)
            {
                ReportType3.Visible = true;
                DownloadButton.Visible = ReportType3.LoadData(data3);
            }

            ReportData = data;
        }

        public void UnloadData()
        {
            Visible = false;
            ReportData = null;
        }

        #endregion
    }
}