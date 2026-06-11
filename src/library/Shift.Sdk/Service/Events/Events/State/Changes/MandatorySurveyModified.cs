using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class MandatorySurveyModified : Change
    {
        public Guid? SurveyForm { get; }

        public MandatorySurveyModified(Guid? surveyForm)
        {
            SurveyForm = surveyForm;
        }
    }
}
