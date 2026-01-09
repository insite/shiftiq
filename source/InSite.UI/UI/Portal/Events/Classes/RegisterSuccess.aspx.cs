using System;

using InSite.Admin.Invoices.Controls;
using InSite.Application.Registrations.Read;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Events.Classes
{
    public partial class RegisterSuccess : PortalBasePage
    {
        private Guid RegistrationIdentifier => Guid.TryParse(Request["registration"], out var id) ? id : Guid.Empty;

        private bool IsAdminOutline => Request["adminoutline"] == "1";

        private QRegistration _registration;
        private QRegistration Registration
        {
            get
            {
                if (_registration == null)
                    _registration = ServiceLocator.RegistrationSearch.GetRegistration(RegistrationIdentifier, x => x.Event);

                return _registration;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PrintReceiptButton.Click += PrintReceiptButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Registration == null || Registration.Event.OrganizationIdentifier != Organization.OrganizationIdentifier)
            {
                HttpResponseHelper.Redirect("/ui/portal/events/classes/search");
                return;
            }

            PageHelper.AutoBindHeader(this);

            var subtitle = $"{Registration.Event.EventScheduledStart.FormatDateOnly(User.TimeZone)}";
            if (Registration.Event.EventScheduledEnd.HasValue && Registration.Event.EventScheduledEnd.Value.Date != Registration.Event.EventScheduledStart.Date)
                subtitle += $" - {Registration.Event.EventScheduledEnd.Value.FormatDateOnly(User.TimeZone)}";

            PortalMaster.Breadcrumbs.BindTitleAndSubtitleNoTranslate(Registration.Event.EventTitle, subtitle);

            PortalMaster.SidebarVisible(false);

            RenderBreadcrumb(GetOutlineLinkArgs());

            var eventContent = ContentEventClass.Deserialize(Registration.Event.Content);
            var completion = eventContent.Get(EventInstructionType.Completion.GetName())?.Default;

            RegistrationCompletion.Text = !string.IsNullOrEmpty(completion) ? Markdown.ToHtml(completion) : null;

            default_message.Visible = (RegistrationCompletion.Text == "");

            ReturnButton.NavigateUrl = "/ui/portal/events/classes/search";
        }

        private void PrintReceiptButton_Click(object sender, EventArgs e)
        {
            var data = InvoiceEventReport.PrintByRegistration(RegistrationIdentifier, InvoiceEventReportType.Receipt);

            Response.SendFile("Receipt", "pdf", data);
        }

        private string GetNavigateBackLink()
            => !IsAdminOutline ? GetOutlineLink() : $"/ui/admin/events/classes/outline?event={Registration?.EventIdentifier}&panel=registrations";

        private string GetOutlineLink()
        {
            return $"/ui/portal/events/classes/outline" + GetOutlineLinkArgs();
        }

        private string GetOutlineLinkArgs()
        {
            return $"?event={Registration?.EventIdentifier}";
        }

    }
}