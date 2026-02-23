using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class ThirdPartyAssessmentEnabled : Change
    {
        public Guid Form { get; set; }

        public ThirdPartyAssessmentEnabled(Guid form)
        {
            Form = form;
        }
    }
}