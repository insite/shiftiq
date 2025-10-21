using System.ComponentModel;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common.Linq;

namespace InSite.Admin.Standards.Collections.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<StandardFilter>
    {
        protected override int SelectCount(StandardFilter filter)
        {
            return StandardSearch.Count(filter);
        }

        protected override IListSource SelectData(StandardFilter filter)
        {
            return StandardSearch
                .Bind(
                    x => new 
                    { 
                        x.StandardIdentifier,
                        x.StandardLabel,
                        x.ContentTitle,
                    }, 
                    filter)
                .ToSearchResult();
        }
    }
}