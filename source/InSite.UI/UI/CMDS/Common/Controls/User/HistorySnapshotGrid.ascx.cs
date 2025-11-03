using System;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Persistence.Plugin.CMDS;

using Shift.Common;

namespace InSite.Cmds.Controls.Reports.Compliances
{
    public partial class HistorySnapshotGrid : UserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.DataBinding += Grid_DataBinding;
            Grid.RowDataBound += Grid_RowDataBound;
        }

        private void Grid_DataBinding(object source, EventArgs e)
        {
            Grid.DataSource = ZUserStatusSearch
                .SelectByFilter(new EmployeeComplianceSnapshotFilter
                {
                    Paging = Paging.SetPage(1, int.MaxValue)
                });
        }

        private void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            var row = (DataRowView)e.Row.DataItem;

            var asAt = (DateTime)row["AsAt"];
            var scoreFormatChangedAt = new DateTime(2025, 7, 13);

            var scores = new[]
            {
                ToNullableDecimal(row["TimeSensitiveSafetyCertificateScore"]),
                ToNullableDecimal(row["AdditionalComplianceRequirementScore"]),
                ToNullableDecimal(row["CriticalCompetencyScore"]),
                ToNullableDecimal(row["NonCriticalCompetencyScore"]),
                ToNullableDecimal(row["CodesOfPracticeScore"]),
                ToNullableDecimal(row["SafeOperatingPracticeScore"])
            };

            var validScores = scores.Where(s => s.HasValue).Select(s => s.Value).ToList();
            var avgCompliance = validScores.Any() ? validScores.Average() : 0m;

            var avgComplianceLabel = (Literal)e.Row.FindControl("AvgCompliance");
            avgComplianceLabel.Text = asAt > scoreFormatChangedAt
                ? string.Format("{0:P2}", avgCompliance)  // P2 = percentage format for scores in the range 0..1
                : string.Format("{0:N2}%", avgCompliance) // N2 = percentage format for scores in the range 0..100
                ;

            decimal? ToNullableDecimal(object value)
            {
                if (value == null || value == DBNull.Value)
                    return null;
                return Convert.ToDecimal(value);
            }
        }

        decimal ToDecimal(object o)
        {
            if (o == DBNull.Value || o == null)
                return 0;

            return (decimal)o;
        }

        public void LoadData()
        {
            Grid.PageIndex = 0;
            Grid.VirtualItemCount = ZUserStatusSearch.CountByFilter(new EmployeeComplianceSnapshotFilter());
            Grid.DataBind();
        }
    }
}