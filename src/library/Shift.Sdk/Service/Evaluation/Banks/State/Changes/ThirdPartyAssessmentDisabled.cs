using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class ThirdPartyAssessmentDisabled : Change
    {
        public Guid Form { get; set; }

        public ThirdPartyAssessmentDisabled(Guid form)
        {
            Form = form;
        }
    }
}
