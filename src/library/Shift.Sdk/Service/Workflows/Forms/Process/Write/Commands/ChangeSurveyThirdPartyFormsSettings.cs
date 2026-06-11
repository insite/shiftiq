using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Surveys.Write
{
    public class ChangeSurveyThirdPartyFormsSettings : Command
    {
        public bool Enabled { get; }

        public ChangeSurveyThirdPartyFormsSettings(Guid form, bool enabled)
        {
            AggregateIdentifier = form;
            Enabled = enabled;
        }
    }
}
