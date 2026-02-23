using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyRespondentsAdded : Change
    {
        public SurveyRespondentsAdded(Guid[] respondents)
        {
            Respondents = respondents;
        }

        public Guid[] Respondents { get; set; }
    }
}