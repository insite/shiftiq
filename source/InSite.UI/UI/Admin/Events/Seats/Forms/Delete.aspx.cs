using System;

using InSite.Application.Events.Read;
using InSite.Application.Events.Write;
using InSite.Application.Registrations.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Events.Seats.Forms
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters, IOverrideWebRouteParent
    {
        private Guid? SeatIdentifier => Guid.TryParse(Request["id"], out var result) ? result : (Guid?)null;

        string DefaultParameters => $"event={EventIdentifier}&panel=seats";

        private Guid? EventIdentifier => Guid.TryParse(Request["event"], out var result) ? result : (Guid?)null;

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/outline") ? $"{DefaultParameters}" : GetParentLinkParameters(parent, null);


        IWebRoute IOverrideWebRouteParent.GetParent() =>
            GetParent();

        private string BackTo => Request["back"];

        private const string SearchClassesUrl = "/ui/admin/events/classes/search";
        private const string SearchExamsUrl = "/ui/admin/events/exams/search";
        private const string EventsToolkitUrl = "/ui/admin/events/home";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            DeleteButton.Click += OnConfirmed;
        }

        private void OnConfirmed(object sender, EventArgs e)
        {
            if (DeleteConfirmationCheckbox.Checked)
            {
                var seat = ServiceLocator.EventSearch.GetSeat(SeatIdentifier.Value);

                if (RegistrationsCount.Text != "0")
                {
                    var filter = new QRegistrationFilter { SeatIdentifier = seat.SeatIdentifier };
                    foreach (var registrationId in ServiceLocator.RegistrationSearch.GetRegistrationIdentifiers(filter))
                        ServiceLocator.SendCommand(new InSite.Application.Registrations.Write.DeleteRegistration(registrationId, false));
                }

                ServiceLocator.SendCommand(new DeleteSeat(seat.EventIdentifier, seat.SeatIdentifier));
                HttpResponseHelper.Redirect(CancelButton.NavigateUrl);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write))
                ReturnToSearch(GetParentUrl(""));

            var seat = SeatIdentifier.HasValue ? ServiceLocator.EventSearch.GetSeat(SeatIdentifier.Value) : null;
            if (seat == null || seat.Event.OrganizationIdentifier != Organization.OrganizationIdentifier)
            {
                ReturnToSearch(GetParentUrl(""));
                return;
            }

            BindSeat(seat);

            if (BackTo == null)
                CancelButton.NavigateUrl = EventsToolkitUrl;
            else
                CancelButton.NavigateUrl = GetParentUrl(DefaultParameters);
        }

        private void BindSeat(QSeat seat)
        {
            EventTitle.Text = $"<a href=\"/ui/admin/events/classes/outline?event={seat.Event.EventIdentifier}\">{seat.Event.EventTitle}</a>";

            var seatUrl = ReturnUrlHelper.GetRedirectUrl($"/ui/admin/events/seats/edit?id={seat.SeatIdentifier}");
            SeatName.Text = $"<a href=\"{seatUrl}\">{seat.SeatTitle}</a>";

            PageHelper.AutoBindHeader(
                Page,
                qualifier: seat.SeatTitle);

            var content = ContentSeat.Deserialize(seat.Content);
            SeatDescription.Text = content.Description.Default.HasValue() ? content.Description.Default : "None";
            SeatAgreement.Text = content.AddOrGet("Agreement").Default ?? "None";

            int registrationCount = ServiceLocator.RegistrationSearch.CountRegistrations(new InSite.Application.Registrations.Read.QRegistrationFilter { SeatIdentifier = seat.SeatIdentifier });
            RegistrationsCount.Text = registrationCount.ToString();

            DeleteConfirmationCheckbox.Text = registrationCount == 0 ? "Delete Seat" : "Delete Seat and Registrations";
            NoDelete.Visible = registrationCount != 0;
        }

        private void ReturnToSearch(string parentUrl)
        {
            if (parentUrl.HasNoValue())
                HttpResponseHelper.Redirect(EventsToolkitUrl);

            if (parentUrl.Contains("events/classes", StringComparison.OrdinalIgnoreCase))
                HttpResponseHelper.Redirect(SearchClassesUrl);
            else if (parentUrl.Contains("events/exams", StringComparison.OrdinalIgnoreCase))
                HttpResponseHelper.Redirect(SearchExamsUrl);
            else
                HttpResponseHelper.Redirect(EventsToolkitUrl);
        }

    }
}