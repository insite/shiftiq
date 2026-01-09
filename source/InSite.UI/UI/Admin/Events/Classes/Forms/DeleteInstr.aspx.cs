using System;

using InSite.Application.Events.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.UI.Admin.Events.Classes.Forms
{
    public partial class DeleteInstr : AdminBasePage, IHasParentLinkParameters
    {
        private Guid EventID => Guid.TryParse(Request["event"], out var result) ? result : Guid.Empty;
        private Guid InstructorID => Guid.TryParse(Request["instructor"], out var result) ? result : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
            CancelButton.NavigateUrl = $"/ui/admin/events/classes/outline?event={EventID}&panel=class";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var instructor = ServiceLocator.PersonSearch.GetPerson(InstructorID, Organization.Key, x => x.User);
            var @event = ServiceLocator.EventSearch.GetEvent(EventID, x => x.VenueLocation);

            PersonDetail.BindPerson(instructor, User.TimeZone);

            PageHelper.AutoBindHeader(this, null, $"{@event.EventTitle} <span class='form-text'>scheduled {@event.EventScheduledStart.FormatDateOnly(User.TimeZone)}</span>");

            EventTitle.Text = $"<a href=\"/ui/admin/events/classes/outline?event={EventID}\">{@event.EventTitle}</a>";
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            //TODO Fix deleting

            ServiceLocator.SendCommand(new RemoveEventAttendee(EventID, InstructorID));

            HttpResponseHelper.Redirect($"/ui/admin/events/classes/outline?event={EventID}&panel=class");
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"event={EventID}"
                : null;
        }
    }
}