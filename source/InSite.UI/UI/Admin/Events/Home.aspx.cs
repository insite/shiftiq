using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using InSite.Admin.Events.Classes.Reports;
using InSite.Application.Events.Read;
using InSite.Application.Registrations.Read;
using InSite.Common.Web;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Toolbox;

namespace InSite.UI.Admin.Events
{
    public partial class Home : AdminBasePage
    {
        protected void BindModelToControls()
        {
            PageHelper.AutoBindHeader(this);

            PageHelper.DisplayCalendarLink(Page);

            var insite = CurrentSessionState.Identity.IsOperator;

            var counts = ServiceLocator.SiteSearch.SelectCount(Organization.OrganizationIdentifier);
            var hasCounts = counts.Length > 0;
            var sum = counts.Sum(x => x.Count);
        }

        private QEventFilter CreateEventFilter(string eventType)
            => new QEventFilter { OrganizationIdentifier = Organization.OrganizationIdentifier, EventType = eventType };

        private QRegistrationFilter CreateRegistrationFilter()
            => new QRegistrationFilter { OrganizationIdentifier = Organization.OrganizationIdentifier };

        protected static string GetExamSearchUrl(string type)
            => "/ui/admin/events/exams/search?clearcriteria=1&type=" + HttpUtility.UrlEncode(type);

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DownloadExamSessionsScheduled.Click += DownloadExamSessionsScheduled_Click;
            DownloadExamsWrittenByType.Click += DownloadExamsWrittenByType_Click;
            DownloadClassesLearningCenterButton.Click += DownloadClassesLearningCenterButton_Click;
        }

        private void DownloadClassesLearningCenterButton_Click(object sender, EventArgs e)
        {
            var data = LearningCenterReport.GetXlsx(Organization.OrganizationIdentifier);
            Response.SendFile("classes-learning-center", "xlsx", data);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                BindModelToControls();
                BindClassesPanel();
                BindExamsPanel();
                BindAppointmentsPanel();
                BindRecentChanges();
            }
        }

        private void BindClassesPanel()
        {
            ClassesPanel.Visible = Identity.IsGranted("Admin/Events/Classes");
            if (!ClassesPanel.Visible)
                return;

            var eventFilter = CreateEventFilter("Class");

            var classEventCount = ServiceLocator.EventSearch.CountEvents(eventFilter);
            ClassCount.Text = $"{classEventCount:n0}";

            var registrationFilter = CreateRegistrationFilter();
            registrationFilter.EventType = "Class";
            var registrationCount = ServiceLocator.RegistrationSearch.CountRegistrations(registrationFilter);
            RegistrationCount1.Text = $"{registrationCount:n0}";

            registrationFilter.EventType = "Exam";
            registrationCount = ServiceLocator.RegistrationSearch.CountRegistrations(registrationFilter);
            RegistrationCount2.Text = $"{registrationCount:n0}";

            var seatCount = ServiceLocator.EventSearch.CountSeats(new QSeatFilter { OrganizationIdentifier = Organization.OrganizationIdentifier });
            SeatCount.Text = $"{seatCount:n0}";
        }

        private void BindExamsPanel()
        {
            ExamsPanel.Visible = Identity.IsGranted("Admin/Events/Exams");
            if (!ExamsPanel.Visible)
                return;

            var filter = CreateEventFilter("Exam");

            var examEventCounts = ServiceLocator.EventSearch.CountEventsByExamType(filter);

            ExamCounterRepeater.DataSource = examEventCounts;
            ExamCounterRepeater.DataBind();

            ExamCounterZero.Visible = examEventCounts.Count == 0;
        }

        private void BindAppointmentsPanel()
        {
            AppointmentsPanel.Visible = Identity.IsGranted("Admin/Events/Classes");
            if (!AppointmentsPanel.Visible)
                return;

            var eventFilter = CreateEventFilter(EventType.Appointment.ToString());
            var appointmentEventCount = ServiceLocator.EventSearch.CountEvents(eventFilter);
            AppointmentCount.Text = $"{appointmentEventCount:n0}";
        }

        private void BindRecentChanges()
        {
            RecentClasses.LoadData(CreateEventFilter("Class"), 10);
            RecentExams.LoadData(CreateEventFilter("Exam"), 10);
            RecentAppointmentsList.LoadData(CreateEventFilter(EventType.Appointment.ToString()), 10);

            HistoryPanel.Visible = (RecentExams.ItemCount > 0) || RecentClasses.ItemCount > 0;
            SummaryPanel.Visible = ExamsPanel.Visible;
        }

        #region Export Exam Sessions Scheduled to XLSX

        private void DownloadExamSessionsScheduled_Click(object sender, EventArgs e)
        {
            ExportExamSessionsScheduledToXLSX();
        }

        private void ExportExamSessionsScheduledToXLSX()
        {
            var bytes = GetExamSessionsScheduledXlsx();
            if (bytes == null)
                return;

            var filename = string.Format("{0}-{1:yyyyMMdd}-{1:HHmmss}",
                StringHelper.Sanitize("Exam Sessions Scheduled", '-'), DateTime.UtcNow);

            Page.Response.SendFile(filename, "xlsx", bytes);
        }

        private byte[] GetExamSessionsScheduledXlsx()
        {
            var list = new List<QExamSessionsScheduled> { ServiceLocator.EventSearch.GetExamSessionsScheduled(Organization.OrganizationIdentifier) };
            if (list.Count == 0)
                return null;

            var helper = new XlsxExportHelper();

            helper.Map("Classes", "Classes", 30, HorizontalAlignment.Right);
            helper.Map("Sittings", "Sittings", 30, HorizontalAlignment.Right);
            helper.Map("Accommodated", "Accommodated", 30, HorizontalAlignment.Right);
            helper.Map("Individuals", "Individuals", 30, HorizontalAlignment.Right);
            helper.Map("Online", "Online", 30, HorizontalAlignment.Right);
            helper.Map("Month", "Month", 30, HorizontalAlignment.Right);
            helper.Map("Year", "Year", 30, HorizontalAlignment.Right);

            return helper.GetXlsxBytes(list, "Exam Sessions Scheduled");
        }

        #endregion

        #region Export Exams Written By Type to XLSX

        private void DownloadExamsWrittenByType_Click(object sender, EventArgs e)
        {
            ExamsWrittenByTypeToXLSX();
        }

        private void ExamsWrittenByTypeToXLSX()
        {
            var bytes = GetExamsWrittenByTypeXlsx();
            if (bytes == null)
                return;

            var filename = string.Format("{0}-{1:yyyyMMdd}-{1:HHmmss}",
                StringHelper.Sanitize("Exams Written by Type", '-'), DateTime.UtcNow);

            Page.Response.SendFile(filename, "xlsx", bytes);
        }

        private byte[] GetExamsWrittenByTypeXlsx()
        {
            var list = new List<QExamsWrittenByType> { ServiceLocator.EventSearch.GetExamsWrittenByType(Organization.OrganizationIdentifier) };
            if (list.Count == 0)
                return null;

            var helper = new XlsxExportHelper();

            helper.Map("CofQ", "CofQ", 30, HorizontalAlignment.Right);
            helper.Map("IPSE", "IPSE", 30, HorizontalAlignment.Right);
            helper.Map("LevelsFoundationCompletion", "Levels/Foundation/Completion", 30, HorizontalAlignment.Right);
            helper.Map("Classes", "Classes", 30, HorizontalAlignment.Right);
            helper.Map("SittingsIndividuals", "Sittings and Individuals", 30, HorizontalAlignment.Right);

            return helper.GetXlsxBytes(list, "Exams Written by Type");
        }

        #endregion
    }
}