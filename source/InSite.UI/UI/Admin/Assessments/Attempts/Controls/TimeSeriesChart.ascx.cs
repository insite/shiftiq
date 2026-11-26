using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;

using InSite.Application.Attempts.Read;
using InSite.Common.Web.UI;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Admin.Assessments.Attempts.Controls
{
    public partial class TimeSeriesChart : BaseUserControl
    {
        #region Constants

        private const int DaysPerPage = 365;

        #endregion

        #region Classes

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        public class JsonDataSource
        {
            #region Properties

            [JsonProperty(PropertyName = "daily")]
            public BarChartData DailyData { get; set; }

            [JsonProperty(PropertyName = "weekly")]
            public BarChartData WeeklyData { get; set; }

            [JsonProperty(PropertyName = "monthly")]
            public BarChartData MonthlyData { get; set; }

            #endregion
        }

        [Serializable]
        private class DataContainer
        {
            #region Classes

            public interface IDateItem
            {
                DateTime Date { get; }
                int Count { get; }
                bool IsLastItem { get; }
            }

            private class DateItem : IDateItem
            {
                public DateTime Date { get; set; }
                public int Count { get; set; }
                public bool IsLastItem { get; set; }
            }

            #endregion

            #region Properties

            public DateTime StartDate => _startDate;

            public int Page
            {
                get => _pageIndex;
                set
                {
                    if (value < 0)
                        _pageIndex = 0;
                    else if (value >= _pageCount)
                        _pageIndex = _pageCount - 1;
                    else
                        _pageIndex = value;
                }
            }

            public bool IsFirstPage => _pageIndex == 0;

            public bool IsLastPage => _pageIndex == _pageCount - 1;

            #endregion

            #region Fields

            private DateTime _startDate;
            private int _pageIndex;
            private int _pageCount;
            private int _pageOffset;
            private List<DataItem> _items;
            private List<Tuple<int, int>> _pages;

            #endregion

            #region Construction

            public DataContainer(DateTime startDate, List<DataItem> items)
            {
                _startDate = startDate;

                var lastDay = items.Last().Day;
                _pageCount = lastDay / DaysPerPage + 1;
                _pageOffset = _pageCount > 1
                    ? (int)Math.Floor((decimal)DaysPerPage - lastDay % DaysPerPage) - 1
                    : 0;
                _pageIndex = _pageCount - 1;

                if (_pageOffset < 0)
                    _pageOffset = 0;

                _items = items;
                _pages = new List<Tuple<int, int>>();

                var nextPageDayStart = 0;

                for (var i = 0; i < items.Count; i++)
                {
                    var day = items[i].Day;
                    if (day >= nextPageDayStart)
                    {
                        var pageIndex = (int)Math.Floor((decimal)(day + _pageOffset) / DaysPerPage);
                        _pages.Add(new Tuple<int, int>(pageIndex, i));
                        nextPageDayStart = pageIndex * DaysPerPage - _pageOffset + DaysPerPage;
                    }
                }
            }

            #endregion

            #region Methods

            public void GetPageDateRange(out DateTime startDate, out DateTime endDate)
            {
                GetPageDayRange(out var startDay, out var endDay);

                startDate = GetDate(startDay);
                endDate = GetDate(endDay);
            }

            public IEnumerable<IDateItem> EnumeratePageItems()
            {
                int index = -1;
                DataItem item = null;

                var page = _pages.BinaryItemSearch(x => x.Item1.CompareTo(_pageIndex));
                if (page != null)
                {
                    index = page.Item2;
                    item = _items[index];
                }

                GetPageDayRange(out var startDay, out var endDay);

                var result = new DateItem();

                while (!result.IsLastItem)
                {
                    result.Date = GetDate(startDay);

                    if (item != null && item.Day == startDay)
                    {
                        result.Count = item.Count;

                        index++;

                        if (index < _items.Count)
                            item = _items[index];
                    }
                    else
                    {
                        result.Count = 0;
                    }

                    result.IsLastItem = startDay >= endDay;

                    yield return result;

                    startDay++;
                }
            }

            #endregion

            #region Methods (helpers)

            private void GetPageDayRange(out int startDay, out int endDay)
            {
                if (_pageCount == 1)
                {
                    startDay = _items.First().Day;
                    endDay = _items.Last().Day;

                    var range = endDay - startDay;
                    if (range < 30)
                    {
                        var offset = (30 - range) / 2;
                        startDay -= offset;
                        endDay += offset;
                    }
                }
                else
                {
                    startDay = _pageIndex * DaysPerPage - _pageOffset;
                    endDay = startDay + DaysPerPage - 1;
                }
            }

            private DateTime GetDate(int day) => _startDate.AddDays(day);

            #endregion
        }

        [Serializable]
        private class DataItem
        {
            public int Day { get; }
            public int Count { get; set; }

            public DataItem(int day)
            {
                Day = day;
            }
        }

        #endregion

        #region Properties

        private DataContainer DataSource
        {
            get => (DataContainer)ViewState[nameof(DataSource)];
            set => ViewState[nameof(DataSource)] = value;
        }

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PagePrevButton.Click += PagePrevButton_Click;
            PageNextButton.Click += PageNextButton_Click;
        }

        private void PagePrevButton_Click(object sender, EventArgs e)
        {
            DataSource.Page--;
            BindData();
        }

        private void PageNextButton_Click(object sender, EventArgs e)
        {
            DataSource.Page++;
            BindData();
        }

        public void LoadData(AttemptAnalysis analysis)
        {
            var timeZone = CurrentSessionState.Identity.User.TimeZone;

            var attemptDates = analysis.Attempts
                .Select(x => TimeZones.ConvertTime(x.AttemptGraded ?? x.AttemptSubmitted.Value, timeZone).Date)
                .OrderBy(x => x)
                .ToArray();

            var today = TimeZones.ConvertTime(DateTimeOffset.Now, timeZone).Date;
            var startOfThisWeek = today.AddDays(-(int)today.DayOfWeek);
            var startOfLastWeek = startOfThisWeek.AddDays(-7);
            var startOfThisMonth = today.AddDays(1 - today.Day);
            var startOfLastMonth = startOfThisMonth.AddMonths(-1);

            var summaryOverall = 0;
            var summaryToday = 0;
            var summaryThisWeek = 0;
            var summaryLastWeek = 0;
            var summaryThisMonth = 0;
            var summaryLastMonth = 0;

            var hasData = attemptDates.Length > 0;

            if (hasData)
            {
                var startDate = attemptDates[0];
                var items = new List<DataItem>();
                var item = new DataItem(-1);

                for (var i = 0; i < attemptDates.Length; i++)
                {
                    var date = attemptDates[i];

                    summaryOverall++;

                    if (date == today)
                        summaryToday++;

                    if (date >= startOfThisWeek && date <= today)
                        summaryThisWeek++;

                    if (date >= startOfLastWeek && date < startOfThisWeek)
                        summaryLastWeek++;

                    if (date >= startOfThisMonth && date <= today)
                        summaryThisMonth++;

                    if (date >= startOfLastMonth && date < startOfThisMonth)
                        summaryLastMonth++;

                    var day = (date - startDate).Days;

                    if (item.Day != day)
                        items.Add(item = new DataItem(day));

                    item.Count++;
                }

                DataSource = new DataContainer(startDate, items);
            }
            else
            {
                DataSource = null;
            }

            StatisticInfo.Visible = hasData;
            StatisticChart.Visible = hasData;
            NoResponsesMessage.Visible = !hasData;

            ResponseToday.Text = $@"{summaryToday:n0}";
            ResponseThisWeek.Text = $@"{summaryThisWeek:n0}";
            ResponseLastWeek.Text = $@"{summaryLastWeek:n0}";
            ResponseThisMonth.Text = $@"{summaryThisMonth:n0}";
            ResponseLastMonth.Text = $@"{summaryLastMonth:n0}";
            ResponseOverall.Text = $@"{summaryOverall:n0}";

            BindData();
        }

        private void BindData()
        {
            if (DataSource == null)
            {
                StatisticData.Value = null;
                return;
            }

            var jsonData = new JsonDataSource
            {
                DailyData = new BarChartData(),
                WeeklyData = new BarChartData(),
                MonthlyData = new BarChartData(),
            };

            var currentCulture = CultureInfo.CurrentCulture;
            var barColor = ColorTranslator.FromHtml("#86c557");

            var dailyDataset = jsonData.DailyData.CreateDataset("daily");
            dailyDataset.Label = "Responses";
            dailyDataset.BackgroundColor = barColor;

            var weeklyDataset = jsonData.WeeklyData.CreateDataset("weekly");
            weeklyDataset.Label = "Responses";
            weeklyDataset.BackgroundColor = barColor;

            var monthlyDataset = jsonData.MonthlyData.CreateDataset("monthly");
            monthlyDataset.Label = "Responses";
            monthlyDataset.BackgroundColor = barColor;

            DataSource.GetPageDateRange(out var startDate, out var endDate);

            var prevWeek = GetWeekNumber(startDate, currentCulture);
            var prevWeekYear = startDate.Year;
            var weeklyValue = 0;

            var prevMonth = startDate;
            var monthlyValue = 0;

            foreach (var item in DataSource.EnumeratePageItems())
            {
                // daily

                var dailyItem = dailyDataset.NewItem();
                dailyItem.Label = $"{item.Date:MMM d, yyyy}";
                dailyItem.Value = item.Count;

                // weekly

                var currentWeek = GetWeekNumber(item.Date, currentCulture);

                if (prevWeek == currentWeek)
                    weeklyValue += item.Count;

                if (prevWeek != currentWeek || item.IsLastItem)
                {
                    var weeklyItem = weeklyDataset.NewItem();
                    weeklyItem.Label = $"Week {prevWeek}, {prevWeekYear}";

                    if (prevWeekYear != item.Date.Year)
                        weeklyItem.Label += " - " + item.Date.Year;

                    weeklyItem.Value = weeklyValue;

                    prevWeek = currentWeek;
                    prevWeekYear = item.Date.Year;
                    weeklyValue = item.Count;
                }

                // monthly

                if (prevMonth.Month == item.Date.Month)
                    monthlyValue += item.Count;

                if (prevMonth.Month != item.Date.Month || item.IsLastItem)
                {
                    var monthlyItem = monthlyDataset.NewItem();
                    monthlyItem.Label = $"{prevMonth:MMM, yyyy}";
                    monthlyItem.Value = monthlyValue;

                    prevMonth = item.Date;
                    monthlyValue = item.Count;
                }
            }

            PageTitle.Text = $"{startDate:MMM d, yyyy} - {endDate:MMM d, yyyy}";
            PageNextButton.Enabled = !DataSource.IsLastPage;
            PagePrevButton.Enabled = !DataSource.IsFirstPage;
            StatisticData.Value = JsonConvert.SerializeObject(jsonData);
        }

        private static int GetWeekNumber(DateTime date, CultureInfo culture) =>
            culture.Calendar.GetWeekOfYear(date, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
    }
}
