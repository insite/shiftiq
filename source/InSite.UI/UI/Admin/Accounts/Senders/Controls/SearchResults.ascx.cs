using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common.Linq;

namespace InSite.Admin.Accounts.Senders.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<TSenderFilter>
    {
        protected override int SelectCount(TSenderFilter filter)
        {
            return TSenderSearch.Count(filter);
        }

        protected override IListSource SelectData(TSenderFilter filter)
        {
            filter.OrderBy = "SenderNickname";

            return TSenderSearch
                .BindByFilter(x => new
                {
                    SenderIdentifier = x.SenderIdentifier,
                    SenderType = x.SenderType,
                    SenderNickname = x.SenderNickname,
                    SenderName = x.SenderName,
                    SenderEmail = x.SenderEmail,
                    SystemMailbox = x.SystemMailbox,
                    CompanyAddress = x.CompanyAddress,
                    CompanyCity = x.CompanyCity,
                    CompanyPostalCode = x.CompanyPostalCode,
                    CompanyCountry = x.CompanyCountry,
                    MessageCount = x.Messages.Count()
                }, filter)
                .ToList()
                .ToSearchResult();
        }
    }
}