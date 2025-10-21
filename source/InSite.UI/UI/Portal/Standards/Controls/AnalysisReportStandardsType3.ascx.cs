using InSite.Admin.Standards.Documents.Utilities;
using InSite.Common.Web.UI;

namespace InSite.UI.Portal.Standards.Controls
{
    public partial class AnalysisReportStandardsType3 : BaseUserControl
    {
        public bool LoadData(AnalysisHelper.ReportDataStandardAnalysis3 data)
        {
            var hasItems = data.Standards.Length > 0;

            MatchesField.Visible = hasItems;
            StandardRepeater.DataSource = data.Standards;
            StandardRepeater.DataBind();

            return hasItems;
        }
    }
}