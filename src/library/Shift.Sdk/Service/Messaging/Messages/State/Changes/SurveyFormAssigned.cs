using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Messages
{
    public class SurveyFormAssigned : Change
    {
        public Guid SurveyFormIdentifier { get; set; }

        public SurveyFormAssigned(Guid survey)
        {
            SurveyFormIdentifier = survey;
        }
    }

    public class SurveyFormSubmissionStarted : Change
    {
        public Guid SurveyFormIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
    }

    public class SurveyFormSubmissionCompleted : Change
    {
        public Guid SurveyFormIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
    }
}