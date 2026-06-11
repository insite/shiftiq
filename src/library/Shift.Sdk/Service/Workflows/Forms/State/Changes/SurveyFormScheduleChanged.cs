using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyFormScheduleChanged : Change
    {
        public SurveyFormScheduleChanged(DateTimeOffset? opened, DateTimeOffset? closed)
        {
            Opened = opened;
            Closed = closed;
        }

        public DateTimeOffset? Opened { get; }
        public DateTimeOffset? Closed { get; }
    }
}