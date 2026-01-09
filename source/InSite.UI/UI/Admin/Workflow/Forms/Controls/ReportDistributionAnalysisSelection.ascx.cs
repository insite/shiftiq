using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Workflow.Forms.Utilities;
using InSite.Common.Web;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Workflow.Forms.Controls
{
    public partial class ReportDistributionAnalysisSelection : BaseUserControl
    {
        #region Classes

        private class CsvItem
        {
            public string Label { get; set; }
            public string Text { get; set; }
            public string Frequency { get; set; }
            public string Relative { get; set; }
            public string Valid { get; set; }
        }

        private class CategoryInfo : ReportBaseOption
        {
            #region Properties

            public override string Text { get; }

            public override int Frequency => _frequency;

            public override IReportQuestion Question { get; }

            public override Guid ID => throw new NotImplementedException();

            public override string Category => throw new NotImplementedException();

            public override decimal Score => throw new NotImplementedException();

            #endregion

            #region Fields

            private int _frequency;

            #endregion

            #region Construction

            public CategoryInfo(string text, IReportQuestion question)
            {
                Text = text;
                Question = question;
            }

            #endregion

            #region Methods

            internal void Add(int frequency)
            {
                _frequency += frequency;
            }

            #endregion
        }

        #endregion

        #region Properties

        private string AnalysisTitle
        {
            get => (string)ViewState[nameof(AnalysisTitle)];
            set => ViewState[nameof(AnalysisTitle)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DownloadCsv.Click += DownloadCsv_Click;
            DownloadPng.Click += DownloadPng_Click;

            OptionRepeater.ItemDataBound += OptionRepeater_ItemDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            CommonStyle.ContentKey = typeof(ReportDistributionAnalysisSelection).ToString();
            CommonScript.ContentKey = typeof(ReportDistributionAnalysisSelection).ToString();

            DownloadPng.OnClientClick = $"selectionAnalysis.onDownloadPng('{Chart.ClientID}');";
        }

        #endregion

        #region Event handlers

        private void DownloadCsv_Click(object sender, EventArgs e)
        {
            var data = new List<CsvItem>();

            foreach (RepeaterItem ri in OptionRepeater.Items)
            {
                var labelControl = (ITextControl)ri.FindControl("Label");
                var textControl = (ITextControl)ri.FindControl("Text");
                var frequencyControl = (ITextControl)ri.FindControl("Frequency");
                var relativeControl = (ITextControl)ri.FindControl("Relative");
                var validControl = (ITextControl)ri.FindControl("Valid");

                data.Add(new CsvItem
                {
                    Label = GetValue(labelControl.Text),
                    Text = GetValue(textControl.Text),
                    Frequency = GetValue(frequencyControl.Text),
                    Relative = GetValue(relativeControl.Text),
                    Valid = GetValue(validControl.Text),
                });
            }

            if (RowNoResponse.Visible)
            {
                data.Add(new CsvItem
                {
                    Text = "No Submission",
                    Frequency = GetValue(RowNoResponseFrequency.Text),
                    Relative = GetValue(RowNoResponseRelative.Text),
                });
            }

            var helper = new CsvExportHelper(data.ToSearchResult());

            helper.AddMapping("Label", "Label");
            helper.AddMapping("Text", "Text");
            helper.AddMapping("Frequency", "Frequency");
            helper.AddMapping("Relative", "Relative");
            helper.AddMapping("Valid", "Valid");

            var filename = $"{StringHelper.Sanitize(AnalysisTitle ?? "Untitled", '-')}-{DateTime.UtcNow:yyyy-MM-dd}";
            var bytes = helper.GetBytes(Encoding.Unicode);

            Page.Response.SendFile(filename, "csv", bytes);

            string GetValue(string value)
            {
                return value == "&nbsp;" ? string.Empty : value;
            }
        }

        private void DownloadPng_Click(object sender, EventArgs e)
        {
            var imageData = Page.Request.Form["img-data"];
            if (string.IsNullOrEmpty(imageData))
                return;

            var startIndex = imageData.IndexOf("base64,");
            if (startIndex < 0)
                return;

            var imageBytes = Convert.FromBase64String(imageData.Substring(startIndex + 7));

            using (var inputMs = new MemoryStream(imageBytes))
            {
                using (var inputImage = new Bitmap(inputMs))
                {
                    using (var outputImage = new Bitmap(inputImage.Width, inputImage.Height))
                    {
                        outputImage.SetResolution(inputImage.HorizontalResolution, inputImage.VerticalResolution);

                        using (var g = Graphics.FromImage(outputImage))
                        {
                            g.Clear(Color.White);
                            g.DrawImageUnscaled(inputImage, 0, 0);
                        }

                        using (var outputMs = new MemoryStream())
                        {
                            var filename = $"{StringHelper.Sanitize(AnalysisTitle ?? "Untitled", '-')}-{DateTime.UtcNow:yyyy-MM-dd}.png";

                            Response.SendFile(filename, s => outputImage.Save(s, ImageFormat.Png));
                        }
                    }
                }
            }
        }

        private void OptionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var label = (ITextControl)e.Item.FindControl("Label");
            label.Text = Calculator.ToBase26(e.Item.ItemIndex + 1);

            var text = (ITextControl)e.Item.FindControl("Text");
            text.Text = StringHelper.StripHtml(((IReportOption)e.Item.DataItem).Text);
        }

        #endregion

        #region Data binding

        internal void LoadResponses(IReportQuestion question)
        {
            AnalysisTitle = question.Text;

            EntityName.Text = "Submission";

            OptionRepeater.DataSource = question.Options;
            OptionRepeater.DataBind();

            if (question.QuestionType == SurveyQuestionType.CheckList)
            {
                ShowRowTotal(question.FrequencySum);
                ShowRowValid(question.FrequencySum, question.FrequencySum);
            }
            else
            {
                ShowRowNoResponse(question.Analysis.SessionCount - question.FrequencySum, question.Analysis.SessionCount);
                ShowRowTotal(question.Analysis.SessionCount);
                ShowRowValid(question.FrequencySum, question.Analysis.SessionCount);

                if (question.NumericEnableAnalysis)
                    ShowRowNumericAnalysis(question);
            }

            LoadChartData(
                question.Options,
                question.QuestionType != SurveyQuestionType.CheckList
                    ? (double)(question.Analysis.SessionCount - question.FrequencySum) / question.Analysis.SessionCount
                    : (double?)null,
                false
            );
        }

        internal void LoadCategories(IReportQuestion question)
        {
            AnalysisTitle = $"{question.Text} (Categories)";

            EntityName.Text = "Category";

            var categories = new List<CategoryInfo>();
            foreach (var group in question.Options.GroupBy(x => x.Category).OrderBy(x => x.Key))
            {
                var category = new CategoryInfo(!string.IsNullOrEmpty(group.Key) ? group.Key : "Empty Category", question);

                foreach (var option in group)
                    category.Add(option.Analysis.AnswerFrequency);

                categories.Add(category);
            }

            OptionRepeater.DataSource = categories;
            OptionRepeater.DataBind();

            ShowRowNoResponse(question.Analysis.SessionCount - question.FrequencySum, question.Analysis.SessionCount);
            ShowRowTotal(question.Analysis.SessionCount);
            ShowRowValid(question.FrequencySum, question.Analysis.SessionCount);

            LoadChartData(
                categories,
                (double)(question.Analysis.SessionCount - question.FrequencySum) / question.Analysis.SessionCount,
                false
            );
        }

        private void LoadChartData(IEnumerable<IReportOption> options, double? naValue, bool isVertical)
        {
            var configData = (BarChartData)Chart.Data;

            configData.Clear();

            var dataset = configData.CreateDataset("options");
            dataset.Label = "Options";
            dataset.BackgroundColor = ColorTranslator.FromHtml("#7193bd");

            var optionNumber = 1;

            foreach (var option in options)
            {
                var datasetItem = dataset.NewItem();
                datasetItem.Label = Calculator.ToBase26(optionNumber);
                datasetItem.Value = Math.Round(100 * (double)(option.Relative ?? 0), 1, MidpointRounding.AwayFromZero);

                optionNumber++;
            }

            if (naValue.HasValue)
            {
                var datasetItem = dataset.NewItem();
                datasetItem.Label = "N/A";
                datasetItem.Value = Math.Round(100 * naValue.Value, 1, MidpointRounding.AwayFromZero);
            }

            Chart.Options.Animation.Duration = 0;
            Chart.Options.Plugins.Tooltip.Intersect = false;

            if (isVertical)
            {
                Chart.Options.Plugins.Tooltip.InteractionMode = ChartInteractionMode.X;
                Chart.OnClientPreInit = "selectionAnalysis.onVerticalChartInit";
            }
            else
            {
                Chart.Options.Plugins.Tooltip.InteractionMode = ChartInteractionMode.Y;
                Chart.Options.MaintainAspectRatio = false;
                Chart.Height = Unit.Pixel(dataset.Count * 30 + 50);
                Chart.OnClientPreInit = "selectionAnalysis.onHorizontalChartInit";
            }
        }
        private void ShowRowNoResponse(int frequency, int total)
        {
            var relative = frequency > 0 && total > 0
                ? frequency / (decimal)total
                : (decimal?)null;

            RowNoResponse.Visible = true;
            RowNoResponseFrequency.Text = frequency.ToString("n0");
            RowNoResponseRelative.Text = relative.HasValue ? FormatPercent(relative) : "&nbsp;";
        }

        private void ShowRowTotal(int total)
        {
            RowTotal.Visible = true;
            RowTotalFrequency.Text = total.ToString("n0");
        }

        private void ShowRowValid(int frequency, int total)
        {
            RowValid.Visible = true;
            RowValidText.Text = frequency.ToString("n0") + " of " + total.ToString("n0");

            if (frequency > 0 && total > 0)
                RowValidText.Text += " (" + FormatPercent(frequency / (decimal)total) + ")";
        }

        private void ShowRowNumericAnalysis(IReportQuestion question)
        {
            var validMean = 0d;
            var standardDeviation = 0d;
            var isValid = question.Options.Any(x => x.Score != 0) && question.Analysis.SelectionAnalysis.Frequency != 0;

            if (isValid)
                validMean = question.Analysis.SelectionAnalysis.CalculateValidMean(question.Options.Select(x => x.ID).ToArray());

            try
            {
                if (isValid)
                    standardDeviation = question.Analysis.SelectionAnalysis.CalculateStandardDeviation(question.ID, question.FrequencySum);
            }
            catch
            {
            }

            RowNumericAnalysis.Visible = true;
            RowNumericAnalysisMean.Text = validMean.ToString("0.00");
            RowNumericAnalysisStandardDeviation.Text = standardDeviation.ToString("0.00");
        }

        #endregion

        #region Helper methods

        protected string FormatPercent(object value)
        {
            if (value == null)
                return "&nbsp;";

            var percent = Math.Round(100 * (decimal)value, 0, MidpointRounding.AwayFromZero);

            return $"{percent:n0}%";
        }

        #endregion
    }
}