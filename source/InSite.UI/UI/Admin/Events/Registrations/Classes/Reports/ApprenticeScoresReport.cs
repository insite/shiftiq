using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Records.Read;
using InSite.Application.Registrations.Read;
using InSite.Domain.Registrations;

using Shift.Common;
using Shift.Constant;
using Shift.Toolbox;

using TimeZone = Shift.Common.TimeZone;

namespace InSite.Admin.Events.Registrations.Reports
{
    public static class ApprenticeScoresReport
    {
        private class Summary
        {
            public string AchievementDescription { get; set; }
            public string EventTitle { get; set; }
            public DateTime EventScheduledStart { get; set; }
            public string UserFullName { get; set; }
            public string UserEmail { get; set; }
            public string EmployerGroupName { get; set; }
            public string Region { get; set; }
            public string Street1 { get; set; }
            public string Street2 { get; set; }
            public string City { get; set; }
            public string Province { get; set; }
            public string PostalCode { get; set; }
            public string UserPhone { get; set; }
            public object[] Scores { get; set; }
        }

        private class SummaryInfo
        {
            public Summary Summary { get; set; }
            public DateTimeOffset?[] ScoreDates { get; set; }
        }

        public static byte[] GetXlsx(QRegistrationFilter filter)
        {
            var achievementFilter = new QAchievementFilter(CurrentSessionState.Identity.Organization.OrganizationIdentifier, "Level");

            var achievements = ServiceLocator.AchievementSearch.GetAchievements(achievementFilter);

            var summaries = GetData(filter, achievements);

            var boldStyle = new XlsxCellStyle { IsBold = true };
            var boldRightStyle = new XlsxCellStyle { Align = HorizontalAlignment.Right, IsBold = true };
            var boldCenterStyle = new XlsxCellStyle { Align = HorizontalAlignment.Center, IsBold = true };
            var rightStyle = new XlsxCellStyle { Align = HorizontalAlignment.Right };
            var centerStyle = new XlsxCellStyle { Align = HorizontalAlignment.Center };
            var numericFormat = "#,##0.00";
            var dateFormat = "MMM d, yyyy";

            var sheet = new XlsxWorksheet("Most Improved Report");
            sheet.Columns[0].Width = 40; // AchievementDescription
            sheet.Columns[1].Width = 35; // EventTitle
            sheet.Columns[2].Width = 15; // EventScheduledStart
            sheet.Columns[3].Width = 20; // UserFullName
            sheet.Columns[4].Width = 30; // UserEmail
            sheet.Columns[5].Width = 30; // EmployerGroupName
            sheet.Columns[6].Width = 25; // Region
            sheet.Columns[7].Width = 25; // Street1
            sheet.Columns[8].Width = 25; // Street2
            sheet.Columns[9].Width = 20; // City
            sheet.Columns[10].Width = 20; // Province
            sheet.Columns[11].Width = 15; // PostalCode
            sheet.Columns[12].Width = 15; // UserPhone

            sheet.Cells.Add(new XlsxCell(0, 0) { Value = "Trade", Style = boldStyle }); // AchievementDescription
            sheet.Cells.Add(new XlsxCell(1, 0) { Value = "Class", Style = boldStyle }); // EventTitle
            sheet.Cells.Add(new XlsxCell(2, 0) { Value = "Date Start", Style = boldCenterStyle }); // EventScheduledStart
            sheet.Cells.Add(new XlsxCell(3, 0) { Value = "Student", Style = boldStyle }); // UserFullName
            sheet.Cells.Add(new XlsxCell(4, 0) { Value = "Email", Style = boldStyle }); // UserEmail
            sheet.Cells.Add(new XlsxCell(5, 0) { Value = "Company", Style = boldStyle }); // EmployerGroupName
            sheet.Cells.Add(new XlsxCell(6, 0) { Value = "Region", Style = boldStyle }); // Region
            sheet.Cells.Add(new XlsxCell(7, 0) { Value = "Address 1", Style = boldStyle }); // Street1
            sheet.Cells.Add(new XlsxCell(8, 0) { Value = "Address 2", Style = boldStyle }); // Street2
            sheet.Cells.Add(new XlsxCell(9, 0) { Value = "City", Style = boldStyle }); // City
            sheet.Cells.Add(new XlsxCell(10, 0) { Value = "Province", Style = boldStyle }); // Province
            sheet.Cells.Add(new XlsxCell(11, 0) { Value = "Postal Code", Style = boldStyle }); // PostalCode
            sheet.Cells.Add(new XlsxCell(12, 0) { Value = "Phone Number", Style = boldStyle }); // UserPhone

            for (int i = 0; i < achievements.Count; i++)
            {
                sheet.Columns[13 + i].Width = 20;
                sheet.Cells.Add(new XlsxCell(13 + i, 0) { Value = achievements[i].AchievementTitle, Style = boldRightStyle });
            }

            for (int summaryIndex = 0; summaryIndex < summaries.Count; summaryIndex++)
            {
                var summary = summaries[summaryIndex];

                sheet.Cells.Add(new XlsxCell(0, summaryIndex + 1) { Value = summary.AchievementDescription });
                sheet.Cells.Add(new XlsxCell(1, summaryIndex + 1) { Value = summary.EventTitle }); // EventTitle
                sheet.Cells.Add(new XlsxCell(2, summaryIndex + 1) { Value = summary.EventScheduledStart, Style = centerStyle, Format = dateFormat }); // EventScheduledStart
                sheet.Cells.Add(new XlsxCell(3, summaryIndex + 1) { Value = summary.UserFullName }); // UserFullName
                sheet.Cells.Add(new XlsxCell(4, summaryIndex + 1) { Value = summary.UserEmail }); // UserEmail
                sheet.Cells.Add(new XlsxCell(5, summaryIndex + 1) { Value = summary.EmployerGroupName }); // EmployerGroupName
                sheet.Cells.Add(new XlsxCell(6, summaryIndex + 1) { Value = summary.Region }); // Region
                sheet.Cells.Add(new XlsxCell(7, summaryIndex + 1) { Value = summary.Street1 }); // Street1
                sheet.Cells.Add(new XlsxCell(8, summaryIndex + 1) { Value = summary.Street2 }); // Street2
                sheet.Cells.Add(new XlsxCell(9, summaryIndex + 1) { Value = summary.City }); // City
                sheet.Cells.Add(new XlsxCell(10, summaryIndex + 1) { Value = summary.Province }); // Province
                sheet.Cells.Add(new XlsxCell(11, summaryIndex + 1) { Value = summary.PostalCode }); // PostalCode
                sheet.Cells.Add(new XlsxCell(12, summaryIndex + 1) { Value = summary.UserPhone }); // UserPhone

                for (int scoreIndex = 0; scoreIndex < summary.Scores.Length; scoreIndex++)
                    sheet.Cells.Add(new XlsxCell(scoreIndex + 13, summaryIndex + 1) { Value = summary.Scores[scoreIndex], Style = rightStyle, Format = numericFormat });
            }

            return sheet.GetBytes();
        }

        private static List<Summary> GetData(QRegistrationFilter filter, List<VAchievement> achievements)
        {
            var user = CurrentSessionState.Identity.User;
            var items = ServiceLocator.RegistrationSearch.GetApprenticeScoresReport(filter);
            var result = new List<Summary>();
            var studentScores = new Dictionary<Guid, SummaryInfo>();

            foreach (var item in items)
            {
                var key = item.RegistrationIdentifier;

                var scoreIndex = item.ScoreAchievementIdentifier.HasValue
                    ? achievements.FindIndex(x => x.AchievementIdentifier == item.ScoreAchievementIdentifier)
                    : -1;

                if (studentScores.TryGetValue(key, out var summaryInfo))
                {
                    if (scoreIndex >= 0)
                    {
                        var lastScoreDate = summaryInfo.ScoreDates[scoreIndex];
                        if (lastScoreDate == null || item.GradebookEventStartDate.HasValue && item.GradebookEventStartDate.Value > lastScoreDate)
                        {
                            summaryInfo.ScoreDates[scoreIndex] = item.GradebookEventStartDate;
                            summaryInfo.Summary.Scores[scoreIndex] = GetScoreValue(item);
                        }
                    }
                }
                else
                {
                    var summary = new Summary
                    {
                        AchievementDescription = item.AchievementDescription,
                        EventTitle = item.EventTitle,
                        EventScheduledStart = TimeZone.ConvertTime(item.EventScheduledStart.UtcDateTime, TimeZones.Utc, user.TimeZone),
                        UserFullName = item.UserFullName,
                        UserEmail = item.UserEmail,
                        EmployerGroupName = item.EmployerGroupName,
                        Region = item.Region,
                        Street1 = item.Street1,
                        Street2 = item.Street2,
                        City = item.City,
                        Province = item.Province,
                        PostalCode = item.PostalCode,
                        UserPhone = item.UserPhone,
                        Scores = new object[achievements.Count]
                    };

                    result.Add(summary);

                    summaryInfo = new SummaryInfo { Summary = summary, ScoreDates = new DateTimeOffset?[achievements.Count] };
                    studentScores.Add(key, summaryInfo);

                    if (scoreIndex >= 0)
                    {
                        summaryInfo.ScoreDates[scoreIndex] = item.GradebookEventStartDate;
                        summary.Scores[scoreIndex] = GetScoreValue(item);
                    }
                }
            }

            return result
                .OrderBy(x => x.AchievementDescription)
                .ThenBy(x => x.EventTitle)
                .ThenBy(x => x.UserFullName)
                .ToList();
        }

        private static object GetScoreValue(ApprenticeScoresReportItem item)
        {
            return item.ScorePercent.HasValue
                ? (object)(item.ScorePercent * 100m)
                : (object)item.ScoreText;
        }
    }
}