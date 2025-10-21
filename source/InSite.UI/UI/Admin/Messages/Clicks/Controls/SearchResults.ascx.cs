using System.ComponentModel;

using InSite.Application.Messages.Read;
using InSite.Common.Web.UI;

using Shift.Common.Linq;

namespace InSite.Admin.Messages.Clicks.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<VClickFilter>
    {
        protected override int SelectCount(VClickFilter filter)
        {
            return ServiceLocator.MessageSearch.CountClicks(filter);
        }

        protected override IListSource SelectData(VClickFilter filter)
        {
            return ServiceLocator.MessageSearch.GetClicks(filter).ToSearchResult();
        }
    }
}