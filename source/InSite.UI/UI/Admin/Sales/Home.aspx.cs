using System;
using System.IO.Packaging;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Application.Invoices.Read;
using InSite.Application.Payments.Read;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Invoices
{
    public partial class Dashboard : AdminBasePage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                BindModelToControls();
                LoadRecentChanges();
            }
        }

        protected void BindModelToControls()
        {
            PageHelper.AutoBindHeader(this);

            var invoicesCount = ServiceLocator.InvoiceSearch.CountInvoices(CreateInvoiceFilter());
            LoadCounter(InvoicesCounter, InvoicesCount, invoicesCount, InvoicesLink, "/ui/admin/sales/invoices/search");

            var paymentsCount = ServiceLocator.PaymentSearch.CountPayments(CreatePaymentFilter());
            LoadCounter(PaymentsCounter, PaymentsCount, paymentsCount, PaymentsLink, "/ui/admin/sales/payments/search");

            var ordersCount = ServiceLocator.InvoiceSearch.CountOrders(CreateOrderFilter());
            LoadCounter(OrdersCounter, OrdersCount, ordersCount, OrdersLink, "/ui/admin/sales/orders/search");

            var productsCount = ServiceLocator.InvoiceSearch.CountProducts(CreateProductFilter());
            LoadCounter(ProductsCounter, ProductsCount, productsCount, ProductsLink, "/ui/admin/sales/products/search");

            var packagesCount = ServiceLocator.InvoiceSearch.CountProducts(CreatePackageFilter());
            LoadCounter(PackagesCounter, PackagesCount, packagesCount, PackagesLink, Sales.Packages.Forms.Search.NavigateUrl);

            // March 04, 2025 - Oleg:
            // Keep it hidden until requested to show
            // https://insite.atlassian.net/browse/DEV-3709
            //var discountsCount = ServiceLocator.PaymentSearch.CountDiscounts(CreateDiscountFilter());
            //LoadCounter(DiscountsCounter, DiscountsCount, discountsCount, DiscountsLink, "/ui/admin/sales/discounts/search");

            PageHelper.BindSubtitle(Page, Organization.CompanyName + " has " + Shift.Common.Humanizer.ToQuantity(invoicesCount, "invoice"));
        }

        private VInvoiceFilter CreateInvoiceFilter()
        {
            var filter = new VInvoiceFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier
            };
            return filter;
        }

        private QPaymentFilter CreatePaymentFilter()
        {
            var filter = new QPaymentFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier
            };
            return filter;
        }

        private TOrderFilter CreateOrderFilter()
        {
            var filter = new TOrderFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier
            };
            return filter;
        }

        private TProductFilter CreateProductFilter()
        {
            var filter = new TProductFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier
            };
            return filter;
        }

        private TProductFilter CreatePackageFilter()
        {
            var filter = new TProductFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                ProductType = "Package"
            };
            return filter;
        }

        private TDiscountFilter CreateDiscountFilter()
        {
            var filter = new TDiscountFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier
            };
            return filter;
        }

        public static void LoadCounter(HtmlGenericControl card, Literal counter, int count, HtmlAnchor link, string action)
        {
            card.Visible = CurrentSessionState.Identity.IsActionAuthorized(action);
            link.HRef = action;
            counter.Text = $@"{count:n0}";
        }

        private void LoadRecentChanges()
        {
            RecentList.LoadData(CreatePaymentFilter(), 10);
            HistoryPanel.Visible = RecentList.ItemCount > 0;
        }
    }
}