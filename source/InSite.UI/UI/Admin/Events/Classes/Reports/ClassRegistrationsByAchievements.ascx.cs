using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using InSite.Application.Events.Read;
using InSite.UI.Admin.Events.Classes.Reports;

using Shift.Common;
using Shift.Constant;
using Shift.Toolbox;

namespace InSite.Admin.Events.Classes.Reports
{
    public partial class ClassRegistrationsByAchievements : BaseReportControl
    {
        private class ClassItem
        {
            public int Number { get; set; }
            public string ClassTitle { get; set; }
            public string ClassInstructors { get; set; }
            public string ClassStart { get; set; }
            public string ClassEnd { get; set; }
            public int RegistrationCount { get; set; }

        }

        private class AchievementItem
        {
            public string AchievementTitle { get; set; }
            public List<ClassItem> Classes { get; set; }
            public int ClassCount => Classes.Count;
            public int RegistrationCount => Classes.Sum(x => x.RegistrationCount);
        }

        public override string ReportTitle => "Class Registrations";

        public override string ReportFileName => "ClassRegistrationsByAchievements";

        public override byte[] GetPdf(QEventFilter filter)
        {
            PageTitle.InnerText = ReportTitle;

            var achievements = GetAchievements(filter);

            AchievementRepeater.ItemDataBound += AchievementRepeater_ItemDataBound;
            AchievementRepeater.DataSource = achievements;
            AchievementRepeater.DataBind();

            var criteriaItems = GetCriteriaItems(filter);

            SearchCriteriaRepeater.Visible = criteriaItems.Count > 0;
            SearchCriteriaRepeater.DataSource = criteriaItems;
            SearchCriteriaRepeater.DataBind();

            NoCriteriaPanel.Visible = criteriaItems.Count == 0;

            return BuildPdf(PageOrientationType.Portrait, 1400, 980, ReportTitle);
        }

        public override byte[] GetXlsx(QEventFilter filter)
        {
            var achievements = GetAchievements(filter);
            var criteriaItems = GetCriteriaItems(filter);

            var criteriaRowHeight = (criteriaItems.Count + 2) * 15 > 45 ? (criteriaItems.Count + 2) * 15 : 45;
            var criterias = new StringBuilder();
            criterias.AppendLine("Search Criteria");

            if (criteriaItems.Count > 0)
            {
                foreach (var item in criteriaItems)
                    criterias.AppendLine($"{item.Name} = {item.Value}");
            }
            else
                criterias.AppendLine("None");

            var sheet = new XlsxWorksheet(ReportTitle);

            var boldStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Left };
            var boldRightStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Right };
            var normalStyle = new XlsxCellStyle { Align = HorizontalAlignment.Left };
            var centerStyle = new XlsxCellStyle { Align = HorizontalAlignment.Center };
            var rightStyle = new XlsxCellStyle { Align = HorizontalAlignment.Right };

            sheet.Columns[0].Width = 60;
            sheet.Columns[1].Width = 30;
            sheet.Columns[2].Width = 20;
            sheet.Columns[3].Width = 20;
            sheet.Columns[4].Width = 20;

            sheet.Rows[0].Height = criteriaRowHeight;
            XlsxCellRichText headerCell = new XlsxCellRichText(0, 0, 5) { Style = normalStyle };
            headerCell.AddText(ReportTitle + "\n", true);
            headerCell.AddText(criterias.ToString(), false);
            sheet.Cells.Add(headerCell);

            var row = 1;

            foreach (var achievement in achievements)
            {
                sheet.Cells.Add(new XlsxCell(0, ++row) { Value = "Class", Style = boldStyle });
                sheet.Cells.Add(new XlsxCell(1, row) { Value = "Instructors", Style = boldStyle });
                sheet.Cells.Add(new XlsxCell(2, row) { Value = "Start", Style = boldStyle });
                sheet.Cells.Add(new XlsxCell(3, row) { Value = "End", Style = boldStyle });
                sheet.Cells.Add(new XlsxCell(4, row) { Value = "Registrations", Style = boldRightStyle });

                foreach (var classItem in achievement.Classes)
                {
                    sheet.Cells.Add(new XlsxCell(0, ++row) { Value = classItem.ClassTitle, Style = normalStyle });
                    sheet.Cells.Add(new XlsxCell(1, row) { Value = classItem.ClassInstructors, Style = normalStyle });
                    sheet.Cells.Add(new XlsxCell(2, row) { Value = classItem.ClassStart, Style = normalStyle });
                    sheet.Cells.Add(new XlsxCell(3, row) { Value = classItem.ClassEnd, Style = normalStyle });
                    sheet.Cells.Add(new XlsxCell(4, row) { Value = classItem.RegistrationCount, Style = rightStyle });
                }

                sheet.Cells.Add(new XlsxCell(0, ++row) { Value = achievement.AchievementTitle, Style = boldStyle });
                XlsxCellRichText totalClassesCell = new XlsxCellRichText(1, row) { Style = centerStyle };
                totalClassesCell.AddText("Totals: ", false);
                totalClassesCell.AddText(achievement.ClassCount.ToString(), true);
                totalClassesCell.AddText(" Classes", false);
                sheet.Cells.Add(totalClassesCell);
                XlsxCellRichText registrationsCountCell = new XlsxCellRichText(2, row, 3) { Style = rightStyle };
                registrationsCountCell.AddText(achievement.RegistrationCount.ToString(), true);
                registrationsCountCell.AddText(" Registrations", false);
                sheet.Cells.Add(registrationsCountCell);

                row += 2;
            }

            return XlsxWorksheet.GetBytes(sheet);
        }

        private List<AchievementItem> GetAchievements(QEventFilter filter)
        {
            var timeZone = CurrentSessionState.Identity.User.TimeZone;

            return ServiceLocator.EventSearch
                .GetEvents(filter, null, null, x => x.Achievement, x => x.Registrations)
                .GroupBy(x => x.AchievementIdentifier)
                .Select(x =>
                {
                    var achievement = x.First().Achievement;
                    var number = 1;

                    return new AchievementItem
                    {
                        AchievementTitle = achievement?.AchievementTitle,
                        Classes = x
                            .OrderBy(@event => @event.EventScheduledStart.Date)
                            .ThenBy(@event => @event.EventTitle)
                            .Select(@event =>
                            {
                                var instructors = ServiceLocator.EventSearch
                                    .GetAttendees(@event.EventIdentifier, y => y.Person.User)
                                    .Where(y => y.AttendeeRole == "Instructor")
                                    .ToList();

                                var instructorsText = instructors.Count > 0
                                    ? string.Join("; ", instructors.Select(y => y.UserFullName))
                                    : "None";

                                return new ClassItem
                                {
                                    Number = number++,
                                    ClassTitle = @event.EventTitle,
                                    ClassStart = @event.EventScheduledStart.FormatDateOnly(timeZone),
                                    ClassEnd = @event.EventScheduledEnd.HasValue ? @event.EventScheduledEnd.FormatDateOnly(timeZone) : "N/A",
                                    ClassInstructors = instructorsText,
                                    RegistrationCount = @event.Registrations.Count(y =>
                                            (string.IsNullOrEmpty(y.ApprovalStatus) || StringHelper.Equals(y.ApprovalStatus, "Registered"))
                                            && !StringHelper.Equals(y.AttendanceStatus, "Withdrawn/Cancelled")
                                        )
                                };
                            }).ToList()
                    };
                })
                .OrderBy(x => x.AchievementTitle)
                .ToList();

        }

        private void AchievementRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var achievement = (AchievementItem)e.Item.DataItem;

            var registrationRepeater = (Repeater)e.Item.FindControl("ClassRepeater");
            registrationRepeater.DataSource = achievement.Classes;
            registrationRepeater.DataBind();
        }
    }
}