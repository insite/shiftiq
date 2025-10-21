using System;
using System.Web.UI.WebControls;

using InSite.Application.Events.Read;
using InSite.Application.Events.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Events.Appointments.Forms
{
    public partial class Reschedule : AdminBasePage, IHasParentLinkParameters
    {
        private QEvent _event;

        private Guid? EventIdentifier => Guid.TryParse(Request["event"], out var result) ? result : (Guid?)null;

        private string OutlineUrl
            => $"/ui/admin/events/appointments/outline?event={EventIdentifier.Value}";

        private string SearchUrl
            => $"/ui/admin/events/appointments/search";

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/outline") ? $"event={EventIdentifier}" : null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            SaveButton.Click += OnConfirmed;

            EventScheduledEndValidator.ServerValidate += EventScheduledEndValidator_ServerValidate;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write))
                NavigateToSearch();

            if (!ValidateEvent())
                NavigateToSearch();

            if (!IsPostBack)
                BindEvent();
        }

        private void BindEvent()
        {
            BackButton.NavigateUrl = OutlineUrl;

            PageHelper.AutoBindHeader(
                this, 
                qualifier: $"{_event.EventTitle} <span class='form-text'>scheduled {_event.EventScheduledStart.FormatDateOnly(User.TimeZone)}</span>");

            EventTitle.Text = _event.EventTitle;
            EventScheduledStart.Value = _event.EventScheduledStart;
            EventScheduledEnd.Value = _event.EventScheduledEnd;

        }

        private void OnConfirmed(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (EventIdentifier.HasValue)
            {
                if (_event.EventScheduledStart != EventScheduledStart.Value || _event.EventScheduledEnd != EventScheduledEnd.Value)
                    ServiceLocator.SendCommand(new RescheduleEvent(_event.EventIdentifier, EventScheduledStart.Value.Value, EventScheduledEnd.Value.Value));
            }

            HttpResponseHelper.Redirect(BackButton.NavigateUrl);
        }

        private void EventScheduledEndValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (EventScheduledEnd.Value?.UtcDateTime <= EventScheduledStart.Value?.UtcDateTime)
            {
                args.IsValid = false;
                EventScheduledEndValidator.ErrorMessage = "End must be later than Start";
            }
        }

        private void NavigateToSearch()
            => HttpResponseHelper.Redirect(SearchUrl, true);

        private bool ValidateEvent()
        {
            _event = EventIdentifier.HasValue ? ServiceLocator.EventSearch.GetEvent(EventIdentifier.Value, x => x.VenueLocation) : null;
            return _event != null && _event.OrganizationIdentifier == Organization.OrganizationIdentifier;
        }

    }
}
