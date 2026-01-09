using System;
using System.Linq;

using InSite.Application.Registrations.Read;
using InSite.Application.Registrations.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Events.Registrations.Forms
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
            ServiceLocator.SendCommand(new DeleteRegistration(RegistrationIdentifier.Value, false));
            HttpResponseHelper.Redirect(CancelButton.NavigateUrl);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write))
                RedirectToSearch();

            var registration = RegistrationIdentifier.HasValue
                ? ServiceLocator.RegistrationSearch.GetRegistration(RegistrationIdentifier.Value, x => x.Event)
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
            var candidate = UserSearch.Bind(registration.CandidateIdentifier, u => new
            {
                u.FullName,
                u.Email,
                IsPerson = u.Persons.Any(p => p.OrganizationIdentifier == Organization.Identifier)
            });

            var title = registration.Event.EventTitle;
            if (candidate != null)
                title += $" <span class='fs-3 text-body-secondary'>for</span> {candidate.FullName}";

            PageHelper.AutoBindHeader(this, null, title);

            var venue = registration.Event.VenueLocationIdentifier.HasValue
                ? ServiceLocator.ContactSearch.GetGroup(registration.Event.VenueLocationIdentifier.Value)
                : null;

            var achievement = registration.Event.AchievementIdentifier.HasValue
                ? ServiceLocator.AchievementSearch.GetAchievement(registration.Event.AchievementIdentifier.Value)
                : null;

            EventTitle.Text = registration.Event.EventTitle;
            EventLink.HRef = $"/ui/admin/events/classes/outline?event={registration.Event.EventIdentifier}";
            AchievementTitle.Text = achievement != null ? $"<a href=\"/ui/admin/records/achievements/outline?id={achievement.AchievementIdentifier}\">{achievement.AchievementTitle}</a>" : "None";

            if (candidate == null)
            {
                FullName.Text = "<strong>User Not Found</strong>";
                Email.Text = null;
            }
            else if (candidate.IsPerson)
            {
                FullName.Text = $"<a href=\"/ui/admin/contacts/people/edit?contact={registration.CandidateIdentifier}\">{candidate.FullName}</a>";
                Email.Text = $"<a href='mailto:{candidate.Email}'>{candidate.Email}</a>";
            }
            else
            {
                FullName.Text = candidate.FullName;
                Email.Text = candidate.Email;
            }

            RegistrationSequence.Text = registration.RegistrationSequence.HasValue ? registration.RegistrationSequence.ToString() : "None";
            RegistrationRequestedOn.Text = registration.RegistrationRequestedOn.FormatDateOnly(User.TimeZone, nullValue: "None");
            RegistrationApprovalStatus.Text = registration.ApprovalStatus ?? "None";
            RegistrationAttendanceStatus.Text = registration.AttendanceStatus ?? "None";
            RegistrationFee.Text = $"{registration.RegistrationFee:c2}";
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return GetParentLinkParameters(parent, null);
        }

        IWebRoute IOverrideWebRouteParent.GetParent() =>
            GetParent();
    }
}