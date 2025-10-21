using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using DocumentFormat.OpenXml.Wordprocessing;

using InSite.Admin.Standards.Documents.Utilities;
using InSite.Common.Web;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Toolbox;

namespace InSite.UI.Portal.Standards.Controls
{
    public partial class AnalysisReportCareerMap : BaseUserControl
    {
        #region Properties

        public AnalysisHelper.ReportDataCareerMap ReportData
        {
            get => (AnalysisHelper.ReportDataCareerMap)ViewState[nameof(ReportData)];
            set => ViewState[nameof(ReportData)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SharedGacRepeater.ItemDataBound += GacRepeater_ItemDataBound;
            MissingGacRepeater.ItemDataBound += GacRepeater_ItemDataBound;

            DownloadButton.Click += DownloadButton_Click;
        }

        #endregion

        #region Event handlers

        private void GacRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var competencyRepeater = (Repeater)e.Item.FindControl("CompetencyRepeater");
            competencyRepeater.DataSource = DataBinder.Eval(e.Item.DataItem, "Competencies");
            competencyRepeater.DataBind();
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            if (ReportData == null)
                return;

            var fileName = string.Empty;

            var bytes = AnalysisHelper.CreateWordDocument((word, abstractNumId) =>
            {
                var data = ReportData;

                word.PackageProperties.Title = fileName = data.Title;

                word.MainDocumentPart.Document.Body.AppendChild(
                    new Paragraph(
                        new ParagraphProperties(new ParagraphStyleId { Val = OxmlHelper.MicrosoftWordStyles.Paragraph.Heading1.Id }),
                        new Run(new Text { Text = data.Title })));

                if (data.Overlap.HasValue)
                {
                    word.MainDocumentPart.Document.Body.AppendChild(
                        new Paragraph(
                            new Run(
                                new RunProperties(
                                    new Bold(),
                                    new BoldComplexScript()),
                                new Text { Text = Common.LabelHelper.GetTranslation("Competency Overlap") }),
                            new Run(new Text { Text = $": {data.Overlap:p0}" })));
                }

                if (data.SharedCompetencies.IsNotEmpty())
                {
                    word.MainDocumentPart.Document.Body.AppendChild(
                        new Paragraph(
                            new ParagraphProperties(new ParagraphStyleId { Val = OxmlHelper.MicrosoftWordStyles.Paragraph.Heading3.Id }),
                            new Run(new Text { Text = Common.LabelHelper.GetTranslation("Shared Competencies") })));

                    AnalysisHelper.RenderGacs(word, abstractNumId, data.SharedCompetencies);
                }

                if (data.MissingCompetencies.IsNotEmpty())
                {
                    word.MainDocumentPart.Document.Body.AppendChild(
                        new Paragraph(
                            new ParagraphProperties(new ParagraphStyleId { Val = OxmlHelper.MicrosoftWordStyles.Paragraph.Heading3.Id }),
                            new Run(new Text { Text = Common.LabelHelper.GetTranslation("Missing Competencies") })));

                    AnalysisHelper.RenderGacs(word, abstractNumId, data.MissingCompetencies);
                }
            });

            if (string.IsNullOrEmpty(fileName))
                return;

            fileName = StringHelper.Sanitize(fileName, '-') + ".docx";

            Response.SendFile(fileName, bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }

        #endregion

        #region Methods (data binding)

        public void LoadData(AnalysisHelper.ReportDataCareerMap data)
        {
            var hasShared = data.SharedCompetencies.IsNotEmpty();
            var hasMissing = data.MissingCompetencies.IsNotEmpty();

            Visible = true;
            OverlapField.Visible = data.Overlap.HasValue;
            SharedGacRepeaterField.Visible = hasShared;
            MissingGacRepeaterField.Visible = hasMissing;

            CompetencyOverlap.Text = data.Overlap?.ToString("p0");

            SharedGacRepeater.DataSource = data.SharedCompetencies;
            SharedGacRepeater.DataBind();

            MissingGacRepeater.DataSource = data.MissingCompetencies;
            MissingGacRepeater.DataBind();

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