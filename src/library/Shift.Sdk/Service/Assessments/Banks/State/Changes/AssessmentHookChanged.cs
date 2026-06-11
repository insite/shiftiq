using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class AssessmentHookChanged : Change
    {
        public Guid Form { get; set; }
        public string Hook { get; set; }

        public AssessmentHookChanged(Guid form, string hook)
        {
            Form = form;
            Hook = hook;
        }
    }
}