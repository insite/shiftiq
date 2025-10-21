using System;
using System.Data;
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
            var timeSensitiveSafetyCertificateScore = ToDecimal(row["TimeSensitiveSafetyCertificateScore"]);
            var additionalComplianceRequirementScore = ToDecimal(row["AdditionalComplianceRequirementScore"]);
            var criticalCompetencyScore = ToDecimal(row["CriticalCompetencyScore"]);
            var nonCriticalCompetencyScore = ToDecimal(row["NonCriticalCompetencyScore"]);
            var codesOfPracticeScore = ToDecimal(row["CodesOfPracticeScore"]);
            var safeOperatingPracticeScore = ToDecimal(row["SafeOperatingPracticeScore"]);

            var avgCompliance = (
                timeSensitiveSafetyCertificateScore
                + additionalComplianceRequirementScore
                + criticalCompetencyScore
                + nonCriticalCompetencyScore
                + codesOfPracticeScore
                + safeOperatingPracticeScore
                ) / 6;

            var avgComplianceLabel = (Literal)e.Row.FindControl("AvgCompliance");
            avgComplianceLabel.Text = string.Format("{0:n0}", avgCompliance);
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