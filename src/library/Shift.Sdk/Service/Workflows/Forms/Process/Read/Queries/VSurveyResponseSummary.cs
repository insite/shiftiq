using System;

namespace InSite.Application.Surveys.Read
{
    public class VSurveyResponseSummary
    {
        public Guid SurveyFormIdentifier { get; set; }

        public DateTimeOffset? MinResponseStarted { get; set; }
        public DateTimeOffset? MaxResponseCompleted { get; set; }
        public int? AvgResponseTimeTaken { get; set; }
        public int SumResponseStartCount { get; set; }
        public int SumResponseCompleteCount { get; set; }
    }
}
