using System.ComponentModel;

using InSite.Application.Payments.Read;
using InSite.Common.Web.UI;

using Shift.Common.Linq;

namespace InSite.Admin.Payments.Discounts.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<TDiscountFilter>
    {
        protected override int SelectCount(TDiscountFilter filter)
        {
            return ServiceLocator.PaymentSearch.CountDiscounts(filter);
        }

        protected override IListSource SelectData(TDiscountFilter filter)
        {
            return ServiceLocator.PaymentSearch.GetDiscounts(filter)
                .ToSearchResult();
        }

        protected static string SplitLines(object o)
        {
            return o != null
                ? ((string)o).Replace(System.Environment.NewLine, "<br/>")
                : null;
        }
    }
}