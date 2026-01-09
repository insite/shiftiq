using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Surveys.Write
{
    public class ChangeSurveyFormSchedule : Command
    {
        public ChangeSurveyFormSchedule(Guid form, DateTimeOffset? opened, DateTimeOffset? closed)
        {
            AggregateIdentifier = form;
            Opened = opened;
            Closed = closed;
        }

        public DateTimeOffset? Opened { get; }
        public DateTimeOffset? Closed { get; }
    }
}