using System.ComponentModel;

using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Admin.Assets.Contents.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<TContentFilter>
    {
        protected override int SelectCount(TContentFilter filter)
        {
            return TContentSearch.Count(filter);
        }

        protected override IListSource SelectData(TContentFilter filter)
        {
            return TContentSearch.Select(filter);
        }
    }
}