using System.Web.UI.WebControls;

using InSite.Admin.Standards.Documents.Utilities;
using InSite.Common.Web.UI;

namespace InSite.Admin.Standards.Documents.Controls
{
    public partial class AnalysisReportStandardsType2 : BaseUserControl
    {
        protected override void OnInit(System.EventArgs e)
        {
            base.OnInit(e);

            ReportRepeater.ItemDataBound += ReportRepeater_ItemDataBound;
        }

        private void ReportRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var report = (AnalysisReportStandardsType1)e.Item.FindControl("Report1");
            report.LoadData((AnalysisHelper.ReportDataStandardAnalysis1)e.Item.DataItem);
        }

        public bool LoadData(AnalysisHelper.ReportDataStandardAnalysis2 data)
        {
            var hasItems = data.Reports.Length > 0;

            ReportRepeater.Visible = hasItems;
            ReportRepeater.DataSource = data.Reports;
            ReportRepeater.DataBind();

            return hasItems;
        }
    }
}