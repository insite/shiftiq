using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Registrations.Read;

using Shift.Common;
using Shift.Constant;
using Shift.Toolbox;

namespace InSite.Admin.Events.Registrations.Reports
{
    public partial class RegistrationReport : UserControl
    {
        private class RegistrationItem
        {
            public int Number { get; set; }
            public string PersonCode { get; set; }
            public string UserFullName { get; set; }
            public string ApprovalStatus { get; set; }
            public string EmployerName { get; set; }
            public int? Age { get; set; }
            public bool IsMinor => Age.HasValue && Age < 19;
        }

        private class EventItem
        {
            public string EventTitle { get; set; }
            public string EventStart { get; set; }
            public string EventEnd { get; set; }
            public int? CapacityMax { get; set; }

            public List<RegistrationItem> Registrations { get; set; }

            public int RegistrationCount => Registrations.Count(x => StringHelper.Equals(x.ApprovalStatus, "Registered"));
            public int WaitlistedCount => Registrations.Count(x => StringHelper.Equals(x.ApprovalStatus, "Waitlisted"));
        }

        public void LoadReport(QRegistrationFilter filter, bool showCriteria)
        {
            PageTitle.InnerText = "Registration Report";

            if (showCriteria)
            {
                var criteriaItems = RegistrationCriteriaHelper.GetCriteriaItems(filter);

                SearchCriteriaRepeater.Visible = criteriaItems.Count > 0;
                SearchCriteriaRepeater.DataSource = criteriaItems;
                SearchCriteriaRepeater.DataBind();

                NoCriteriaPanel.Visible = criteriaItems.Count == 0;
            }
            else
            {
                SearchCriteriaRepeater.Visible = false;
                NoCriteriaPanel.Visible = false;
            }

            var timeZone = CurrentSessionState.Identity.User.TimeZone;

            var events = ServiceLocator.RegistrationSearch
                .GetRegistrations(filter, x => x.Event, x => x.Candidate, x => x.Employer)
                .GroupBy(x => x.EventIdentifier)
                .Select(x =>
                {
                    var registration = x.First();
                    var number = 1;

                    return new EventItem
                    {
                        EventTitle = registration.Event.EventTitle,
                        EventStart = registration.Event.EventScheduledStart.FormatDateOnly(timeZone),
                        EventEnd = registration.Event.EventScheduledEnd.HasValue ? registration.Event.EventScheduledEnd.FormatDateOnly(timeZone) : "N/A",
                        CapacityMax = registration.Event.CapacityMaximum,
                        Registrations = x
                            .OrderBy(y => y.ApprovalStatus)
                            .ThenBy(y => y.Candidate.UserFullName)
                            .Select(y => new RegistrationItem
                            {
                                Number = number++,
                                PersonCode = y.Candidate.PersonCode,
                                UserFullName = y.Candidate.UserFullName,
                                ApprovalStatus = y.ApprovalStatus,
                                EmployerName = y.Employer?.GroupName,
                                Age = y.Candidate.Birthdate.HasValue
                                    ? GetAge(y.Candidate.Birthdate.Value, registration.Event.EventScheduledStart.Date)
                                    : (int?)null
                            }).ToList()
                    };
                })
                .OrderBy(x => x.EventTitle)
                .ToList();

            EventRepeater.ItemDataBound += EventRepeater_ItemDataBound;
            EventRepeater.DataSource = events;
            EventRepeater.DataBind();
        }

        public static byte[] GetXlsx(QRegistrationFilter filter)
        {
            var timeZone = CurrentSessionState.Identity.User.TimeZone;

            var @event = ServiceLocator.RegistrationSearch
                .GetRegistrations(filter, x => x.Event, x => x.Candidate, x => x.Employer)
                .GroupBy(x => x.EventIdentifier)
                .Select(x =>
                {
                    var registration = x.First();
                    var number = 1;

                    return new EventItem
                    {
                        EventTitle = registration.Event.EventTitle,
                        EventStart = registration.Event.EventScheduledStart.FormatDateOnly(timeZone),
                        EventEnd = registration.Event.EventScheduledEnd.HasValue ? registration.Event.EventScheduledEnd.FormatDateOnly(timeZone) : "N/A",
                        CapacityMax = registration.Event.CapacityMaximum,
                        Registrations = x
                            .OrderBy(y => y.ApprovalStatus)
                            .ThenBy(y => y.Candidate.UserFullName)
                            .Select(y => new RegistrationItem
                            {
                                Number = number++,
                                PersonCode = y.Candidate.PersonCode,
                                UserFullName = y.Candidate.UserFullName,
                                ApprovalStatus = y.ApprovalStatus,
                                EmployerName = y.Employer?.GroupName,
                                Age = y.Candidate.Birthdate.HasValue
                                    ? GetAge(y.Candidate.Birthdate.Value, registration.Event.EventScheduledStart.Date)
                                    : (int?)null
                            }).ToList()
                    };
                })
                .OrderBy(x => x.EventTitle)
                .ToList()[0];

            var boldStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Left, VAlign = XlsxCellVAlign.Center };
            var boldCenterStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Center, VAlign = XlsxCellVAlign.Center };
            var normalStyle = new XlsxCellStyle { Align = HorizontalAlignment.Left, VAlign = XlsxCellVAlign.Center };
            var normalCenterStyle = new XlsxCellStyle { Align = HorizontalAlignment.Center, VAlign = XlsxCellVAlign.Center };

            var sheet = new XlsxWorksheet("Registration Report");

            sheet.Columns[0].Width = 5;
            sheet.Columns[1].Width = 10;
            sheet.Columns[2].Width = 40;
            sheet.Columns[3].Width = 20;
            sheet.Columns[4].Width = 40;

            string title = $"Registration Report \n" +
                $"{@event.EventTitle} ({@event.EventStart} - {@event.EventEnd})";

            sheet.Rows[0].Height = 30;
            sheet.Cells.Add(new XlsxCell(0, 0, 5) { Value = title, Style = boldStyle });
            sheet.Cells.Add(new XlsxCell(0, 1) { Value = "#", Style = boldCenterStyle });
            sheet.Cells.Add(new XlsxCell(1, 1) { Value = "ID #", Style = boldStyle });
            sheet.Cells.Add(new XlsxCell(2, 1) { Value = "Name", Style = boldStyle });
            sheet.Cells.Add(new XlsxCell(3, 1) { Value = "Status", Style = boldStyle });
            sheet.Cells.Add(new XlsxCell(4, 1) { Value = "Employer", Style = boldStyle });

            for (int i = 0; i < @event.Registrations.Count; i++)
            {
                sheet.Cells.Add(new XlsxCell(0, i + 2) { Value = @event.Registrations[i].Number, Style = normalCenterStyle });
                sheet.Cells.Add(new XlsxCell(1, i + 2) { Value = @event.Registrations[i].PersonCode, Style = normalStyle });
                sheet.Cells.Add(new XlsxCell(2, i + 2) { Value = @event.Registrations[i].UserFullName, Style = normalStyle });
                sheet.Cells.Add(new XlsxCell(3, i + 2) { Value = @event.Registrations[i].ApprovalStatus, Style = normalStyle });
                sheet.Cells.Add(new XlsxCell(4, i + 2) { Value = @event.Registrations[i].EmployerName, Style = normalStyle });
            }

            String summary = $"Total registrations: {@event.RegistrationCount}   (Maximum {@event.CapacityMax})   Waitlisted: {@event.WaitlistedCount}";
            sheet.Cells.Add(new XlsxCell(0, @event.Registrations.Count + 2, 5) { Value = summary, Style = boldStyle });

            return XlsxWorksheet.GetBytes(sheet);
        }

        private void EventRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var @event = (EventItem)e.Item.DataItem;

            var registrationRepeater = (Repeater)e.Item.FindControl("RegistrationRepeater");
            registrationRepeater.DataSource = @event.Registrations;
            registrationRepeater.DataBind();
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