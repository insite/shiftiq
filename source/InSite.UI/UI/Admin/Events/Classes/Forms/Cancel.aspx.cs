using System;

using InSite.Application.Events.Write;
using InSite.Application.Registrations.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Events.Classes.Forms
{
    public partial class Cancel : AdminBasePage, IHasParentLinkParameters
    {
        private const string SearchUrl = "/ui/admin/events/classes/search";

        private Guid? EventIdentifier => Guid.TryParse(Request["event"], out var result) ? result : (Guid?)null;

        private string OutlineUrl
            => $"/ui/admin/events/classes/outline?event={EventIdentifier}&panel=class";

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/outline") ? $"event={EventIdentifier}" : null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ConfirmButton.Click += (s, a) => Confirm();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!CanEdit)
                HttpResponseHelper.Redirect(SearchUrl);

            Open();
        }

        private void Open()
        {
            var @event = EventIdentifier.HasValue
                ? ServiceLocator.EventSearch.GetEvent(EventIdentifier.Value, x => x.VenueLocation)
                : null;

            if (@event == null || @event.OrganizationIdentifier != Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect(SearchUrl);

            CancelButton.NavigateUrl = OutlineUrl;

            PageHelper.AutoBindHeader(
                Page,
                qualifier: $"{@event.EventTitle} <span class='form-text'>scheduled to start {@event.EventScheduledStart.FormatDateOnly(User.TimeZone)}</span>");

            SummaryInfo.Bind(@event);
            LocationInfo.Bind(@event);

            var registrationCount = ServiceLocator.RegistrationSearch.CountRegistrations(new QRegistrationFilter
            {
                EventIdentifier = @event.EventIdentifier
            });

            CancelRegistrationsHelp.InnerText = $"This event has {registrationCount} registrations. Do you want to cancel these registrations?";
        }

        private void Confirm()
        {
            if (!Page.IsValid)
                return;

            ServiceLocator.SendCommand(new CancelEvent(EventIdentifier.Value, Reason.Text, CancelRegistrations.Checked));

            HttpResponseHelper.Redirect(OutlineUrl);
        }
    }
}