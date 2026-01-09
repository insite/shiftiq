using System;
using System.Web.UI;

using InSite.Application.Gateways.Write;
using InSite.Common.Web;
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

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SubmitPaymentButton.Click += SubmitPaymentButton_Click;
            NextButton.Click += NextButton_Click;
            ConfirmPaymentButton.Click += ConfirmPaymentButton_Click;
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

            PortalMaster.ShowAvatar(dashboardUrl: "/ui/portal/profile");
            PortalMaster.RenderHelpContent(null);
            PortalMaster.Breadcrumbs.BindTitleAndSubtitle($"{Translate("Invoice to")} {invoice.CustomerFullName}", null);
            PortalMaster.HideBreadcrumbsOnly();

            if (Identity.IsAuthenticated)
                OverrideHomeLink("/ui/portal/management/dashboard/home");
            else
                OverrideHomeLink("/ui/portal/billing/catalog");

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

            var amount = ServiceLocator.InvoiceSearch.GetInvoiceTotalAmount(InvoiceIdentifier.Value);
            PaymentDetail.SetInputValues(amount, false);

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

            ConfirmPaymentSection.Visible = ConfirmPaymentSection.IsSelected = true;
        }

        private void ConfirmPaymentButton_Click(object sender, EventArgs e)
        {
            var invoiceNumber = GetInvoiceNumber();
            var paymentInput = PaymentDetailConfirm.GetInputValues(invoiceNumber);
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

            if (payment != null)
            {
                if (payment.Status == Shift.Constant.PaymentStatus.Completed)
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
                    if (payment.Error.Details != null)
                    {
                        error += "<ul>";
                        foreach (var detail in payment.Error.Details)
                            error += $"<li>- <strong>{detail.Field}</strong>: {detail.Message}</li>";
                        error += "</ul>";
                    }
                    if (StringHelper.Equals(payment.Error.Message, "CALL HELP DESK"))
                        error += "<span class='fs-sm text-body-secondary'><a target='_blank' href='https://www.bambora.com/en/ca/contact-worldline/'>www.bambora.com/en/ca/contact-worldline</a></span>";
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
            else
            {
                PaymentStatus.AddMessage(AlertType.Error, Translate("Sorry, your payment wasn't saved successfully, please contact support team."));
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
    }
}
