using System;

using InSite.Application.Events.Read;
using InSite.Application.Events.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Events.Exams.Forms
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        private QEvent _event;

        private Guid? EventIdentifier
            => Guid.TryParse(Request["event"], out var result) ? result : (Guid?)null;

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => $"event={EventIdentifier}";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            DeleteButton.Click += OnConfirmed;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Identity.IsGranted(Route.ToolkitName, PermissionOperation.Delete))
                NavigateToSearch();

            if (ValidateEvent())
                BindEvent();
            else
                NavigateToSearch();
        }

        private void BindEvent()
        {
            CancelButton.NavigateUrl = $"/ui/admin/events/exams/outline?event={EventIdentifier.Value}";

            PageHelper.AutoBindHeader(this, null, $"{_event.EventTitle} <span class='form-text'>scheduled {_event.EventScheduledStart.FormatDateOnly(User.TimeZone)}</span>");

            EventFormat.Text = _event.EventFormat;
            EventSchedulingStatus.Text = _event.EventSchedulingStatus;
            EventClassCode.Text = _event.EventClassCode;

            //1 - Physical Address
            ClassVenueInfo.Bind(_event.EventIdentifier, _event.VenueLocation);

            VenueRoomField.Visible = _event.VenueRoom.HasValue();
            VenueRoom.Text = _event.VenueRoom;

            var registrationsCount = ServiceLocator.RegistrationSearch.CountRegistrations(new InSite.Application.Registrations.Read.QRegistrationFilter { EventIdentifier = _event.EventIdentifier });
            RegistrationCount.Text = registrationsCount.ToString();

            EventTime.Text = _event.EventDate + " " + _event.EventTime;
        }

        private void OnConfirmed(object sender, EventArgs e)
        {
            if (EventIdentifier.HasValue)
                ServiceLocator.SendCommand(new DeleteEvent(EventIdentifier.Value));
            NavigateToSearch();
        }

        private void NavigateToSearch()
            => HttpResponseHelper.Redirect("/ui/admin/events/exams/search", true);

        private bool ValidateEvent()
        {
            _event = EventIdentifier.HasValue ? ServiceLocator.EventSearch.GetEvent(EventIdentifier.Value, x => x.VenueLocation) : null;
            return _event != null && _event.OrganizationIdentifier == Organization.OrganizationIdentifier;
        }
    }
}