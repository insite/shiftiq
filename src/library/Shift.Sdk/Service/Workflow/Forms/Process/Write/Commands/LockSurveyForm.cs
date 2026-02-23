using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Surveys.Write
{
    public class LockSurveyForm : Command
    {
        public LockSurveyForm(Guid form, DateTimeOffset locked)
        {
            AggregateIdentifier = form;
            Locked = locked;
        }

        public DateTimeOffset Locked { get; set; }
    }
}