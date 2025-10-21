using System;

using InSite.Application.Events.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

using ChangeAppointmentTypeCommand = InSite.Application.Events.Write.ChangeAppointmentType;

namespace InSite.UI.Admin.Events.Appointments.Forms
{
    public partial class ChangeAppointmentType : AdminBasePage, IHasParentLinkParameters
    {
        private const string SearchUrl = "/ui/admin/events/appointments/search";

        private Guid EventIdentifier => Guid.TryParse(Request["event"], out var result) ? result : Guid.Empty;

        private string OutlineUrl
            => $"/ui/admin/events/appointments/outline?event={EventIdentifier}";

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/outline") ? $"event={EventIdentifier}" : null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanEdit)
                NavigateToSearch();

            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            var @event = ServiceLocator.EventSearch.GetEvent(EventIdentifier);
            if (@event == null || @event.OrganizationIdentifier != Organization.OrganizationIdentifier)
                NavigateToSearch();

            BackButton.NavigateUrl = OutlineUrl;

            var title = $"{@event.EventTitle} <span class='form-text'>scheduled {@event.EventScheduledStart.FormatDateOnly(User.TimeZone)}</span>";
            PageHelper.AutoBindHeader(this, null, title);

            EventTitle.Text = @event.EventTitle;
            AppointmentType.Value = @event.AppointmentType;
            AppointmentCalendarColor.Value = @event.EventCalendarColor ?? "Info";
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            ServiceLocator.SendCommand(new ChangeAppointmentTypeCommand(EventIdentifier, AppointmentType.Value));
            ServiceLocator.SendCommand(new ModifyEventCalendarColor(EventIdentifier, AppointmentCalendarColor.Value));

            HttpResponseHelper.Redirect(BackButton.NavigateUrl);
        }

        private void NavigateToSearch()
            => HttpResponseHelper.Redirect(SearchUrl, true);
    }
}