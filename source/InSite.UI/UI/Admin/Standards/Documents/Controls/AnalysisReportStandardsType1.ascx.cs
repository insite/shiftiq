using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Standards.Documents.Utilities;
using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.Admin.Standards.Documents.Controls
{
    public partial class AnalysisReportStandardsType1 : BaseUserControl
    {
        protected override void OnInit(System.EventArgs e)
        {
            base.OnInit(e);

            SharedGacRepeater.ItemDataBound += GacRepeater_ItemDataBound;
            MissingGacRepeater.ItemDataBound += GacRepeater_ItemDataBound;
        }

        private void GacRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var competencyRepeater = (Repeater)e.Item.FindControl("CompetencyRepeater");
            competencyRepeater.DataSource = DataBinder.Eval(e.Item.DataItem, "Competencies");
            competencyRepeater.DataBind();
        }

        public bool LoadData(AnalysisHelper.ReportDataStandardAnalysis1 data)
        {
            var hasShared = data.Shared.IsNotEmpty();
            var hasMissing = data.Missing.IsNotEmpty();

            OverlapOutputField.Visible = data.Overlap.HasValue;
            SharedField.Visible = hasShared;
            MissingField.Visible = hasMissing;

            OverlapOutput.Text = data.Overlap?.ToString("p0");

            SharedGacRepeater.DataSource = data.Shared;
            SharedGacRepeater.DataBind();

            MissingGacRepeater.DataSource = data.Missing;
            MissingGacRepeater.DataBind();

            return hasShared || hasMissing;
        }
    }
}