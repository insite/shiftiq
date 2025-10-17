using System;

using Common.Timeline.Commands;

namespace InSite.Application.Surveys.Write
{
    public class ChangeSurveyDisplaySummaryChart : Command
    {
        public ChangeSurveyDisplaySummaryChart(Guid form, bool displaySummaryChart)
        {
            AggregateIdentifier = form;
            DisplaySummaryChart = displaySummaryChart;
        }

        public bool DisplaySummaryChart { get; }
    }
}
