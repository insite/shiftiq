using System;
using System.Collections.Generic;

using InSite.Application.Invoices.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.UI.Admin.Sales.Taxes.Controls;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Sales.Taxes
{
    public partial class Search : SearchPage<TTaxFilter>
    {
        private static readonly DownloadColumn[] _downloadColumns =
        {
            new DownloadColumn(nameof(SearchResultRow.TaxIdentifier), "Tax Identifier"),
            new DownloadColumn(nameof(SearchResultRow.TaxName), "Tax Name"),
            new DownloadColumn(nameof(SearchResultRow.CountryName), "Country"),
            new DownloadColumn(nameof(SearchResultRow.RegionName), "Province / Region"),
            new DownloadColumn(nameof(SearchResultRow.TaxPercent), "Tax Percent"),
        };

        public override string EntityName => "Tax";

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return _downloadColumns;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Tax", "/ui/admin/sales/taxes/create", null, null));
        }
    }
}