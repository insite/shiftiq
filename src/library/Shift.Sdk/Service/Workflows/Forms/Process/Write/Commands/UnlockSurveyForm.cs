using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Surveys.Write
{
    public class UnlockSurveyForm : Command
    {
        public UnlockSurveyForm(Guid form, DateTimeOffset unlocked)
        {
            AggregateIdentifier = form;
            Unlocked = unlocked;
        }

        public DateTimeOffset Unlocked { get; set; }
    }
}