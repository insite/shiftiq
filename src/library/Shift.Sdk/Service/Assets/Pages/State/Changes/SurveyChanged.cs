using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Sites.Pages
{
    public class SurveyChanged : Change
    {
        public Guid? Survey { get; set; }
        public SurveyChanged(Guid? survey)
        {
            Survey = survey;
        }
    }
}
