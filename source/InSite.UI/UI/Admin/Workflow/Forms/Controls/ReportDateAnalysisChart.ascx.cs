using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;

using InSite.Application.Surveys.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Surveys.Forms;

using Newtonsoft.Json;

using Shift.Sdk.UI;

namespace InSite.Admin.Workflow.Forms.Controls
{
    public partial class ReportDateAnalysisChart : BaseUserControl
    {
        #region Classes

        [Serializable, JsonObject(MemberSerialization = MemberSerialization.OptIn)]
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

        public interface IDataItem
        {
            DateTimeOffset DateTime { get; }
            int Count { get; }
        }

        private interface IResultDaySummary
        {
            DateTime Date { get; }
            int Count { get; }
            bool IsLastDay { get; }
        }

        private class SummaryEnumerator : IEnumerator<IResultDaySummary>, IResultDaySummary
        {
            #region Properties

            public DateTime StartDate => _startDate;

            public bool HasData => _data != null;

            public IResultDaySummary Current => IsValid() ? this : null;

            #endregion

            #region Fields

            private readonly DateTime _startDate;
            private readonly int _daysCount;
            private readonly IDataItem[] _data;

            private int _currentIndex;
            private int _currentDay;
            private DateTime _currentDate;
            private int _currentCount;
            private DateTime _nextDataDate;

            #endregion

            #region Construction

            public SummaryEnumerator(IEnumerable<IDataItem> data, TimeZoneInfo timezone)
            {
                if (data.Any())
                {
                    _data = data.OrderBy(x => x.DateTime).ToArray();
                    _startDate = (_data[0].DateTime).Date;
                    _daysCount = (int)Math.Ceiling(((_data[_data.Length - 1].DateTime).Date - _startDate).TotalDays);
                }
                else
                {
                    _data = null;
                    _startDate = DateTime.MaxValue;
                    _daysCount = int.MinValue;
                }

                Reset();
            }

            #endregion

            #region Methods

            public bool MoveNext()
            {
                _currentDay++;

                var isValid = IsValid();

                if (isValid)
                {
                    _currentDate = _startDate.AddDays(_currentDay);
                    _currentCount = 0;

                    if (_currentDate >= _nextDataDate)
                    {
                        for (; _currentIndex < _data.Length; _currentIndex++)
                        {
                            var dataItem = _data[_currentIndex];
                            var itemDate = dataItem.DateTime.Date;

                            if (itemDate > _currentDate)
                            {
                                _nextDataDate = itemDate;
                                break;
                            }

                            _currentCount += dataItem.Count;
                        }
                    }
                }

                return isValid;
            }

            public void Reset()
            {
                _currentIndex = 0;
                _currentDay = -1;
                _currentCount = -1;
                _nextDataDate = DateTime.MinValue;
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);
            }

            #endregion

            #region IEnumerator

            object IEnumerator.Current => Current;

            #endregion

            #region IResultDaySummary

            DateTime IResultDaySummary.Date => _currentDate;
            int IResultDaySummary.Count => _currentCount;
            bool IResultDaySummary.IsLastDay => _currentDay == _daysCount;

            #endregion

            #region Helper methods

            private bool IsValid() => _currentDay >= 0 && _currentDay <= _daysCount;

            #endregion
        }

        private class DataItem : IDataItem
        {
            public DateTimeOffset DateTime { get; set; }
            public int Count => 1;
        }

        #endregion

        #region Properties

        protected Guid SurveyID
        {
            get => (Guid?)ViewState[nameof(SurveyID)] ?? Guid.Empty;
            set => ViewState[nameof(SurveyID)] = value;
        }

        protected JsonDataSource DataSource
        {
            get => (JsonDataSource)ViewState[nameof(DataSource)];
            set => ViewState[nameof(DataSource)] = value;
        }

        #endregion

        public void LoadData(SurveyForm survey, IEnumerable<ISurveyResponse> responses)
        {
            var data = responses
                .Select(x => new DataItem
                {
                    DateTime = x.ResponseSessionCompleted ?? x.ResponseSessionStarted ?? x.ResponseSessionCreated ?? DateTimeOffset.MinValue
                })
                .Where(x => x.DateTime != DateTimeOffset.MinValue)
                .OrderBy(x => x.DateTime)
                .ToArray();

            LoadData(survey.Identifier, data);
        }

        public void LoadData(Guid surveyId, IEnumerable<IDataItem> data)
        {
            SurveyID = surveyId;

            var summary = new SummaryEnumerator(data, CurrentSessionState.Identity.User.TimeZone);

            LoadData(summary);
        }

        private void LoadData(SummaryEnumerator summary)
        {
            DataSource = new JsonDataSource
            {
                DailyData = new BarChartData(),
                WeeklyData = new BarChartData(),
                MonthlyData = new BarChartData(),
            };

            StatisticInfo.Visible = summary.HasData;
            StatisticChart.Visible = summary.HasData;
            NoResponsesMessage.Visible = !summary.HasData;

            if (!summary.HasData)
                return;

            var nowDate = DateTime.UtcNow.Date;
            var startOfThisWeek = nowDate.AddDays(-(int)nowDate.DayOfWeek);
            var startOfLastWeek = startOfThisWeek.AddDays(-7);
            var endOfLastWeek = startOfThisWeek.AddDays(-1);
            var startOfThisMonth = nowDate.AddDays(1 - nowDate.Day);
            var startOfLastMonth = startOfThisMonth.AddMonths(-1);
            var endOfLastMonth = startOfThisMonth.AddDays(-1);

            var summaryOverall = 0;
            var summaryToday = 0;
            var summaryThisWeek = 0;
            var summaryLastWeek = 0;
            var summaryThisMonth = 0;
            var summaryLastMonth = 0;

            var barColor = ColorTranslator.FromHtml("#86c557");

            var dailyDataset = DataSource.DailyData.CreateDataset("daily");
            dailyDataset.Label = "Submissions";
            dailyDataset.BackgroundColor = barColor;

            var weeklyDataset = DataSource.WeeklyData.CreateDataset("weekly");
            weeklyDataset.Label = "Submissions";
            weeklyDataset.BackgroundColor = barColor;

            var monthlyDataset = DataSource.MonthlyData.CreateDataset("monthly");
            monthlyDataset.Label = "Submissions";
            monthlyDataset.BackgroundColor = barColor;

            var currentCulture = CultureInfo.CurrentCulture;

            var prevWeek = GetWeekNumber(summary.StartDate, currentCulture);
            var prevWeekYear = summary.StartDate.Year;
            var weeklyValue = 0;

            var prevMonth = summary.StartDate;
            var monthlyValue = 0;

            while (summary.MoveNext())
            {
                var data = summary.Current;

                // daily

                var dailyItem = dailyDataset.NewItem();
                dailyItem.Label = $"{data.Date:MMM d, yyyy}";
                dailyItem.Value = data.Count;

                // weekly

                var currentWeek = GetWeekNumber(data.Date, currentCulture);

                if (prevWeek == currentWeek)
                    weeklyValue += data.Count;

                if (prevWeek != currentWeek || data.IsLastDay)
                {
                    var weeklyItem = weeklyDataset.NewItem();
                    weeklyItem.Label = $"Week {prevWeek}, {prevWeekYear}";

                    if (prevWeekYear != data.Date.Year)
                        weeklyItem.Label += " - " + data.Date.Year;

                    weeklyItem.Value = weeklyValue;

                    prevWeek = currentWeek;
                    prevWeekYear = data.Date.Year;
                    weeklyValue = data.Count;
                }

                // monthly

                if (prevMonth.Month == data.Date.Month)
                    monthlyValue += data.Count;

                if (prevMonth.Month != data.Date.Month || data.IsLastDay)
                {
                    var monthlyItem = monthlyDataset.NewItem();
                    monthlyItem.Label = $"{prevMonth:MMM, yyyy}";
                    monthlyItem.Value = monthlyValue;

                    prevMonth = data.Date;
                    monthlyValue = data.Count;
                }

                // summary

                if (data.Count > 0)
                {
                    summaryOverall += data.Count;

                    if (data.Date == nowDate)
                        summaryToday += data.Count;

                    if (data.Date >= startOfThisWeek && data.Date <= nowDate)
                        summaryThisWeek += data.Count;

                    if (data.Date >= startOfLastWeek && data.Date <= endOfLastWeek)
                        summaryLastWeek += data.Count;

                    if (data.Date >= startOfThisMonth && data.Date <= nowDate)
                        summaryThisMonth += data.Count;

                    if (data.Date >= startOfLastMonth && data.Date <= endOfLastMonth)
                        summaryLastMonth += data.Count;
                }
            }

            ResponseToday.Text = $@"{summaryToday:n0}";
            ResponseThisWeek.Text = $@"{summaryThisWeek:n0}";
            ResponseLastWeek.Text = $@"{summaryLastWeek:n0}";
            ResponseThisMonth.Text = $@"{summaryThisMonth:n0}";
            ResponseLastMonth.Text = $@"{summaryLastMonth:n0}";
            ResponseOverall.Text = $@"{summaryOverall:n0}";
        }

        private static int GetWeekNumber(DateTime date, CultureInfo culture) =>
            culture.Calendar.GetWeekOfYear(date, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
    }
}