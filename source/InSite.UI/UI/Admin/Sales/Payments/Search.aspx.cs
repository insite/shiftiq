using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Admin.Payments.Controls;
using InSite.Application.Payments.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Sales.Payments
{
    public partial class Search : SearchPage<QPaymentFilter>
    {
        public override string EntityName => "Payment";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);
        }

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            var columns = BaseSearchDownload
                .GetColumns(typeof(SearchResults.ExportDataItem))
                .ToList();

            var taxPercentColumn = columns.Find(x => x.Name == nameof(SearchResults.ExportDataItem.TaxPercent));
            if (taxPercentColumn != null)
                taxPercentColumn.Format = "0.00%";

            return columns;
        }
    }
}