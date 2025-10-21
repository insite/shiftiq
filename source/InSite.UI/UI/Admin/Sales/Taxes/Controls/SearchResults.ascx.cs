using System.Linq;
using System.ComponentModel;

using InSite.Application.Invoices.Read;
using InSite.Common.Web.UI;

using Shift.Common.Linq;

namespace InSite.UI.Admin.Sales.Taxes.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<TTaxFilter>
    {
        protected override int SelectCount(TTaxFilter filter)
        {
            return ServiceLocator.InvoiceSearch.CountTaxes(filter);
        }

        protected override IListSource SelectData(TTaxFilter filter)
        {
            return ServiceLocator.InvoiceSearch.GetTaxes(filter)
                .Select(x => new SearchResultRow
                {
                    TaxIdentifier = x.TaxIdentifier,
                    TaxName = x.TaxName,
                    CountryName = ServiceLocator.CountrySearch.SelectByCode(x.CountryCode)?.Name ?? x.CountryCode,
                    RegionName = ServiceLocator.ProvinceSearch.Unabbreviate(x.RegionCode),
                    TaxPercent = x.TaxRate * 100m
                })
                .ToList()
                .ToSearchResult();
        }
    }
}