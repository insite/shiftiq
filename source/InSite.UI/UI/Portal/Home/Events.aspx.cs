using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Events.Read;
using InSite.Application.Registrations.Read;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Home
{
    public partial class Events : PortalBasePage
    {
        private bool HasAppointments
        {
            get => (bool)ViewState[nameof(HasAppointments)];
            set => ViewState[nameof(HasAppointments)] = value;
        }

        private bool HasClasses
        {
            get => (bool)ViewState[nameof(HasClasses)];
            set => ViewState[nameof(HasClasses)] = value;
        }

        private bool HasExams
        {
            get => (bool)ViewState[nameof(HasExams)];
            set => ViewState[nameof(HasExams)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EventTypeMultiComboBox.AutoPostBack = true;
            EventTypeMultiComboBox.SelectAll();
            EventTypeMultiComboBox.ValueChanged += (s, a) => FilterEvents();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!HasAppointments)
                MyAppointmentsStatus.AddMessage(AlertType.Information, GetDisplayText("You have no scheduled Appointments."));

            if (!HasClasses)
                MyClassesStatus.AddMessage(AlertType.Information, GetDisplayText("You have no scheduled Classes."));

            if (!HasExams)
                MyExamsStatus.AddMessage(AlertType.Information, GetDisplayText("You have no scheduled Exams."));
        }

        private void LoadData()
        {
            PageHelper.AutoBindHeader(this);

            PortalMaster.ShowAvatar();
            PortalMaster.EnableSidebarToggle(true);

            var filter = new QRegistrationFilter { OrganizationIdentifier = Organization.Identifier, CandidateIdentifier = User.Identifier };
            var all = ServiceLocator.RegistrationSearch.GetRegistrations(
                filter, 
                x => x.Event.Achievement);

            HasAppointments = BindAppointments(all);
            HasClasses = BindClasses(all);
            HasExams = BindExams(all);

            if (!HasAppointments && !HasClasses && !HasExams)
                StatusAlert.AddMessage(AlertType.Warning, GetDisplayText("You are not registered for any events."));
        }

        private bool BindAppointments(List<QRegistration> all)
        {
            var appointments = all
                .Where(x => string.Equals(x.Event.EventType, "Appointment", StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(x => x.Event.EventScheduledStart)
                .Select(x =>
                {
                    var isCancelled = string.Equals(x.AttendanceStatus, "Withdrawn/Cancelled", StringComparison.OrdinalIgnoreCase);

                    return new
                    {
                        EventIdentifier = x.Event.EventIdentifier,
                        EventTitle = x.Event.EventTitle,
                        AchievementIdentifier = x.Event.AchievementIdentifier,
                        AchievementTitle = x.Event.Achievement?.AchievementTitle,
                        EventScheduledStart = x.Event.EventScheduledStart,
                        EventScheduledEnd = x.Event.EventScheduledEnd,
                        EventScheduledText = GetEventScheduledText(x.Event),
                        Summary = GetSummary(x.Event),
                        IsCancelled = isCancelled,
                        IsScheduled = !isCancelled && string.Equals(x.ApprovalStatus, "Scheduled", StringComparison.OrdinalIgnoreCase),
                        IsRegistered = !isCancelled && string.Equals(x.ApprovalStatus, "Registered", StringComparison.OrdinalIgnoreCase),
                        IsWaitlisted = !isCancelled && string.Equals(x.ApprovalStatus, "Waitlisted", StringComparison.OrdinalIgnoreCase),
                        IsInvited = !isCancelled && string.Equals(x.ApprovalStatus, "Invitation Sent", StringComparison.OrdinalIgnoreCase),
                        IsMoved = !isCancelled && string.Equals(x.ApprovalStatus, "Moved", StringComparison.OrdinalIgnoreCase)
                    };
                })
                .ToList();

            MyAppointments.Visible = appointments.Count > 0;

            AppointmentCount.InnerText = Shift.Common.Humanizer.ToQuantity(appointments.Count, "Appointment");

            AppointmentRepeater.DataSource = appointments;
            AppointmentRepeater.DataBind();

            return appointments.Count > 0;
        }

        private bool BindClasses(List<QRegistration> all)
        {
            var classes = all
                .Where(x => string.Equals(x.Event.EventType, "Class", StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(x => x.Event.EventScheduledStart)
                .Select(x =>
                {
                    var isCancelled = string.Equals(x.AttendanceStatus, "Withdrawn/Cancelled", StringComparison.OrdinalIgnoreCase);

                    return new
                    {
                        EventIdentifier = x.Event.EventIdentifier,
                        EventTitle = x.Event.EventTitle,
                        AchievementIdentifier = x.Event.AchievementIdentifier,
                        AchievementTitle = x.Event.Achievement?.AchievementTitle,
                        EventScheduledStart = x.Event.EventScheduledStart,
                        EventScheduledEnd = x.Event.EventScheduledEnd,
                        EventScheduledText = GetEventScheduledText(x.Event),
                        Summary = GetSummary(x.Event),
                        IsCancelled = isCancelled,
                        IsRegistered = !isCancelled && string.Equals(x.ApprovalStatus, "Registered", StringComparison.OrdinalIgnoreCase),
                        IsWaitlisted = !isCancelled && string.Equals(x.ApprovalStatus, "Waitlisted", StringComparison.OrdinalIgnoreCase),
                        IsInvited = !isCancelled && string.Equals(x.ApprovalStatus, "Invitation Sent", StringComparison.OrdinalIgnoreCase),
                        IsMoved = !isCancelled && string.Equals(x.ApprovalStatus, "Moved", StringComparison.OrdinalIgnoreCase)
                    };
                })
                .ToList();

            MyClasses.Visible = classes.Count > 0;

            ClassCount.InnerText = Shift.Common.Humanizer.ToQuantity(classes.Count, "Class");

            ClassRepeater.DataSource = classes;
            ClassRepeater.DataBind();

            return classes.Count > 0;
        }

        private bool BindExams(List<QRegistration> all)
        {
            var exams = all
                .Where(x => string.Equals(x.Event.EventType, "Exam", StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(x => x.Event.EventScheduledStart)
                .Select(x =>
                {
                    var isCancelled = string.Equals(x.AttendanceStatus, "Withdrawn/Cancelled", StringComparison.OrdinalIgnoreCase);

                    return new
                    {
                        EventIdentifier = x.Event.EventIdentifier,
                        EventTitle = x.Event.EventTitle,
                        EventScheduledStart = x.Event.EventScheduledStart,
                        EventScheduledText = x.Event.EventScheduledStart.Format(null, true),
                        ExamType = x.Event.ExamType,
                        ExamFormat = x.Event.EventFormat,
                        EventNumber = x.Event.EventNumber.ToString()
                    };
                })
                .ToList();

            MyExams.Visible = exams.Count > 0;

            ExamCount.InnerText = Shift.Common.Humanizer.ToQuantity(exams.Count, "Exam");

            ExamRepeater.DataSource = exams;
            ExamRepeater.DataBind();

            return exams.Count > 0;
        }

        private void FilterEvents()
        {
            var selected = EventTypeMultiComboBox.ValuesArray;

            MyAppointments.Visible = MyClasses.Visible = MyExams.Visible = false;

            foreach (var item in selected)
            {
                if (item.Equals(EventType.Appointment.ToString()))
                    MyAppointments.Visible = true;

                if (item.Equals(EventType.Class.ToString()))
                    MyClasses.Visible = true;

                if (item.Equals(EventType.Exam.ToString()))
                    MyExams.Visible = true;

            }
        }

        private static string GetSummary(QEvent @event)
        {
            return Markdown.ToHtml(ContentEventClass.Deserialize(@event.Content).Summary.Default);
        }

        private static string GetEventScheduledText(QEvent @event)
        {
            return @event.EventScheduledEnd == null || @event.EventScheduledEnd.Value.Date == @event.EventScheduledStart.Date
                ? $"{@event.EventScheduledStart:dddd, MMM d}"
                : $"{@event.EventScheduledStart:dddd, MMM d} to {@event.EventScheduledEnd:dddd, MMM d}";
        }
    }
}