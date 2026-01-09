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
    }

    public partial class Invoices : PortalBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            InvoicesRepeater.ItemDataBound += Repeater_ItemDataBound;
            InvoicesRepeater.ItemCreated += Repeater_ItemCreated;

            ReceiptsRepeater.ItemDataBound += Repeater_ItemDataBound;
            ReceiptsRepeater.ItemCreated += Repeater_ItemCreated;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        private bool LoadInvoices()
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
                .Select(x => new SearchResultPacket()
                {
                    InvoiceAmount = x.InvoiceAmount,
                    InvoiceDrafted = GetDateString(x.InvoiceDrafted),
                    InvoiceIdentifier = x.InvoiceIdentifier,
                    InvoicePaid = GetDateString(x.InvoicePaid),
                    InvoiceSubmitted = GetDateString(x.InvoiceSubmitted),
                    InvoiceStatus = x.InvoiceStatus,
                    ItemsHtml = GetInvoiceItemsHtml(x.InvoiceIdentifier)
                })
                .ToList();

            if (invoices.Count > 0)
            {
                InvoicesRepeater.DataSource = invoices;
                InvoicesRepeater.DataBind();
            }
            else
            {
                StatusAlert.AddMessage(AlertType.Warning, Translate("There are no invoices for you"));
            }

            return invoices.Count > 0;
        }

        private bool LoadReceipts()
        {
            var receiptFilter = new VInvoiceFilter
            {
                OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier,
                CustomerIdentifier = User.UserIdentifier,
                InvoiceStatus = InvoiceStatus.Paid.ToString()
            };

            var receipts = ServiceLocator
               .InvoiceSearch
               .GetInvoices(receiptFilter)
               .Select(x => new SearchResultPacket()
               {
                   InvoiceAmount = x.InvoiceAmount,
                   InvoiceDrafted = GetDateString(x.InvoiceDrafted),
                   InvoiceIdentifier = x.InvoiceIdentifier,
                   InvoicePaid = GetDateString(x.InvoicePaid),
                   InvoiceSubmitted = GetDateString(x.InvoiceSubmitted),
                   InvoiceStatus = x.InvoiceStatus,
                   ItemsHtml = GetInvoiceItemsHtml(x.InvoiceIdentifier)
               })
               .ToList();

            if (receipts.Count > 0)
            {
                ReceiptsRepeater.DataSource = receipts;
                ReceiptsRepeater.DataBind();
            }
            else
            {
                StatusAlert.AddMessage(AlertType.Warning, Translate("There are no receipts for you"));
            }

            return receipts.Count > 0;
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

            var invoicesVisible = LoadInvoices();
            var receiptsVisible = LoadReceipts();
            MainAccordion.Visible = invoicesVisible || receiptsVisible;
        }

        private string GetInvoiceItemsHtml(Guid invoice)
        {
            var items = ServiceLocator.InvoiceSearch.GetInvoiceItems(invoice);
            if (items.Count == 0)
                return null;

            var html = new StringBuilder();
            html.Append("<ul>");
            foreach (var item in items)
                html.Append($"<li>{item.ItemDescription}</li>");
            html.Append("</ul>");
            return html.ToString();
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