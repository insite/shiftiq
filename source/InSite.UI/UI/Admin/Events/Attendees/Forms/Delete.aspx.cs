using System;

using InSite.Application.Events.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Events.Attendees.Forms
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? EventIdentifier => Guid.TryParse(Request.QueryString["event"], out Guid value) ? value : (Guid?)null;
        private Guid? AttendeeIdentifier => Guid.TryParse(Request["contact"], out var result) ? result : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
            CancelButton.NavigateUrl = $"/ui/admin/events/exams/outline?event={EventIdentifier}&panel=contacts";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Identity.IsGranted(Route.ToolkitName, PermissionOperation.Delete))
                RedirectToParent();

            if (!IsPostBack)
            {
                if (!EventIdentifier.HasValue)
                    HttpResponseHelper.Redirect($"/ui/admin/events/exams/search");

                if (!AttendeeIdentifier.HasValue)
                    RedirectToParent();

                LoadControls();
            }


        }

        private void LoadControls()
        {
            var attendee = ServiceLocator.EventSearch.GetAttendee(EventIdentifier.Value, AttendeeIdentifier.Value, x => x.Event);
            if (attendee == null || attendee.Event.OrganizationIdentifier != Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect($"/ui/admin/events/exams/search");

            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{attendee.Event.EventTitle} <span class='form-text'>scheduled {attendee.Event.EventScheduledStart.FormatDateOnly(User.TimeZone)}</span>");

            AttendeeRole.Text = attendee.AttendeeRole;
            Assigned.Text = attendee.Assigned.Format(User.TimeZone, nullValue: "None");

            var person = ServiceLocator.PersonSearch.GetPerson(AttendeeIdentifier.Value, Organization.Key, x => x.User);

            PersonDetail.BindPerson(person, User.TimeZone);
            EventTitle.Text = $"<a href=\"/ui/admin/events/exams/outline?event={EventIdentifier.Value}\">{attendee.Event.EventTitle}</a>";
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            ServiceLocator.SendCommand(new RemoveEventAttendee(EventIdentifier.Value, AttendeeIdentifier.Value));

            RedirectToParent();
        }

        private void RedirectToParent() =>
            HttpResponseHelper.Redirect($"/ui/admin/events/exams/outline?event={EventIdentifier}&panel=contacts");

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/exams/outline")
                ? $"event={EventIdentifier}&panel=contacts"
                : null;
        }
    }
}