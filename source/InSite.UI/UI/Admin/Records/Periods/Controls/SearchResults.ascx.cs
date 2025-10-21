using System.ComponentModel;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

using Shift.Common.Linq;

namespace InSite.Admin.Records.Periods.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QPeriodFilter>
    {
        protected override int SelectCount(QPeriodFilter filter)
        {
            return ServiceLocator.PeriodSearch.CountPeriods(filter);
        }

        protected override IListSource SelectData(QPeriodFilter filter)
        {
            return ServiceLocator.PeriodSearch
                .GetPeriods(filter)
                .ToSearchResult();
        }
    }
}