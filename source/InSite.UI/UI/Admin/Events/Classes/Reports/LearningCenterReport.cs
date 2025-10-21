using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Events.Read;
using InSite.Application.Records.Read;
using InSite.Application.Registrations.Read;

using Shift.Constant;
using Shift.Toolbox;

namespace InSite.Admin.Events.Classes.Reports
{
    public static class LearningCenterReport
    {
        private class RegistrationItem
        {
            public string ClassName { get; set; }
            public string Instructors { get; set; }
            public string UserFullName { get; set; }
            public string Code { get; set; }
            public int? WorkBasedHoursToDate { get; set; }
            public string EmployerName { get; set; }
            public string EmployerStatus { get; set; }
            public decimal? FinalScore { get; set; }
            public string UserPhone { get; set; }
            public string UserEmail { get; set; }
            public string EmergencyContactName { get; set; }
            public string EmergencyContactPhone { get; set; }
            public string EmergencyContactRelationship { get; set; }
        }

        private class Data
        {
            public List<RegistrationItem> Current { get; set; }
            public List<RegistrationItem> Future { get; set; }
            public List<RegistrationItem> Past { get; set; }
        }

        public static byte[] GetXlsx(Guid organizationIdentifier)
        {
            var data = LoadData(organizationIdentifier);
            var current = CreateSheet("Current", data.Current);
            var future = CreateSheet("Future", data.Future);
            var past = CreateSheet("Past", data.Past);

            return XlsxWorksheet.GetBytes(current, future, past);
        }

        private static XlsxWorksheet CreateSheet(string sheetName, List<RegistrationItem> items)
        {
            items = items
                .OrderBy(x => x.ClassName)
                .ThenBy(x => x.UserFullName)
                .ToList();

            var headerLeftStyle = new XlsxCellStyle { IsBold = true };
            var headerRightStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Right };
            var leftStyle = new XlsxCellStyle { };
            var rightStyle = new XlsxCellStyle { Align = HorizontalAlignment.Right };
            var scoreFormat = "#0.00%";
            var hoursFormat = "####0.00";

            var sheet = new XlsxWorksheet(sheetName);
            sheet.Columns[0].Width = 30; // Class
            sheet.Columns[1].Width = 30; // Instructors
            sheet.Columns[2].Width = 30; // Student
            sheet.Columns[3].Width = 15; // Code
            sheet.Columns[4].Width = 15; // Hours Reported
            sheet.Columns[5].Width = 30; // Employer
            sheet.Columns[6].Width = 20; // Group Status
            sheet.Columns[7].Width = 15; // Final Score
            sheet.Columns[8].Width = 25; // Preferred Phone Number
            sheet.Columns[9].Width = 30; // Email
            sheet.Columns[10].Width = 30; // Emergency Contact Name
            sheet.Columns[11].Width = 32; // Emergency Contact Phone Number
            sheet.Columns[12].Width = 30; // Emergency Contact Relationship

            var codeCaption = CurrentSessionState.Identity.Organization.OrganizationIdentifier == OrganizationIdentifiers.RCABC
                ? "Tradeworker #"
                : "Person Code";

            sheet.Cells.Add(new XlsxCell(0, 0) { Value = "Class", Style = headerLeftStyle });
            sheet.Cells.Add(new XlsxCell(1, 0) { Value = "Instructor(s)", Style = headerLeftStyle });
            sheet.Cells.Add(new XlsxCell(2, 0) { Value = "Student", Style = headerLeftStyle });
            sheet.Cells.Add(new XlsxCell(3, 0) { Value = codeCaption, Style = headerLeftStyle });
            sheet.Cells.Add(new XlsxCell(4, 0) { Value = "Hours Reported", Style = headerLeftStyle });
            sheet.Cells.Add(new XlsxCell(5, 0) { Value = "Employer", Style = headerLeftStyle });
            sheet.Cells.Add(new XlsxCell(6, 0) { Value = "Group Status", Style = headerLeftStyle });
            sheet.Cells.Add(new XlsxCell(7, 0) { Value = "Final Score", Style = headerRightStyle });
            sheet.Cells.Add(new XlsxCell(8, 0) { Value = "Preferred Phone Number", Style = headerLeftStyle });
            sheet.Cells.Add(new XlsxCell(9, 0) { Value = "Email", Style = headerLeftStyle });
            sheet.Cells.Add(new XlsxCell(10, 0) { Value = "Emergency Contact Name", Style = headerLeftStyle });
            sheet.Cells.Add(new XlsxCell(11, 0) { Value = "Emergency Contact Phone Number", Style = headerLeftStyle });
            sheet.Cells.Add(new XlsxCell(12, 0) { Value = "Emergency Contact Relationship", Style = headerLeftStyle });

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var row = i + 1;

                sheet.Cells.Add(new XlsxCell(0, row) { Value = item.ClassName, Style = leftStyle });
                sheet.Cells.Add(new XlsxCell(1, row) { Value = item.Instructors, Style = leftStyle });
                sheet.Cells.Add(new XlsxCell(2, row) { Value = item.UserFullName, Style = leftStyle });
                sheet.Cells.Add(new XlsxCell(3, row) { Value = item.Code, Style = leftStyle });
                sheet.Cells.Add(new XlsxCell(4, row) { Value = item.WorkBasedHoursToDate, Style = leftStyle, Format = hoursFormat });
                sheet.Cells.Add(new XlsxCell(5, row) { Value = item.EmployerName, Style = leftStyle });
                sheet.Cells.Add(new XlsxCell(6, row) { Value = item.EmployerStatus, Style = leftStyle });
                sheet.Cells.Add(new XlsxCell(7, row) { Value = item.FinalScore, Style = rightStyle, Format = scoreFormat });
                sheet.Cells.Add(new XlsxCell(8, row) { Value = item.UserPhone, Style = leftStyle });
                sheet.Cells.Add(new XlsxCell(9, row) { Value = item.UserEmail, Style = leftStyle });
                sheet.Cells.Add(new XlsxCell(10, row) { Value = item.EmergencyContactName, Style = leftStyle });
                sheet.Cells.Add(new XlsxCell(11, row) { Value = item.EmergencyContactPhone, Style = leftStyle });
                sheet.Cells.Add(new XlsxCell(12, row) { Value = item.EmergencyContactRelationship, Style = leftStyle });
            }

            return sheet;
        }

        private static Data LoadData(Guid organizationIdentifier)
        {
            var prevMonth = DateTime.UtcNow.AddMonths(-1);
            var startDate = new DateTime(prevMonth.Year, prevMonth.Month, 1);
            var endDate = startDate.AddMonths(3);

            var filter = new QRegistrationFilter
            {
                OrganizationIdentifier = organizationIdentifier,
                IsRegistered = true,
                EventScheduledSince = startDate,
                EventScheduledBefore = endDate
            };

            var registrations = ServiceLocator.RegistrationSearch
                .GetRegistrations(
                    filter,
                    x => x.Event, x => x.Candidate, x => x.Employer);

            return LoadData(registrations);
        }

        private static Data LoadData(List<QRegistration> registrations)
        {
            var eventsToFinalMarks = new Dictionary<Guid, Dictionary<Guid, decimal?>>();
            var eventsToInstructors = new Dictionary<Guid, string>();
            var result = new Data
            {
                Current = new List<RegistrationItem>(),
                Future = new List<RegistrationItem>(),
                Past = new List<RegistrationItem>()
            };

            var today = DateTime.UtcNow.Date;
            var nextDay = today.AddDays(1);

            foreach (var registration in registrations)
            {
                if (!eventsToFinalMarks.TryGetValue(registration.EventIdentifier, out var finalMarks))
                {
                    finalMarks = LoadGradebookScores(registration);
                    eventsToFinalMarks.Add(registration.EventIdentifier, finalMarks);
                }

                if (!finalMarks.TryGetValue(registration.CandidateIdentifier, out var finalScore))
                    finalScore = null;

                if (!eventsToInstructors.TryGetValue(registration.EventIdentifier, out var instructors))
                {
                    instructors = LoadEventInstructors(registration.EventIdentifier);
                    eventsToInstructors.Add(registration.EventIdentifier, instructors);
                }

                var item = new RegistrationItem
                {
                    ClassName = registration.Event.EventTitle,
                    Instructors = instructors,
                    UserFullName = registration.Candidate.UserFullName,
                    Code = registration.Candidate.PersonCode,
                    WorkBasedHoursToDate = registration.WorkBasedHoursToDate,
                    EmployerName = registration.Employer?.GroupName,
                    EmployerStatus = registration.Employer?.GroupStatus,
                    FinalScore = finalScore,
                    UserPhone = registration.Candidate.UserPhone,
                    UserEmail = registration.Candidate.UserEmail,
                    EmergencyContactName = registration.Candidate.EmergencyContactName,
                    EmergencyContactPhone = registration.Candidate.EmergencyContactPhone,
                    EmergencyContactRelationship = registration.Candidate.EmergencyContactRelationship
                };

                if (registration.Event.EventScheduledStart.UtcDateTime >= nextDay)
                    result.Future.Add(item);
                else if (registration.Event.EventScheduledEnd == null || registration.Event.EventScheduledEnd.Value.UtcDateTime < today)
                    result.Past.Add(item);
                else
                    result.Current.Add(item);
            }

            return result;
        }

        private static string LoadEventInstructors(Guid eventIdentifier)
        {
            var filter = new QEventAttendeeFilter { EventIdentifier = eventIdentifier, ContactRole = "Instructor" };
            var instructors = ServiceLocator.EventSearch.GetAttendees(filter, null, null, x => x.Person.User);

            return instructors.Count > 0
                ? string.Join(", ", instructors.Select(x => x.UserFullName))
                : null;
        }

        private static Dictionary<Guid, decimal?> LoadGradebookScores(QRegistration registration)
        {
            var finalMarks = new Dictionary<Guid, decimal?>();

            var gradebookFilter = new QGradebookFilter
            {
                OrganizationIdentifier = registration.Event.OrganizationIdentifier,
                PrimaryEventIdentifier = registration.EventIdentifier
            };

            var gradebook = ServiceLocator.RecordSearch.GetGradebooks(gradebookFilter).FirstOrDefault();
            if (gradebook == null)
                return finalMarks;

            var gradeItems = ServiceLocator.RecordSearch.GetGradeItems(gradebook.GradebookIdentifier);
            var topItem = gradeItems
                .Where(x => x.ParentGradeItemIdentifier == null)
                .OrderBy(x => x.GradeItemSequence)
                .FirstOrDefault();

            if (topItem == null)
                return finalMarks;

            var progresses = ServiceLocator.RecordSearch.GetGradebookScores(new QProgressFilter { GradeItemIdentifier = topItem.GradeItemIdentifier });
            foreach (var progress in progresses)
                finalMarks.Add(progress.UserIdentifier, progress.ProgressPercent);

            return finalMarks;
        }
    }
}