using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyDisplaySummaryChartChanged : Change
    {
        public SurveyDisplaySummaryChartChanged(bool displaySummaryChart)
        {
            DisplaySummaryChart = displaySummaryChart;
        }

        public bool DisplaySummaryChart { get; set; }
    }
}
