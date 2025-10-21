using System.ComponentModel;

using InSite.Common.Web.UI;
using InSite.Persistence.Integration.DirectAccess;

namespace InSite.UI.Desktops.Custom.SkilledTradesBC.Individuals.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<IndividualFilter>
    {
        protected override int SelectCount(IndividualFilter filter)
        {
            return DirectAccessSearch.CountByFilter(filter);
        }

        protected override IListSource SelectData(IndividualFilter filter)
        {
            return DirectAccessSearch.SelectByFilter(filter);
        }
    }
}