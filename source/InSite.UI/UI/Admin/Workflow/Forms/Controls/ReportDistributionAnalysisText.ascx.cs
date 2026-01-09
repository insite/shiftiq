using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using InSite.Admin.Workflow.Forms.Utilities;
using InSite.Common.Web;
using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.Admin.Workflow.Forms.Controls
{
    public partial class ReportDistributionAnalysisText : BaseUserControl
    {
        private string QuestionText
        {
            get => (string)ViewState[nameof(QuestionText)];
            set => ViewState[nameof(QuestionText)] = value;
        }

        private string[] Answers
        {
            get => (string[])ViewState[nameof(Answers)];
            set => ViewState[nameof(Answers)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DownloadCsv.Click += DownloadCsv_Click;
        }

        private void DownloadCsv_Click(object sender, EventArgs e)
        {
            var csv = new StringBuilder();
            csv.AppendLine(GetCsvValue(QuestionText));

            foreach (var answer in Answers)
                csv.AppendLine(GetCsvValue(answer));

            var filename = $"{StringHelper.Sanitize(QuestionText ?? "Untitled", '-')}-{DateTime.UtcNow:yyyy-MM-dd}";
            var bytes = Encoding.UTF8.GetBytes(csv.ToString());

            Page.Response.SendFile(filename, "csv", bytes);
        }

        private static string GetCsvValue(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            if (!text.Contains(",") && !text.Contains("\"") && !text.Contains("\n"))
                return text;

            return "\"" + text.Replace("\"", "\"\"").Replace("\n", "").Replace("\r", "") + "\"";
        }

        internal void LoadData(string questionText, IEnumerable<SubmissionAnalysis.TextItem> items)
        {
            QuestionText = StringHelper.StripHtml(questionText);

            Answers = items.Select(x => x.AnswerText).ToArray();

            DataRepeater.DataSource = items;
            DataRepeater.DataBind();
        }
    }
}