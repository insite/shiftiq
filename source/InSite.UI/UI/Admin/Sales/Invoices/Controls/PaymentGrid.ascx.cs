using System;
using System.ComponentModel;

using InSite.Application.Payments.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Invoices.Controls
{
    public partial class PaymentGrid : SearchResultsGridViewController<QPaymentFilter>
    {
        protected override bool IsFinder => false;

        public int LoadData(Guid invoiceIdentifier)
        {
            var filter = new QPaymentFilter { InvoiceIdentifier = invoiceIdentifier, OrganizationIdentifier = Organization.Identifier };

            Search(filter);

            return RowCount;
        }

        protected override int SelectCount(QPaymentFilter filter)
        {
            return ServiceLocator.PaymentSearch.CountPayments(filter);
        }

        protected override IListSource SelectData(QPaymentFilter filter)
        {
            return ServiceLocator.PaymentSearch
                .GetPayments(filter)
                .ToSearchResult();
        }

        protected static string GetLocalTime(object item)
        {
            var when = (DateTimeOffset?)item;
            return when.FormatDateOnly(User.TimeZone, nullValue: string.Empty);
        }
    }
}