using System.ComponentModel;

using InSite.Application.Contacts.Read;
using InSite.Common.Web.UI;

using Shift.Common.Linq;

namespace InSite.UI.Admin.Identity.UsersConnections.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QUserConnectionFilter>
    {
        protected override int SelectCount(QUserConnectionFilter filter)
        {
            return ServiceLocator.UserSearch.CountConnections(filter);
        }

        protected override IListSource SelectData(QUserConnectionFilter filter)
        {
            return ServiceLocator.UserSearch.GetConnections(filter, x => x.ToUser, x => x.FromUser).ToSearchResult();
        }
    }
}