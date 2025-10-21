using System;
using System.Collections.Generic;

using InSite.Application.Invoices.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Sales.Packages.Forms
{
    public partial class Search : SearchPage<TProductFilter>
    {
        public const string NavigateUrl = "/ui/admin/sales/packages/search";

        public static void Redirect() => HttpResponseHelper.Redirect(NavigateUrl);

        public override string EntityName => "Package";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Package", Create.NavigateUrl, null, null));
        }

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return new[]
            {
                new DownloadColumn(nameof(TProduct.ProductIdentifier), "Package Identifier"),
                new DownloadColumn(nameof(TProduct.ProductName), "Package Title"),
                new DownloadColumn(nameof(TProduct.ProductDescription), "Description"),
                new DownloadColumn(nameof(TProduct.ProductQuantity), "Number of Packages"),
                new DownloadColumn(nameof(TProduct.ProductPrice), "Package Price")
            };
        }
    }
}