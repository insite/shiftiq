using System.Web.UI;

using InSite.Persistence;

namespace InSite.UI.Admin.Reports.Controls
{
    public partial class ReportInfo : UserControl
    {
        public void BindReport(VReport report)
        {
            ReportTitle.Text = $"<a href=\"/ui/admin/reports/edit?id={report.ReportIdentifier}\">{report.ReportTitle}</a>" ;
            ReportDescription.Text = !string.IsNullOrEmpty(report.ReportDescription) ? report.ReportDescription.Replace("\n", "<br>"):"None";
        }
    }
}