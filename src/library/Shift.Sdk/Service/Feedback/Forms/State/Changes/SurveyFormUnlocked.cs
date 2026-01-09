using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyFormUnlocked : Change
    {
        public SurveyFormUnlocked(DateTimeOffset unlocked)
        {
            Unlocked = unlocked;
        }

        public DateTimeOffset Unlocked { get; set; }
    }
}