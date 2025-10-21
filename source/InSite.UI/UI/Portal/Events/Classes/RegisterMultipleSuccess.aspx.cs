using System;
using System.Linq;

using InSite.Admin.Invoices.Controls;
using InSite.Application.Payments.Read;
using InSite.Application.Registrations.Read;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Events.Classes
{
    public partial class RegisterMultipleSuccess : PortalBasePage
    {
        private Guid InvoiceIdentifier => Guid.TryParse(Request["invoice"], out var id) ? id : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PrintReceiptButton.Click += PrintReceiptButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        private void PrintReceiptButton_Click(object sender, EventArgs e)
        {
            var (data, fileName) = InvoiceEventReport.PrintByInvoice(InvoiceIdentifier, InvoiceEventReportType.Receipt);
            if (data != null)
                Response.SendFile(fileName, "pdf", data);
        }

        private void LoadData()
        {
            var paymentFilter = new QPaymentFilter { InvoiceIdentifier = InvoiceIdentifier, PaymentStatus = "Completed" };
            var payment = ServiceLocator.PaymentSearch.GetPayments(paymentFilter).FirstOrDefault();
            if (payment == null || payment.OrganizationIdentifier != Organization.Identifier)
            {
                HttpResponseHelper.Redirect("/ui/portal/events/classes/search");
                return;
            }

            PageHelper.AutoBindHeader(this);

            var registrations = ServiceLocator.RegistrationSearch.GetRegistrations(
                new QRegistrationFilter { PaymentIdentifier = payment.PaymentIdentifier },
                x => x.Event, x => x.Candidate
            );
            var @event = registrations[0].Event;

            var subtitle = $"{@event.EventScheduledStart.FormatDateOnly(User.TimeZone)}";
            if (@event.EventScheduledEnd.HasValue && @event.EventScheduledEnd.Value.Date != @event.EventScheduledStart.Date)
                subtitle += $" - {@event.EventScheduledEnd.Value.FormatDateOnly(User.TimeZone)}";

            PortalMaster.Breadcrumbs.BindTitleAndSubtitleNoTranslate(@event.EventTitle, subtitle);

            PortalMaster.SidebarVisible(false);

            RenderBreadcrumb($"?event={@event.EventIdentifier}");

            var eventContent = ContentEventClass.Deserialize(@event.Content);
            var completion = eventContent.Get(EventInstructionType.Completion.GetName())?.Default;
            var completionText = !string.IsNullOrEmpty(completion) ? Markdown.ToHtml(completion) : null;

            RegistrationCompletion.Text = completionText;
            DefaultMessage.Visible = !string.IsNullOrEmpty(completionText);

            RegistrationRepeater.DataSource = registrations;
            RegistrationRepeater.DataBind();

            ReturnButton.NavigateUrl = $"/ui/portal/events/classes/outline?event={@event.EventIdentifier}";
        }
    }
}