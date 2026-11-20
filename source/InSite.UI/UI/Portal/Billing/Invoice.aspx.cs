using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

using InSite.Application.Gateways.Write;
using InSite.Application.Invoices.Read;
using InSite.Application.Invoices.Write;
using InSite.Common.Web;
using InSite.Domain.Invoices;
using InSite.Domain.Payments;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;
using InSite.UI.Lobby.Utilities;
using InSite.UI.Portal.Billing.Utilities;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Billing
{
    public partial class Invoice : PortalBasePage
    {
        private Guid? InvoiceIdentifier => Guid.TryParse(Request["invoice"], out var result) ? result : (Guid?)null;
        private string ReturnBackUrl => Request["return"];

        private string TaxRegion
        {
            get => (string)ViewState[nameof(TaxRegion)];
            set => ViewState[nameof(TaxRegion)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SubmitPaymentButton.Click += SubmitPaymentButton_Click;
            NextButton.Click += NextButton_Click;
            ConfirmPaymentButton.Click += ConfirmPaymentButton_Click;
            PaymentDetail.ProvinceChanged += (s, a) => OnProvinceChanged(a.Value);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var invoice = InvoiceIdentifier.HasValue ? ServiceLocator.InvoiceSearch.GetInvoice(InvoiceIdentifier.Value) : null;
            if (invoice == null || (invoice.InvoiceStatus != "Submitted" && invoice.InvoiceStatus != "PaymentFailed"))
                RedirectToSearch();

            PageHelper.AutoBindHeader(this);

            PortalMaster.RenderHelpContent(null);
            PortalMaster.Breadcrumbs.BindTitleAndSubtitle($"{Translate("Invoice to")} {invoice.CustomerFullName}", null);

            DashboardNavigation.Visible = PortalMaster.IsSalesReady;

            if (PortalMaster.IsSalesReady)
            {
                PortalMaster.ShowAvatar(dashboardUrl: "/ui/portal/profile");

                PortalMaster.HideBreadcrumbsOnly();

                if (Identity.IsAuthenticated)
                    OverrideHomeLink("/ui/portal/management/dashboard/home");
                else
                    OverrideHomeLink("/ui/portal/billing/catalog");
            }
            else
                PortalMaster.SidebarVisible(false);

            LoadData();
        }

        private void RedirectToSearch() =>
            HttpResponseHelper.Redirect($"/ui/admin/sales/invoices/search", true);

        private void LoadData()
        {
            var invoice = ServiceLocator.InvoiceSearch.GetInvoice(InvoiceIdentifier.Value);
            var data = ServiceLocator.InvoiceSearch.GetInvoiceItems(InvoiceIdentifier.Value);

            InvoiceStatus.Text = ProductHelper.GetInvoiceStatus(invoice.InvoiceStatus);
            InvoiceItemRepeater.DataSource = data;
            InvoiceItemRepeater.DataBind();

            InvoiceSection.Visible = data.Count > 0;
            if (data.Count == 0)
            {
                PaymentStatus.AddMessage(AlertType.Warning, Translate("There are no invoice items"));
            }
        }

        private void SubmitPaymentButton_Click(object sender, EventArgs e)
        {
            PaymentDetail.Clear();
            TaxRegion = null;

            var (subtotal, tax, rate) = CalculateTotals(null);
            PaymentDetail.SetInputValues(subtotal, tax, rate, false);

            PaymentSection.Visible = PaymentSection.IsSelected = true;
            ConfirmPaymentSection.Visible = false;
        }

        private void OnProvinceChanged(string province)
        {
            TaxRegion = province;

            var (subtotal, tax, rate) = CalculateTotals(province);
            PaymentDetail.SetInputValues(subtotal, tax, rate, false);

            PaymentSection.Visible = PaymentSection.IsSelected = true;
            ConfirmPaymentSection.Visible = false;
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var invoiceNumber = GetInvoiceNumber();
            var payment = PaymentDetail.GetInputValues(invoiceNumber);

            if (!payment.Card.IsValid)
            {
                PaymentAlert.AddMessage(AlertType.Error, payment.Card.ErrorMessage);
                return;
            }

            PaymentDetailConfirm.SetInputValues(payment);

            var (subtotal, tax, rate) = CalculateTotals(TaxRegion);
            PaymentDetailConfirm.SetTaxBreakdown(subtotal, tax, rate);

            ConfirmPaymentSection.Visible = ConfirmPaymentSection.IsSelected = true;
        }

        private void ConfirmPaymentButton_Click(object sender, EventArgs e)
        {
            var invoice = ServiceLocator.InvoiceSearch.GetInvoice(InvoiceIdentifier.Value);
            if (invoice == null || (invoice.InvoiceStatus != "Submitted" && invoice.InvoiceStatus != "PaymentFailed"))
            {
                PaymentStatus.AddMessage(AlertType.Error, Translate("Invalid invoice status for payment."));
                return;
            }

            var itemRates = CalculateItemRates(TaxRegion);
            foreach (var (item, newRate, newCountry, newRegion) in itemRates)
            {
                if (item.TaxRate == newRate && item.TaxCountry == newCountry && item.TaxRegion == newRegion)
                    continue;

                ServiceLocator.SendCommand(new ChangeInvoiceItem(
                    InvoiceIdentifier.Value,
                    new InvoiceItem
                    {
                        Identifier = item.ItemIdentifier,
                        Product = item.ProductIdentifier,
                        Quantity = item.ItemQuantity,
                        Price = item.ItemPrice,
                        Description = item.ItemDescription,
                        TaxRate = newRate,
                        TaxCountry = newCountry,
                        TaxRegion = newRegion
                    }
                ));
            }

            var invoiceNumber = GetInvoiceNumber();
            var paymentDetails = PaymentDetailConfirm.GetInputValues(invoiceNumber);
            var totalAmount = ServiceLocator.InvoiceSearch.GetInvoiceTotalAmount(InvoiceIdentifier.Value);
            var paymentInput = new PaymentInput(
                invoiceNumber,
                totalAmount,
                paymentDetails.Card,
                paymentDetails.BillingAddress,
                Request.UserHostAddress
            );

            var paymentIdentifier = Guid.NewGuid();

            var command = new StartPayment(
                PaymentIdentifiers.BamboraGateway,
                Organization.Identifier,
                InvoiceIdentifier.Value,
                paymentIdentifier,
                paymentInput
            );

            ServiceLocator.SendCommand(command);

            SetPaymentStatus(paymentIdentifier);
        }

        private string GetInvoiceNumber()
        {
            var invoice = ServiceLocator.InvoiceSearch.GetInvoice(InvoiceIdentifier.Value);

            return invoice.InvoiceNumber.HasValue
                ? Domain.Invoices.Invoice.FormatInvoiceNumber(invoice.InvoiceNumber.Value)
                : invoice.InvoiceIdentifier.ToString();
        }

        private void SetPaymentStatus(Guid paymentIdentifier)
        {
            var gateway = ServiceLocator.ChangeRepository.Get<GatewayAggregate>(PaymentIdentifiers.BamboraGateway);
            var payment = gateway.Data.FindPayment(paymentIdentifier);

            if (payment == null)
            {
                PaymentStatus.AddMessage(
                    AlertType.Error,
                    Translate("Sorry, your payment wasn't saved successfully, please contact support team."));
            }
            else if (payment.Status == Shift.Constant.PaymentStatus.Completed)
            {
                IsProductPayment();

                PaymentStatus.AddMessage(AlertType.Success, Translate("Invoice has been paid successfully!"));
                PaymentSection.Visible = ConfirmPaymentSection.Visible = false;
                SubmitPaymentButton.Visible = false;

                var invoice = ServiceLocator.InvoiceSearch.GetInvoice(InvoiceIdentifier.Value);
                InvoiceStatus.Text = invoice.InvoiceStatus;
            }
            else if (payment.Status == Shift.Constant.PaymentStatus.Aborted)
            {
                var error = $"Bambora Error Code {payment.Error.Code}: {payment.Error.Message}";

                if (payment.Error.Details.IsNotEmpty())
                {
                    var items = string.Concat(payment.Error.Details.Select(d => $"<li>- <strong>{d.Field}</strong>: {d.Message}</li>"));
                    error += "<ul>" + items + "</ul>";
                }

                if (StringHelper.Equals(payment.Error.Message, "CALL HELP DESK"))
                    error += "<span class='fs-sm text-body-secondary'>" +
                        "<a target='_blank' href='https://www.shift4.com/support'>www.shift4.com/support</a>" +
                        "</span>";

                PaymentStatus.AddMessage(AlertType.Error, error);
            }
            else if (payment.Status == Shift.Constant.PaymentStatus.Started)
            {
                PaymentStatus.AddMessage(AlertType.Information, Translate("Your payment started. Please check the payment status later."));
            }
            else
            {
                PaymentStatus.AddMessage(AlertType.Error, Translate("Sorry, your payment is failed, please contact support team."));
            }
        }

        private void IsProductPayment()
        {
            if (ReturnBackUrl.HasNoValue() || !InvoiceIdentifier.HasValue)
                return;

            var customerGroup = Organization.Toolkits.Sales?.ProductCustomerGroup;
            var classEventVenueGroup = Organization.Toolkits.Sales?.ProductClassEventVenueGroup;

            ProductHelper.ProcessOrder(InvoiceIdentifier.Value, customerGroup, classEventVenueGroup);
            HttpResponseHelper.Redirect(TokenHelper.DecodeReturnBackUrl(ReturnBackUrl) + $"?invoice={InvoiceIdentifier}", false);
        }

        private List<(QInvoiceItem Item, decimal? NewRate, string NewCountry, string NewRegion)> CalculateItemRates(string region)
        {
            var hasRegion = region.IsNotEmpty();
            var provinceTax = hasRegion ? TaxHelper.GetProvinceTax(Organization.Identifier, region) : ProvinceTax.None;
            var provinceRate = provinceTax.Rate > 0m ? provinceTax.Rate : (decimal?)null;
            var items = ServiceLocator.InvoiceSearch.GetInvoiceItems(InvoiceIdentifier.Value);

            return items
                .Select(x => hasRegion && TaxHelper.IsProductTaxable(x.ProductIdentifier)
                    ? (x, provinceRate, provinceTax.Country, provinceTax.Region)
                    : (x, x.TaxRate, x.TaxCountry, x.TaxRegion))
                .ToList();
        }

        private (decimal Subtotal, decimal Tax, decimal? Rate) CalculateTotals(string region)
        {
            var itemRates = CalculateItemRates(region);

            var subtotal = 0m;
            var tax = 0m;
            decimal? rate = -1m;

            foreach (var (item, newRate, _, _) in itemRates)
            {
                var lineTotal = item.ItemPrice * item.ItemQuantity;
                subtotal += lineTotal;
                tax += TaxHelper.CalculateTax(lineTotal, newRate);

                if (rate.HasValue && newRate > 0m)
                {
                    if (rate == -1)
                        rate = newRate;
                    else if (rate != newRate)
                        rate = null;
                }
            }

            if (rate == -1)
                rate = null;

            return (subtotal, tax, rate);
        }
    }
}
