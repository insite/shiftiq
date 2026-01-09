using System.ComponentModel;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common.Linq;

namespace InSite.Custom.CMDS.Admin.Reports.Controls
{
    public partial class ExecutiveSummaryOnAchievementStatusResults : SearchResultsGridViewController<ExecutiveSummaryOnAchievementStatusFilter>
    {
        CmdsReportHelper.ExecutiveSummaryOnAchievementStatus[] result = null;

        protected override int SelectCount(ExecutiveSummaryOnAchievementStatusFilter filter)
        {
            if (result == null) result = CmdsReportHelper.SelectExecutiveSummaryOnAchievementStatus(filter);

            return result.Length;
        }

        protected override IListSource SelectData(ExecutiveSummaryOnAchievementStatusFilter filter)
        {
            if (result == null)
                result = CmdsReportHelper.SelectExecutiveSummaryOnAchievementStatus(filter);

            return result
                .ApplyPaging(filter)
                .ToList()
                .ToSearchResult();
        }

        protected string GetComplianceScore(decimal? score)
        {
            if (score == null)
                return "NA <span class='text-body-secondary'><i class='fas fa-circle'></i></span>";

            score = score / 100.0m;

            if (score >= 1.0m)
                return $"{score:p0} <span class='text-success'><i class='fas fa-flag-checkered'></i></span>";

            if (score >= 0.5m)
                return $"{score:p0} <span class='text-warning'><i class='far fa-flag'></i></span>";

            return $"{score:p0} <span class='text-danger'><i class='far fa-flag'></i></span>";
        }
    }
}