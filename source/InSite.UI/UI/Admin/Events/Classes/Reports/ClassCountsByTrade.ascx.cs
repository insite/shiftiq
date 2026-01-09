using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using InSite.Application.Events.Read;
using InSite.Domain.Events;
using InSite.UI.Admin.Events.Classes.Reports;

using Shift.Common;
using Shift.Constant;
using Shift.Toolbox;

using TimeZone = Shift.Common.TimeZone;

namespace InSite.Admin.Events.Classes.Reports
{
    public partial class ClassCountsByTrade : BaseReportControl
    {
        private class AchievementRegistrationState
        {
            public int? Count { get; set; }
            public bool IsCancelled { get; set; }

            public override string ToString()
            {
                return IsCancelled
                    ? "cancelled"
                    : Count.ToString();
            }
        }

        private class TimeFrameItem
        {
            public int Sequence { get; set; }
            public string TimeFrame { get; set; }
            public List<AchievementRegistrationState> AchievementRegistrations { get; set; }

            public int Total => AchievementRegistrations.IsNotEmpty()
                ? AchievementRegistrations.Sum(x => !x.IsCancelled && x.Count.HasValue ? x.Count.Value : 0)
                : 0;
        }

        private class TradeItem
        {
            public string AchievementDescription { get; set; }
            public List<string> Achievements { get; set; }
            public List<TimeFrameItem> TimeFrames { get; set; }

            public List<int> GetAchievementTotals()
            {
                var result = new List<int>();

                for (int i = 0; i < Achievements.Count; i++)
                    result.Add(TimeFrames.Sum(x =>
                    {
                        var item = x.AchievementRegistrations[i];
                        return !item.IsCancelled && item.Count.HasValue ? item.Count.Value : 0;
                    }));

                return result;
            }
        }

        public override string ReportTitle => "Student Counts by Trade";

        public override string ReportFileName => "StudentCountsByTrades";

        public override byte[] GetPdf(QEventFilter filter)
        {
            PageTitle.InnerText = ReportTitle;

            var trades = GetTrades(filter);

            TradeRepeater.ItemDataBound += TradeRepeater_ItemDataBound;
            TradeRepeater.DataSource = trades;
            TradeRepeater.DataBind();

            var total = trades.Sum(x => x.GetAchievementTotals().Sum());

            Total.Text = total.ToString();

            var criteriaItems = GetCriteriaItems(filter);

            SearchCriteriaRepeater.Visible = criteriaItems.Count > 0;
            SearchCriteriaRepeater.DataSource = criteriaItems;
            SearchCriteriaRepeater.DataBind();

            NoCriteriaPanel.Visible = criteriaItems.Count == 0;

            return BuildPdf(PageOrientationType.Portrait, 1400, 980, ReportTitle);
        }

        public override byte[] GetXlsx(QEventFilter filter)
        {
            var trades = GetTrades(filter);
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
            var boldCenterStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Center };
            var normalStyle = new XlsxCellStyle { Align = HorizontalAlignment.Left };
            var centerStyle = new XlsxCellStyle { Align = HorizontalAlignment.Center };

            sheet.Columns[0].Width = 10;

            sheet.Rows[0].Height = criteriaRowHeight;
            var headerCell = new XlsxCellRichText(0, 0, 6) { Style = normalStyle };
            headerCell.AddText(ReportTitle + "\n", true);
            headerCell.AddText(criterias.ToString(), false);
            sheet.Cells.Add(headerCell);

            var row = 1;
            var mostAchievemnts = 0;

            foreach (var trade in trades)
                mostAchievemnts = mostAchievemnts > trade.Achievements.Count ? mostAchievemnts : trade.Achievements.Count;

            var combinedTotal = 0;

            foreach (var trade in trades)
            {
                sheet.Cells.Add(new XlsxCell(0, ++row, 6) { Value = trade.AchievementDescription, Style = boldStyle });
                sheet.Cells.Add(new XlsxCell(0, ++row) { Value = "#", Style = boldCenterStyle });
                sheet.Cells.Add(new XlsxCell(1, row) { Value = "Time Frame", Style = boldStyle });
                for (var i = 0; i < trade.Achievements.Count; i++)
                    sheet.Cells.Add(new XlsxCell(i + 2, row) { Value = trade.Achievements[i], Style = boldCenterStyle });

                foreach (var timeFrame in trade.TimeFrames)
                {
                    sheet.Cells.Add(new XlsxCell(0, ++row) { Value = timeFrame.Sequence, Style = centerStyle });
                    sheet.Cells.Add(new XlsxCell(1, row) { Value = timeFrame.TimeFrame, Style = normalStyle });
                    for (var i = 0; i < trade.Achievements.Count; i++)
                        sheet.Cells.Add(new XlsxCell(i + 2, row) { Value = timeFrame.AchievementRegistrations[i].Count, Style = centerStyle });
                    sheet.Cells.Add(new XlsxCell(mostAchievemnts + 2, row) { Value = timeFrame.Total, Style = boldCenterStyle });
                }

                sheet.Cells.Add(new XlsxCell(1, ++row) { Value = "TOTAL", Style = boldStyle });
                for (var i = 0; i < trade.Achievements.Count; i++)
                    sheet.Cells.Add(new XlsxCell(i + 2, row) { Value = trade.GetAchievementTotals()[i], Style = boldCenterStyle });
                var total = 0;
                foreach (var count in trade.GetAchievementTotals())
                    total += count;
                sheet.Cells.Add(new XlsxCell(mostAchievemnts + 2, row) { Value = total, Style = boldCenterStyle });

                row += 2;

                combinedTotal += total;
            }

            sheet.Cells.Add(new XlsxCell(1, row) { Value = "COMBINED TOTAL", Style = boldStyle });
            sheet.Cells.Add(new XlsxCell(5, row) { Value = combinedTotal, Style = boldCenterStyle });

            for (var i = 0; i < mostAchievemnts + 1; i++)
                sheet.Columns[i + 1].Width = 25;

            return XlsxWorksheet.GetBytes(sheet);
        }

        private List<TradeItem> GetTrades(QEventFilter filter)
        {
            var data = ServiceLocator.EventSearch.GetEventParticipationSummary(filter);

            return RollupTrades(data);
        }

        private static List<TradeItem> RollupTrades(List<EventParticipationSummary> data)
        {
            var result = new List<TradeItem>();

            var achievementDescriptionGroups = data
                .GroupBy(x => x.AchievementDescription)
                .Select(x => new
                {
                    AchievementDescription = x.Key,
                    Summaries = x
                        .OrderBy(y => y.EventScheduledStart.UtcDateTime.Date)
                        .ThenBy(y => y.EventIdentifier)
                        .ToList()
                })
                .OrderBy(x => x.AchievementDescription)
                .ToList();

            foreach (var achievementDescriptionGroup in achievementDescriptionGroups)
            {
                var tradeItem = new TradeItem
                {
                    AchievementDescription = achievementDescriptionGroup.AchievementDescription,
                    Achievements = achievementDescriptionGroup.Summaries
                        .Select(x => x.AchievementTitle)
                        .Distinct()
                        .OrderBy(x => x)
                        .ToList(),
                    TimeFrames = new List<TimeFrameItem>()
                };

                result.Add(tradeItem);

                AddSummaries(tradeItem, achievementDescriptionGroup.Summaries);
            }

            return result;
        }

        private static void AddSummaries(TradeItem tradeItem, List<EventParticipationSummary> summaries)
        {
            foreach (var summary in summaries)
            {
                var timeFrame = GetTimeFrame(summary);
                var achievementRegistration = GetAchievementRegistration(tradeItem, summary.AchievementTitle, timeFrame);

                achievementRegistration.Count = summary.RegistrationCount;

                if (string.Equals(summary.EventSchedulingStatus, "Cancelled", StringComparison.OrdinalIgnoreCase))
                    achievementRegistration.IsCancelled = true;
            }
        }

        private static AchievementRegistrationState GetAchievementRegistration(TradeItem tradeItem, string achievementTitle, string timeFrame)
        {
            var index = tradeItem.Achievements.FindIndex(x => x == achievementTitle);

            var timeFrameItem = tradeItem.TimeFrames
                .Where(x => x.TimeFrame == timeFrame && x.AchievementRegistrations[index].Count == null)
                .OrderBy(x => x.Sequence)
                .FirstOrDefault();

            if (timeFrameItem == null)
                timeFrameItem = AddTimeFrameItem(tradeItem, timeFrame);

            return timeFrameItem.AchievementRegistrations[index];
        }

        private static TimeFrameItem AddTimeFrameItem(TradeItem tradeItem, string timeFrame)
        {
            var timeFrameItem = new TimeFrameItem
            {
                Sequence = tradeItem.TimeFrames.Count + 1,
                TimeFrame = timeFrame,
                AchievementRegistrations = new List<AchievementRegistrationState>()
            };

            tradeItem.TimeFrames.Add(timeFrameItem);

            for (var i = 0; i < tradeItem.Achievements.Count; i++)
                timeFrameItem.AchievementRegistrations.Add(new AchievementRegistrationState());

            return timeFrameItem;
        }

        private static string GetTimeFrame(EventParticipationSummary summary)
        {
            var start = TimeZone.ConvertTime(summary.EventScheduledStart.UtcDateTime, TimeZones.Utc, CurrentSessionState.Identity.User.TimeZone);

            if (summary.EventScheduledEnd == null)
                return $"{start:MMM d, yyyy}";

            var end = TimeZone.ConvertTime(summary.EventScheduledEnd.Value.UtcDateTime, TimeZones.Utc, CurrentSessionState.Identity.User.TimeZone);

            return $"{start:MMM d} - {end:MMM d, yyyy}";
        }

        private void TradeRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var tradeItem = (TradeItem)e.Item.DataItem;

            var achievementRepeater = (Repeater)e.Item.FindControl("AchievementRepeater");
            achievementRepeater.DataSource = tradeItem.Achievements;
            achievementRepeater.DataBind();

            var timeFrameRepeater = (Repeater)e.Item.FindControl("TimeFrameRepeater");
            timeFrameRepeater.ItemDataBound += TimeFrameRepeater_ItemDataBound;
            timeFrameRepeater.DataSource = tradeItem.TimeFrames;
            timeFrameRepeater.DataBind();

            var achievementTotals = tradeItem.GetAchievementTotals();
            var total = achievementTotals.Count > 0 ? achievementTotals.Sum() : 0;

            var totalRepeater = (Repeater)e.Item.FindControl("TotalRepeater");
            totalRepeater.DataSource = achievementTotals;
            totalRepeater.DataBind();

            var totalLiteral = (Literal)e.Item.FindControl("Total");
            totalLiteral.Text = total.ToString();
        }

        private void TimeFrameRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var timeFrameItem = (TimeFrameItem)e.Item.DataItem;

            var achievementRegistrationRepeater = (Repeater)e.Item.FindControl("AchievementRegistrationRepeater");
            achievementRegistrationRepeater.DataSource = timeFrameItem.AchievementRegistrations;
            achievementRegistrationRepeater.DataBind();
        }
    }
}