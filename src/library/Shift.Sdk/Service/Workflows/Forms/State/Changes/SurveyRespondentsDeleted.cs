using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyRespondentsDeleted : Change
    {
        public SurveyRespondentsDeleted(Guid[] respondents)
        {
            Respondents = respondents;
        }

        public Guid[] Respondents { get; set; }
    }
}