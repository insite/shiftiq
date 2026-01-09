using System;

using InSite.Application.Events.Read;
using InSite.Application.Events.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Events.Classes.Forms
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        private QEvent _event;

        private Guid? EventIdentifier => Guid.TryParse(Request["event"], out var result) ? result : (Guid?)null;

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/outline") ? $"event={EventIdentifier}" : null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += OnConfirmed;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanDelete)
                NavigateToSearch();

            if (ValidateEvent())
                BindEvent();
            else
                NavigateToSearch();
        }

        private void BindEvent()
        {
            CancelButton.NavigateUrl = $"/ui/admin/events/classes/outline?event={EventIdentifier.Value}";

            PageHelper.AutoBindHeader(this, null, $"{_event.EventTitle} <span class='form-text'>scheduled {_event.EventScheduledStart.FormatDateOnly(User.TimeZone)}</span>");

            var registrationCount = ServiceLocator.RegistrationSearch.CountRegistrations(new InSite.Application.Registrations.Read.QRegistrationFilter { EventIdentifier = _event.EventIdentifier });
            RegistrationCount.Text = registrationCount.ToString();

            var seatCount = ServiceLocator.EventSearch.CountSeats(new QSeatFilter { EventIdentifier = _event.EventIdentifier });
            SeatCount.Text = seatCount.ToString();

            var gradebooksCount = ServiceLocator.RecordSearch.CountGradebooks(new QGradebookFilter { GradebookEventIdentifier = _event.EventIdentifier });
            GradebookCount.Text = gradebooksCount.ToString();

            var commentCount = ServiceLocator.EventSearch.CountComments(new QEventCommentFilter { EventIdentifier = _event.EventIdentifier });
            CommentsCount.Text = commentCount.ToString();

            SummaryInfo.Bind(_event);
            LocationInfo.Bind(_event);
        }

        private void OnConfirmed(object sender, EventArgs e)
        {
            if (EventIdentifier.HasValue)
                ServiceLocator.SendCommand(new DeleteEvent(EventIdentifier.Value));
            
            NavigateToSearch();
        }

        private void NavigateToSearch()
            => HttpResponseHelper.Redirect("/ui/admin/events/classes/search", true);

        private bool ValidateEvent()
        {
            _event = EventIdentifier.HasValue ? ServiceLocator.EventSearch.GetEvent(EventIdentifier.Value, x => x.VenueLocation) : null;
            return _event != null && _event.OrganizationIdentifier == Organization.OrganizationIdentifier;
        }
    }
}