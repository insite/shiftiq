using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyFormLocked : Change
    {
        public SurveyFormLocked(DateTimeOffset locked)
        {
            Locked = locked;
        }

        public DateTimeOffset Locked { get; set; }
    }
}