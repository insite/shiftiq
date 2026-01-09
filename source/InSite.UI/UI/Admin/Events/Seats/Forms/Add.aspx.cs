using System;
using System.Collections.Generic;

using InSite.Application.Events.Read;
using InSite.Application.Events.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Events.Seats.Forms
{
    public partial class Add : AdminBasePage, IHasParentLinkParameters, IOverrideWebRouteParent
    {
        private const string SearchClassesUrl = "/ui/admin/events/classes/search";
        private const string SearchExamsUrl = "/ui/admin/events/exams/search";
        private const string EventsToolkitUrl = "/ui/admin/events/home";

        private Guid? EventIdentifier => Guid.TryParse(Request["event"], out var result) ? result : (Guid?)null;

        string DefaultParameters => $"event={EventIdentifier}&panel=seats";

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/outline") ? $"{DefaultParameters}" : GetParentLinkParameters(parent, null);

        IWebRoute IOverrideWebRouteParent.GetParent() =>
            GetParent();

        private List<SeatConfiguration.Price> Prices
        {
            get => (List<SeatConfiguration.Price>)ViewState[nameof(Prices)];
            set => ViewState[nameof(Prices)] = value;
        }

        private List<string> BillingCustomers
        {
            get => (List<string>)ViewState[nameof(BillingCustomers)];
            set => ViewState[nameof(BillingCustomers)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += (s, a) => { if (Page.IsValid) Save(); };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!CanCreate)
                ReturnToSearch(GetParentUrl("")); 

            var @event = EventIdentifier.HasValue ? ServiceLocator.EventSearch.GetEvent(EventIdentifier.Value, x => x.VenueLocation) : null;
            if (@event == null || @event.OrganizationIdentifier != Organization.OrganizationIdentifier)
                ReturnToSearch(GetParentUrl(""));

            PageHelper.AutoBindHeader(
                Page,
                qualifier: $"{@event.EventTitle} <span class='form-text'>scheduled {@event.EventScheduledStart.FormatDateOnly(User.TimeZone)}</span>");

            Detail.SetDefaultInputValues();

            CancelButton.NavigateUrl = GetParentUrl(DefaultParameters);
        }

        private void Save()
        {
            var seat = new QSeat();

            Detail.GetInputValues(seat);

            var seatId = UniqueIdentifier.Create();

            var command = new AddSeat(EventIdentifier.Value, seatId, seat.Configuration, seat.Content, seat.IsAvailable, seat.IsTaxable, seat.OrderSequence, seat.SeatTitle); ;
            ServiceLocator.SendCommand(command);

            HttpResponseHelper.Redirect(GetParentUrl(DefaultParameters));
        }

        private void ReturnToSearch(string parentUrl)
        {
            if(parentUrl.HasNoValue())
                HttpResponseHelper.Redirect(EventsToolkitUrl);

            if (parentUrl.Contains("events/classes", StringComparison.OrdinalIgnoreCase))
                HttpResponseHelper.Redirect(SearchClassesUrl);
            else if(parentUrl.Contains("events/exams", StringComparison.OrdinalIgnoreCase))
                HttpResponseHelper.Redirect(SearchExamsUrl);
            else
                HttpResponseHelper.Redirect(EventsToolkitUrl);
        }
    }
}