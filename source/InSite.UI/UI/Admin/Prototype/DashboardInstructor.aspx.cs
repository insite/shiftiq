using System;

using InSite.Application.Events.Read;
using InSite.Application.Issues.Read;
using InSite.Application.Registrations.Read;
using InSite.UI.Layout.Admin;

using Shift.Constant;

namespace InSite.UI.Admin.Prototype
{
    public partial class DashboardInstructor : AdminBasePage
    {
        protected string CalendarDate
        {
            get
            {
                if (!DateTime.TryParse(Request.QueryString["date"], out var date))
                    date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, User.TimeZone);

                return date.ToShortDateString();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            PageHelper.AutoBindHeader(this);

            if (IsPostBack)
                return;

            BindActivities();
            BindCalendar();
            BindNotifications();
        }

        private QEventFilter CreateEventFilter(string eventType)
            => new QEventFilter { OrganizationIdentifier = Organization.OrganizationIdentifier, EventType = eventType };

        private QRegistrationFilter CreateRegistrationFilter()
            => new QRegistrationFilter { OrganizationIdentifier = Organization.OrganizationIdentifier };

        private QIssueFilter CreateCaseFilter()
            => new QIssueFilter { OrganizationIdentifier = Organization.OrganizationIdentifier };

        private void BindActivities()
        {
            RecentPeople.LoadData(7);
            RecentClasses.LoadData(CreateEventFilter("Class"), 7);
            RecentExams.LoadData(CreateEventFilter("Exam"), 7);
            RecentAppointmentsList.LoadData(CreateEventFilter(EventType.Appointment.ToString()), 7);
        }

        private void BindCalendar()
        {
            CalendarPanel.Visible = true;
        }

        private void BindNotifications()
        {
            RecentCases.LoadData(CreateCaseFilter(), 7);
        }
    }
}