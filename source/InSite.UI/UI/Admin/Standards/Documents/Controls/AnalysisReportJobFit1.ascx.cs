using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using DocumentFormat.OpenXml.Wordprocessing;

using InSite.Admin.Standards.Documents.Utilities;
using InSite.Common.Web;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Toolbox;

namespace InSite.Admin.Standards.Documents.Controls
{
    public partial class AnalysisReportJobFit1 : BaseUserControl
    {
        #region Properties

        public AnalysisHelper.ReportDataJobFit1 ReportData
        {
            get => (AnalysisHelper.ReportDataJobFit1)ViewState[nameof(ReportData)];
            set => ViewState[nameof(ReportData)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GacObtainRepeater.ItemDataBound += GacRepeater_ItemDataBound;
            GacOverlapRepeater.ItemDataBound += GacRepeater_ItemDataBound;

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

                word.MainDocumentPart.Document.Body.AppendChild(
                    new Paragraph(
                        new Run(
                            new RunProperties(
                                new Bold(),
                                new BoldComplexScript()),
                            new Text { Text = "Competency Overlap" }),
                        new Run(new Text { Text = $": {data.OverlapValue:p0}" })));

                if (data.GacsObtain.Length > 0)
                {
                    word.MainDocumentPart.Document.Body.AppendChild(
                        new Paragraph(
                            new ParagraphProperties(new ParagraphStyleId { Val = OxmlHelper.MicrosoftWordStyles.Paragraph.Heading3.Id }),
                            new Run(new Text { Text = "Competencies Still to Obtain" })));

                    AnalysisHelper.RenderGacs(word, abstractNumId, data.GacsObtain);
                }

                if (data.GacsOverlap.Length > 0)
                {
                    word.MainDocumentPart.Document.Body.AppendChild(
                        new Paragraph(
                            new ParagraphProperties(new ParagraphStyleId { Val = OxmlHelper.MicrosoftWordStyles.Paragraph.Heading3.Id }),
                            new Run(new Text { Text = "Competencies that Overlap" })));

                    AnalysisHelper.RenderGacs(word, abstractNumId, data.GacsOverlap);
                }
            });

            if (string.IsNullOrEmpty(fileName))
                return;

            fileName = StringHelper.Sanitize(fileName, '-') + ".docx";

            Response.SendFile(fileName, bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }

        #endregion

        #region Methods (data binding)

        public void LoadData(AnalysisHelper.ReportDataJobFit1 data)
        {
            Visible = true;
            GacObtainField.Visible = true;
            GacOverlapField.Visible = true;

            CompetencyOverlap.Text = data.OverlapValue.ToString("p0");

            GacObtainRepeater.DataSource = data.GacsObtain;
            GacObtainRepeater.DataBind();

            GacOverlapRepeater.DataSource = data.GacsOverlap;
            GacOverlapRepeater.DataBind();

            var hasObtainItems = data.GacsObtain.Length > 0;
            var hasOverlapItems = data.GacsOverlap.Length > 0;

            GacObtainField.Visible = hasObtainItems;
            GacOverlapField.Visible = hasOverlapItems;
            DownloadButton.Visible = hasObtainItems || hasOverlapItems;

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