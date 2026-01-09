using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Surveys.Write
{
    public class ChangeSurveyHook : Command
    {
        public ChangeSurveyHook(Guid form, string hook)
        {
            AggregateIdentifier = form;
            Hook = hook;
        }

        public string Hook { get; set; }
    }
}