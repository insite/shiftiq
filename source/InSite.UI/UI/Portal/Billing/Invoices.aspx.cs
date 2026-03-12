using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using InSite.Admin.Invoices.Controls;
using InSite.Application.Invoices.Read;
using InSite.Common.Web;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;
using InSite.UI.Portal.Billing.Utilities;
using InSite.Web.Helpers;

using Shift.Common;
using Shift.Constant;

using UxButton = InSite.Common.Web.UI.Button;

namespace InSite.UI.Portal.Billing
{
    public class SearchResultPacket
    {
        public decimal? InvoiceAmount { get; set; }
        public string InvoiceDrafted { get; set; }
        public Guid InvoiceIdentifier { get; set; }
        public string InvoicePaid { get; set; }
        public string InvoiceStatus { get; set; }
        public string InvoiceSubmitted { get; set; }
        public string ItemsHtml { get; set; }
        public bool HasRegistrationItem { get; set; }
    }

    public partial class Invoices : PortalBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            UnpaidInvoiceRepeater.ItemDataBound += Repeater_ItemDataBound;
            UnpaidInvoiceRepeater.ItemCreated += Repeater_ItemCreated;

            PaidInvoiceRepeater.ItemDataBound += Repeater_ItemDataBound;
            PaidInvoiceRepeater.ItemCreated += Repeater_ItemCreated;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        private bool LoadUnpaidInvoices()
        {
            var invoiceFilter = new VInvoiceFilter
            {
                OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier,
                CustomerIdentifier = User.UserIdentifier,
                ExcludeInvoiceStatuses = new[] { InvoiceStatus.Paid.ToString() },
            };

            var invoices = ServiceLocator
                .InvoiceSearch
                .GetInvoices(invoiceFilter)
                .Select(GetPacket)
                .ToList();

            if (invoices.Count > 0)
            {
                UnpaidInvoiceRepeater.DataSource = invoices;
                UnpaidInvoiceRepeater.DataBind();
            }

            UnpaidInvoicePanel.Visible = invoices.Count > 0;

            return invoices.Count > 0;
        }

        private bool LoadPaidInvoices()
        {
            var receiptFilter = new VInvoiceFilter
            {
                OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier,
                CustomerIdentifier = User.UserIdentifier,
                InvoiceStatus = InvoiceStatus.Paid.ToString()
            };

            var invoices = ServiceLocator
               .InvoiceSearch
               .GetInvoices(receiptFilter)
               .Select(GetPacket)
               .ToList();

            if (invoices.Count > 0)
            {
                PaidInvoiceRepeater.DataSource = invoices;
                PaidInvoiceRepeater.DataBind();
            }

            PaidInvoicePanel.Visible = invoices.Count > 0;

            return invoices.Count > 0;
        }

        private SearchResultPacket GetPacket(VInvoice invoice)
        {
            var html = new StringBuilder();
            var items = ServiceLocator.InvoiceSearch.GetInvoiceItems(invoice.InvoiceIdentifier);
            if (items.Count > 0)
            {
                html.Append("<ul>");
                foreach (var item in items)
                    html.Append($"<li>{item.ItemDescription}</li>");
                html.Append("</ul>");
            }

            var hasRegistrationItem = items.Any(x => x.ItemDescription.Contains("activity registration", StringComparison.OrdinalIgnoreCase));

            return new SearchResultPacket
            {
                InvoiceAmount = invoice.InvoiceAmount,
                InvoiceDrafted = GetDateString(invoice.InvoiceDrafted),
                InvoiceIdentifier = invoice.InvoiceIdentifier,
                InvoicePaid = GetDateString(invoice.InvoicePaid),
                InvoiceSubmitted = GetDateString(invoice.InvoiceSubmitted),
                InvoiceStatus = invoice.InvoiceStatus,
                ItemsHtml = html.ToString(),
                HasRegistrationItem = hasRegistrationItem
            };
        }

        private void LoadData()
        {
            PageHelper.AutoBindHeader(this);

            PortalMaster.RenderHelpContent(null);

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

            var hasUnpaidInvoices = LoadUnpaidInvoices();
            var hasPaidInvoices = LoadPaidInvoices();

            MainAccordion.Visible = hasUnpaidInvoices || hasPaidInvoices;

            if (!hasUnpaidInvoices && !hasPaidInvoices)
                StatusAlert.AddMessage(AlertType.Warning, Translate("There are no invoices for you"));
        }

        private void Repeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!RepeaterHelper.IsContentItem(e))
                return;

            var printInvoiceButton = (UxButton)e.Item.FindControl("PrintInvoiceButton");
            printInvoiceButton.Click += PrintInvoiceButton_Click;
        }

        private void Repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!RepeaterHelper.IsContentItem(e))
                return;

            var packet = (SearchResultPacket)e.Item.DataItem;

            var invoiceIdentifier = packet.InvoiceIdentifier;

            var canPrintReport = packet.InvoiceAmount < 0
                || (packet.InvoiceStatus == "Paid" && ServiceLocator.InvoiceSearch.InvoiceIsPaid(invoiceIdentifier));

            var printInvoiceButton = (UxButton)e.Item.FindControl("PrintInvoiceButton");
            printInvoiceButton.Visible = canPrintReport;

            var payButton = (UxButton)e.Item.FindControl("PayInvoiceButton");
            payButton.Visible = (packet.InvoiceStatus == "Submitted" || packet.InvoiceStatus == "PaymentFailed")
                && !packet.HasRegistrationItem;

            var invoiceIdentifierLiteral = (Literal)e.Item.FindControl("InvoiceIdentifier");
            invoiceIdentifierLiteral.Text = invoiceIdentifier.ToString();
        }

        private void PrintInvoiceButton_Click(object sender, EventArgs e)
        {
            var invocieIdentifierText = ((Literal)((UxButton)sender).NamingContainer.FindControl("InvoiceIdentifier")).Text;
            if (Guid.TryParse(invocieIdentifierText, out var invoiceIdentifier))
            {
                var (data, fileName) = InvoiceEventReport.PrintByInvoice(invoiceIdentifier, InvoiceEventReportType.Invoice);
                if (data != null)
                    Response.SendFile(fileName, "pdf", data);
            }
        }

        protected string GetInvoiceStatus(object value)
        {
            var status = (string)value;

            return ProductHelper.GetInvoiceStatus(status);
        }

        protected string GetDateString(DateTimeOffset? date)
        {
            if (date != null && date.HasValue)
            {
                return TimeZones.FormatDateOnly(date.Value, User.TimeZone, CultureInfo.GetCultureInfo(CurrentSessionState.Identity.Language));
            }

            return null;
        }
    }
}