using System;
using System.ComponentModel;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Common.Web.Cmds;
using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;

namespace InSite.Cmds.Controls.Training.Achievements
{
    public partial class AchievementSearchResults : SearchResultsGridViewController<VCmdsAchievementFilter>
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowDataBound += Grid_RowDataBound;
        }

        private void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            var row = (DataRowView)e.Row.DataItem;
            var validForUnit = row["ValidForUnit"] as string;
            var validForCount = row["ValidForCount"] as int?;
            var isTimeSensitive = validForCount.HasValue;

            var timeSensitiveImage = (HtmlGenericControl)e.Row.FindControl("TimeSensitiveImage");
            TimeSensitivityHelper.SetTimeSensitiveImage(isTimeSensitive, validForUnit, validForCount, timeSensitiveImage);
        }

        protected override int SelectCount(VCmdsAchievementFilter filter)
        {
            return VCmdsAchievementSearch.CountSearchResults(filter);
        }

        protected override IListSource SelectData(VCmdsAchievementFilter filter)
        {
            filter.OrganizationCode = Organization.Code;

            return VCmdsAchievementSearch.SelectSearchResults(filter, false);
        }

        protected int GetProgressionsCount(Guid achievementIdentifier)
        {
            return VCmdsCredentialSearch.Count(x => x.AchievementIdentifier == achievementIdentifier);
        }

        protected int GetCompaniesCount(Guid achievementIdentifier)
        {
            return VCmdsAchievementOrganizationSearch.Count(x => x.AchievementIdentifier == achievementIdentifier);
        }

        protected int GetDepartmentsCount(Guid achievementIdentifier)
        {
            return VCmdsAchievementDepartmentSearch.Count(x => x.AchievementIdentifier == achievementIdentifier);
        }
    }
}