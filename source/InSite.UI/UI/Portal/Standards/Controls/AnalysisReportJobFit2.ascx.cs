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
    public partial class AnalysisReportJobFit2 : BaseUserControl
    {
        #region Properties

        public AnalysisHelper.ReportDataJobFit2 ReportData
        {
            get => (AnalysisHelper.ReportDataJobFit2)ViewState[nameof(ReportData)];
            set => ViewState[nameof(ReportData)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            NosRepeater.ItemCreated += NosRepeater_ItemCreated;
            NosRepeater.ItemDataBound += NosRepeater_ItemDataBound;

            DownloadButton.Click += DownloadButton_Click;
        }

        #endregion

        #region Event handlers

        private void NosRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var gacRepeater = (Repeater)e.Item.FindControl("GacRepeater");
            gacRepeater.ItemDataBound += GacRepeater_ItemDataBound;
        }

        private void NosRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var obtainField = e.Item.FindControl("ObtainField");
            obtainField.Visible = true;

            var gacRepeater = (Repeater)e.Item.FindControl("GacRepeater");
            gacRepeater.DataSource = DataBinder.Eval(e.Item.DataItem, "GacsObtain");
            gacRepeater.DataBind();

            var hasItems = gacRepeater.Items.Count > 0;

            obtainField.Visible = hasItems;
        }

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

                foreach (var nos in data.NosReports)
                {
                    word.MainDocumentPart.Document.Body.AppendChild(
                        new Paragraph(
                            new ParagraphProperties(new ParagraphStyleId { Val = OxmlHelper.MicrosoftWordStyles.Paragraph.Heading2.Id }),
                            new Run(new Text { Text = nos.Title })));

                    word.MainDocumentPart.Document.Body.AppendChild(
                        new Paragraph(
                            new Run(
                                new RunProperties(
                                    new Bold(),
                                    new BoldComplexScript()),
                                new Text { Text = Common.LabelHelper.GetTranslation("Competency Overlap") }),
                            new Run(new Text { Text = $": {nos.OverlapValue:p0}" })));

                    if (nos.GacsObtain.Length > 0)
                    {
                        word.MainDocumentPart.Document.Body.AppendChild(
                            new Paragraph(
                                new ParagraphProperties(new ParagraphStyleId { Val = OxmlHelper.MicrosoftWordStyles.Paragraph.Heading3.Id }),
                                new Run(new Text { Text = Common.LabelHelper.GetTranslation("Competencies Still to Obtain") })));

                        AnalysisHelper.RenderGacs(word, abstractNumId, nos.GacsObtain);
                    }
                }
            });

            if (string.IsNullOrEmpty(fileName))
                return;

            fileName = StringHelper.Sanitize(fileName, '-') + ".docx";

            Response.SendFile(fileName, bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }

        #endregion

        #region Methods (data binding)

        public void LoadData(AnalysisHelper.ReportDataJobFit2 data)
        {
            Visible = true;

            NosRepeater.DataSource = data.NosReports;
            NosRepeater.DataBind();

            var hasItems = data.NosReports.Length > 0;

            DownloadButton.Visible = hasItems;

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