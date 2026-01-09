using System;
using System.Threading.Tasks;

using Shift.Common.Timeline.Exceptions;

using InSite.Application.Registrations.Read;
using InSite.Application.Registrations.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Events.Registrations.Exams.Forms
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters, IOverrideWebRouteParent
    {
        private Guid? RegistrationIdentifier => Guid.TryParse(Request["id"], out var result) ? result : (Guid?)null;

        private Guid? ClassIdentifier => Guid.TryParse(Request["event"], out var result) ? result : (Guid?)null;

        private string OutlineUrl
            => $"/ui/admin/events/classes/outline?event={ClassIdentifier}&panel=registrations";

        private string SearchUrl
            => "/ui/admin/registrations/classes/search";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            DeleteButton.Click += OnConfirmed;
        }

        private void OnConfirmed(object sender, EventArgs e)
        {
            try
            {
                ServiceLocator.SendCommand(new DeleteRegistration(RegistrationIdentifier.Value, false));
            }
            catch (UnhandledCommandException ex)
            {
                if (ex.InnerException is TaskCanceledException)
                {
                    // Ignore this error because it comes from DirectAccess
                }
                else
                    throw;
            }

            HttpResponseHelper.Redirect(CancelButton.NavigateUrl);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write))
                RedirectToSearch();

            var registration = RegistrationIdentifier.HasValue
                ? ServiceLocator.RegistrationSearch.GetRegistration(RegistrationIdentifier.Value, x => x.Event, x => x.Candidate)
                : null;

            if (registration == null || registration.Event.OrganizationIdentifier != Organization.OrganizationIdentifier)
            {
                RedirectToSearch();
                return;
            }

            BindRegistration(registration);

            if (ClassIdentifier == null)
                CancelButton.NavigateUrl = GetParentUrl(null);
            else
                CancelButton.NavigateUrl = OutlineUrl;
        }

        private void RedirectToSearch()
            => HttpResponseHelper.Redirect(SearchUrl, true);

        private void BindRegistration(QRegistration registration)
        {
            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{registration.Event.EventTitle} <span class=form-text>for </span> {registration.Candidate.UserFullName}");

            var venue = registration.Event.VenueLocationIdentifier.HasValue
                ? ServiceLocator.ContactSearch.GetGroup(registration.Event.VenueLocationIdentifier.Value)
                : null;

            EventTitle.Text = registration.Event.EventTitle;
            EventLink.HRef = $"/ui/admin/events/exams/outline?event={registration.Event.EventIdentifier}";

            FullName.Text = $"<a href=\"/ui/admin/contacts/people/edit?contact={registration.Candidate.UserIdentifier}\">{registration.Candidate.UserFullName}</a>";
            Email.Text = $"<a href='mailto:{registration.Candidate.UserEmail}'>{registration.Candidate.UserEmail}</a>";

            RegistrationSequence.Text = registration.RegistrationSequence.HasValue ? registration.RegistrationSequence.ToString() : "None";
            RegistrationRequestedOn.Text = registration.RegistrationRequestedOn.FormatDateOnly(User.TimeZone, nullValue: "None");
            RegistrationApprovalStatus.Text = registration.ApprovalStatus ?? "None";
            RegistrationAttendanceStatus.Text = registration.AttendanceStatus ?? "None";
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return GetParentLinkParameters(parent, null);
        }

        IWebRoute IOverrideWebRouteParent.GetParent() =>
            GetParent();
    }
}