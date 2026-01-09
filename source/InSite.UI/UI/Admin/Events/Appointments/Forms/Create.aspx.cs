using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Events.Write;
using InSite.Application.Registrations.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Events.Appointments.Forms
{
    public partial class Create : AdminBasePage
    {
        protected string OutlineUrl => "/ui/admin/events/appointments/outline";
        protected string SearchUrl => "/ui/admin/events/appointments/search";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EventScheduledEndValidator.ServerValidate += EventScheduledEndValidator_ServerValidate;
            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write))
                HttpResponseHelper.Redirect(SearchUrl);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            CreationType.EnsureDataBound();
            CreationType.SetVisibleOptions(CreationTypeEnum.One);

            CancelButton.NavigateUrl = SearchUrl;
        }

        private void EventScheduledEndValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            OnEventScheduledEndValidatorServerValidate(EventScheduledEndValidator, args, EventScheduledStart.Value?.UtcDateTime, EventScheduledEnd.Value?.UtcDateTime);
        }

        private void OnEventScheduledEndValidatorServerValidate(Common.Web.UI.CustomValidator validator, ServerValidateEventArgs args, DateTime? start, DateTime? end)
        {
            if (end <= start)
            {
                args.IsValid = false;
                validator.ErrorMessage = "End Time must be later than Start Time";
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var commands = new List<ICommand>();
            var appointmentId = Guid.NewGuid();

            commands.Add(new ScheduleAppointment(
                appointmentId,
                Organization.OrganizationIdentifier,
                EventTitle.Text,
                AppointmentType.Value,
                AppointmentDescription.Text,
                EventScheduledStart.Value.Value,
                EventScheduledEnd.Value.Value
                ));


            commands.Add(new RequestRegistration(
                Guid.NewGuid(),
                Organization.OrganizationIdentifier,
                appointmentId,
                User.UserIdentifier,
                null,
                "Scheduled",
                null, null, null));

            commands.Add(new ModifyEventCalendarColor(
                appointmentId,
                CalendarColor.Value
                ));

            if (AppointmentDescription.Text.HasValue())
            {
                var title = new MultilingualString();
                title.Default = EventTitle.Text;

                var description = new MultilingualString();
                description.Default = AppointmentDescription.Text;

                commands.Add(
                    new DescribeAppointment(appointmentId, title, description)
                    );
            }

            foreach (var command in commands)
                ServiceLocator.SendCommand(command);

            HttpResponseHelper.Redirect($"{OutlineUrl}?event={appointmentId}");
        }
    }
}