using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Registrations.Read;
using InSite.Domain.Registrations;

using Shift.Common;

namespace InSite.Admin.Events.Registrations.Reports
{
    public partial class ApprenticeCompletionRateReport : UserControl
    {
        private class DateDifferenceItem
        {
            public int Years { get; set; }
            public int Months { get; set; }
            public int Days { get; set; }
        }

        private class ClassItem
        {
            public string ClassName { get; set; }
            public string StartDate { get; set; }
            public string EndDate { get; set; }
            public string Score { get; set; }
        }

        private class StudentItem
        {
            public string UserFullName { get; set; }
            public string Started { get; set; }
            public string Completed { get; set; }
            public string TimeToPass { get; set; }

            public List<ClassItem> Classes { get; set; }
        }

        private class TimeToPassItem
        {
            public int Years { get; set; }
            public int CredentialCount { get; set; }
            public decimal CompletedPercent { get; set; }

            public List<StudentItem> Students { get; set; }

            public int CompletedCount => Students.Count;
        }

        private class TradeItem
        {
            public string AchievementDescription { get; set; }
            public int StudentCount => Times.Sum(x => x.CompletedCount);

            public List<TimeToPassItem> Times { get; set; }
        }

        public void LoadReport(QRegistrationFilter filter)
        {
            PageTitle.InnerText = "Apprentice Completion Rate";

            var trades = GetTrades(filter);

            TradeRepeater.ItemDataBound += TradeRepeater_ItemDataBound;
            TradeRepeater.DataSource = trades;
            TradeRepeater.DataBind();

            var criteriaItems = RegistrationCriteriaHelper.GetCriteriaItems(filter);

            SearchCriteriaRepeater.Visible = criteriaItems.Count > 0;
            SearchCriteriaRepeater.DataSource = criteriaItems;
            SearchCriteriaRepeater.DataBind();

            NoCriteriaPanel.Visible = criteriaItems.Count == 0;
        }

        private void TradeRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var trade = (TradeItem)e.Item.DataItem;

            var timeRepeater = (Repeater)e.Item.FindControl("TimeRepeater");
            timeRepeater.ItemDataBound += TimeRepeater_ItemDataBound;
            timeRepeater.DataSource = trade.Times;
            timeRepeater.DataBind();
        }

        private void TimeRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var time = (TimeToPassItem)e.Item.DataItem;

            var studentRepeater = (Repeater)e.Item.FindControl("StudentRepeater");
            studentRepeater.ItemDataBound += StudentRepeater_ItemDataBound;
            studentRepeater.DataSource = time.Students;
            studentRepeater.DataBind();
        }

        private void StudentRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var student = (StudentItem)e.Item.DataItem;

            var classRepeater = (Repeater)e.Item.FindControl("ClassRepeater");
            classRepeater.DataSource = student.Classes;
            classRepeater.DataBind();
        }

        private List<TradeItem> GetTrades(QRegistrationFilter filter)
        {
            var timeZone = CurrentSessionState.Identity.User.TimeZone;
            var data = ServiceLocator.RegistrationSearch.GetApprenticeCompletionRateReport(filter);

            var trades = data
                .GroupBy(x => x.AchievementDescription)
                .Select(a => new TradeItem
                {
                    AchievementDescription = a.Key,
                    Times = a
                        .GroupBy(x => GetGroupingYear(x.Classes))
                        .Select(b => new TimeToPassItem
                        {
                            Years = b.Key,
                            CredentialCount = b.Where(x => x.IsCompleted).FirstOrDefault()?.CredentialCount ?? 0,
                            Students = b.Where(x => x.IsCompleted).Select(c =>
                            {
                                var started = c.Classes
                                    .Where(x => x.EventScheduledStart.HasValue)
                                    .OrderBy(x => x.EventScheduledStart)
                                    .FirstOrDefault()?
                                    .EventScheduledStart;

                                var completed = c.Classes
                                    .Where(x => x.EventScheduledEnd.HasValue)
                                    .OrderByDescending(x => x.EventScheduledEnd)
                                    .FirstOrDefault()?
                                    .EventScheduledEnd;

                                var diff = completed.HasValue && started.HasValue ? completed.Value.Subtract(started.Value) : (TimeSpan?)null;

                                return new StudentItem
                                {
                                    UserFullName = c.UserFullName,
                                    Started = started.HasValue ? started.Value.FormatDateOnly(timeZone) : "N/A",
                                    Completed = completed.HasValue ? completed.Value.FormatDateOnly(timeZone) : "N/A",
                                    TimeToPass = FormatDateDifference(started, completed),
                                    Classes = c.Classes
                                        .Where(x => x.EventScheduledStart.HasValue)
                                        .OrderBy(x => x.EventScheduledStart)
                                        .Select(d => new ClassItem
                                        {
                                            ClassName = d.EventTitle,
                                            StartDate = d.EventScheduledStart.FormatDateOnly(timeZone),
                                            EndDate = d.EventScheduledEnd.HasValue ? d.EventScheduledEnd.Value.FormatDateOnly(timeZone) : "N/A",
                                            Score = $"{d.Percent:p2}"
                                        })
                                        .ToList()
                                };
                            })
                            .OrderBy(x => x.UserFullName)
                            .ToList()
                        })
                        .Where(x => x.Students.Count > 0)
                        .OrderBy(x => x.Years)
                        .ToList()
                })
                .OrderBy(x => x.AchievementDescription)
                .ToList();

            foreach (var trade in trades)
            {
                foreach (var time in trade.Times)
                    time.CompletedPercent = (decimal)time.CompletedCount / (decimal)trade.StudentCount;
            }

            return trades;
        }

        private static int GetGroupingYear(List<ApprenticeCompletionRateReportItem.ClassItem> classes)
        {
            var started = classes
                .Where(x => x.EventScheduledStart.HasValue)
                .OrderBy(x => x.EventScheduledStart)
                .FirstOrDefault()?
                .EventScheduledStart;

            var completed = classes
                .Where(x => x.EventScheduledEnd.HasValue)
                .OrderByDescending(x => x.EventScheduledEnd)
                .FirstOrDefault()?
                .EventScheduledEnd;

            var diff = GetDateDifference(started, completed);

            return diff != null ? diff.Years + 1 : 0;
        }

        private static DateDifferenceItem GetDateDifference(DateTimeOffset? started, DateTimeOffset? completed)
        {
            if (started == null || completed == null || completed.Value < started.Value)
                return null;

            var startedUtc = started.Value.UtcDateTime;
            var completedUtc = completed.Value.UtcDateTime;

            var months = 0;

            while (startedUtc < completedUtc)
            {
                months++;
                startedUtc = startedUtc.AddMonths(1);
            }

            if (startedUtc > completedUtc)
                startedUtc = startedUtc.AddMonths(-1);

            var diff = completedUtc.Subtract(startedUtc);
            var days = diff.Days;
            var years = months / 12;

            if (years > 0)
                months %= 12;

            return new DateDifferenceItem { Years = years, Months = months, Days = days };
        }

        private static string FormatDateDifference(DateTimeOffset? started, DateTimeOffset? completed)
        {
            var item = GetDateDifference(started, completed);

            if (item == null)
                return null;

            var result = new List<string>();

            if (item.Years > 0)
                result.Add(item.Years > 1 ? $"{item.Years} years" : $"{item.Years} year");

            if (item.Months > 0)
                result.Add(item.Months > 1 ? $"{item.Months} months" : $"{item.Months} month");

            if (item.Days > 0)
                result.Add(item.Days > 1 ? $"{item.Days} days" : $"{item.Days} day");

            return string.Join(" ", result);
        }
    }
}