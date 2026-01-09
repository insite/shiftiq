using InSite.Admin.Workflow.Forms.Utilities;
using InSite.Common.Web.UI;

namespace InSite.Admin.Workflow.Forms.Controls
{
    public partial class ReportDistributionAnalysisNumber : BaseUserControl
    {
        internal void LoadData(IReportQuestion question)
        {
            var analysis = question.Analysis.NumberAnalysis.GetData(question.ID);

            LiteralCount.Text = analysis.Count.ToString("n0");
            LiteralMinimum.Text = analysis.Minimum.ToString("n0");
            LiteralMaximum.Text = analysis.Maximum.ToString("n0");
            LiteralSum.Text = analysis.Sum.ToString("n0");
            LiteralAverage.Text = analysis.Average.ToString("n0");
            LiteralStandardDeviation.Text = analysis.StandardDeviation.HasValue
                ? analysis.StandardDeviation.Value.ToString("n0")
                : "n/a";
            LiteralVariance.Text = analysis.Variance.HasValue
                ? analysis.Variance.Value.ToString("n0")
                : "n/a";
        }
    }
}