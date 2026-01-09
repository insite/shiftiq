using System.ComponentModel;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common.Linq;

namespace InSite.UI.Admin.Records.Reports.EngagementPrompt
{
    public partial class SearchResults : SearchResultsGridViewController<VLearnerActivityFilter>
    {
        protected override int SelectCount(VLearnerActivityFilter filter)
        {
            return VLearnerActivitySearch.Count(filter);
        }

        protected override IListSource SelectData(VLearnerActivityFilter filter)
        {
            filter.OrderBy = "LearnerNameLast, LearnerNameFirst, ProgramName";

            return VLearnerActivitySearch.Select(filter).ToSearchResult();
        }
    }
}