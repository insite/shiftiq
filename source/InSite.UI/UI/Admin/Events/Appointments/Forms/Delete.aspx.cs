using System;

using InSite.Application.Events.Read;
using InSite.Application.Events.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Events.Appointments.Forms
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        private const string OutlineUrl = "/ui/admin/events/appointments/outline";
        private const string SearchUrl = "/ui/admin/events/appointments/search";

        private QEvent _event;

        private Guid? EventIdentifier => Guid.TryParse(Request["event"], out var result) ? result : (Guid?)null;

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/outline") ? $"event={EventIdentifier}" : null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteCheck.AutoPostBack = true;
            DeleteCheck.CheckedChanged += (x, y) => { DeleteButton.Enabled = DeleteCheck.Checked; };
            DeleteButton.Click += OnConfirmed;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Identity.IsGranted(Route.ToolkitName, PermissionOperation.Delete))
                NavigateToSearch();

            if (!ValidateEvent())
                NavigateToSearch();

            BindNavigation();
            BindEvent();
            BindImpact();
        }

        private void BindNavigation()
        {
            CancelButton.NavigateUrl = $"{OutlineUrl}?event={EventIdentifier.Value}";

            PageHelper.AutoBindHeader(this, null, _event.EventTitle);
        }

        private void BindEvent()
        {
            AppointmentDetails.BindAppointment(_event, true);
        }

        private void BindImpact()
        {
            var filter = new Application.Registrations.Read.QRegistrationFilter { EventIdentifier = EventIdentifier.Value };
            var count = ServiceLocator.RegistrationSearch.CountRegistrations(filter);
        }

        private void OnConfirmed(object sender, EventArgs e)
        {
            if (EventIdentifier.HasValue)
                ServiceLocator.SendCommand(new DeleteEvent(EventIdentifier.Value));

            NavigateToSearch();
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