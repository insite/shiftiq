using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Sites.Pages
{
    public class PageAssessmentChanged : Change
    {
        public Guid? Assessment { get; set; }
        public PageAssessmentChanged(Guid? assessment)
        {
            Assessment = assessment;
        }
    }
}
