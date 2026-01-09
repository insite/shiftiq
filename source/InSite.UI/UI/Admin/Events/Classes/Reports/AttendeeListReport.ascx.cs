using System;
using System.Linq;
using System.Web.UI;

using InSite.Application.Events.Read;
using InSite.Application.Registrations.Read;
using InSite.Domain.Registrations;

using Shift.Common;
using Shift.Constant;
using Shift.Toolbox;

using UserModel = InSite.Domain.Foundations.User;

namespace InSite.Admin.Events.Classes.Controls
{
    public partial class AttendeeListReport : UserControl
    {
        public void LoadReport(QEvent @event, UserModel user)
        {
            PageTitle.InnerText = "Attendee List";

            ClassTitle.Text = @event.EventTitle;
            ClassStartDate.Text = @event.EventScheduledStart.Format(user.TimeZone);
            ClassEndDate.Text = @event.EventScheduledEnd.Format(user.TimeZone);

            var instructors = ServiceLocator.EventSearch
                .GetAttendees(@event.EventIdentifier, x => x.Person.User)
                .Where(x => x.AttendeeRole == "Instructor")
                .ToList();

            ClassInstructors.Text = instructors.Count > 0
                ? string.Join(", ", instructors.Select(x => x.UserFullName))
                : "None";

            BindRegistrations(@event);
        }

        public static byte[] GetXlsx(QEvent @event, UserModel user)
        {
            var filter = new QRegistrationFilter
            {
                EventIdentifier = @event.EventIdentifier,
                OrderBy = "Candidate.UserFullName"
            };

            var registrations = ServiceLocator.RegistrationSearch
                .GetRegistrationsForAttendeeListReport(filter);

            var headerStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Center, VAlign = XlsxCellVAlign.Center };
            var boldStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Left, VAlign = XlsxCellVAlign.Center };
            var normalStyle = new XlsxCellStyle { Align = HorizontalAlignment.Left, VAlign = XlsxCellVAlign.Center };

            var sheet = new XlsxWorksheet("Attendee List");

            sheet.Columns[0].Width = 40;
            sheet.Columns[1].Width = 10;
            sheet.Columns[2].Width = 35;
            sheet.Columns[3].Width = 20;
            sheet.Columns[4].Width = 40;

            var instructors = ServiceLocator.EventSearch
                .GetAttendees(@event.EventIdentifier, x => x.Person.User)
                .Where(x => x.AttendeeRole == "Instructor")
                .ToList();

            var title = $"Class List \n {@event.EventTitle} \n " +
                $"{@event.EventScheduledStart.Format(user.TimeZone)} - " +
                $"{@event.EventScheduledEnd.Format(user.TimeZone)} \n Instructor(s): \n " +
                $"{(instructors.Count > 0 ? string.Join(", ", instructors.Select(x => x.UserFullName)) : "None")}";

            sheet.Rows[0].Height = 75;
            sheet.Cells.Add(new XlsxCell(0, 0, 5) { Value = title, Style = headerStyle });
            sheet.Cells.Add(new XlsxCell(0, 1) { Value = "Apprentice Name", Style = boldStyle });
            sheet.Cells.Add(new XlsxCell(1, 1) { Value = "ID #", Style = boldStyle });
            sheet.Cells.Add(new XlsxCell(2, 1) { Value = "Email", Style = boldStyle });
            sheet.Cells.Add(new XlsxCell(3, 1) { Value = "Phone", Style = boldStyle });
            sheet.Cells.Add(new XlsxCell(4, 1) { Value = "Employer", Style = boldStyle });

            for (var i = 0; i < registrations.Count; i++)
            {
                sheet.Cells.Add(new XlsxCell(0, i + 2) { Value = registrations[i].UserFullName, Style = normalStyle });
                sheet.Cells.Add(new XlsxCell(1, i + 2) { Value = registrations[i].PersonCode, Style = normalStyle });
                sheet.Cells.Add(new XlsxCell(2, i + 2) { Value = registrations[i].Email, Style = normalStyle });
                sheet.Cells.Add(new XlsxCell(3, i + 2) { Value = registrations[i].Phone, Style = normalStyle });
                sheet.Cells.Add(new XlsxCell(4, i + 2) { Value = registrations[i].EmployerName, Style = normalStyle });
            }

            return XlsxWorksheet.GetBytes(sheet);
        }

        private void BindRegistrations(QEvent @event)
        {
            var filter = new QRegistrationFilter
            {
                EventIdentifier = @event.EventIdentifier,
                OrderBy = "Candidate.UserFullName"
            };
            var registrations = ServiceLocator.RegistrationSearch.GetRegistrationsForAttendeeListReport(filter);

            ParticipationsRepeater.DataSource = registrations;
            ParticipationsRepeater.DataBind();
        }

        protected bool IsMinor()
        {
            var item = (AttendeeListReportDataItem)Page.GetDataItem();
            return item.CandidateBirthdate.HasValue && GetAge(item.CandidateBirthdate.Value, item.EventScheduledStart.Date) < 19;
        }

        private static int GetAge(DateTime birthdate, DateTime current)
        {
            var age = current.Year - birthdate.Year;

            if (current.Month < birthdate.Month || current.Month == birthdate.Month && current.Day < birthdate.Day)
                age--;

            return age;
        }
    }
}