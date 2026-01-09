using System.ComponentModel;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common.Linq;

namespace InSite.Admin.Utilities.Collections.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<TCollectionFilter>
    {
        protected override int SelectCount(TCollectionFilter filter)
        {
            return TCollectionSearch.Count(filter);
        }

        protected override IListSource SelectData(TCollectionFilter filter)
        {
            filter.OrderBy = nameof(TCollection.CollectionName);

            return TCollectionSearch.Bind(
                x => new
                {
                    x.CollectionIdentifier,
                    x.CollectionName,
                    x.CollectionTool,
                    x.CollectionType,
                    x.CollectionProcess,
                    ItemsCount = x.Items.Count()
                },
                filter
            ).ToSearchResult();
        }
    }
}