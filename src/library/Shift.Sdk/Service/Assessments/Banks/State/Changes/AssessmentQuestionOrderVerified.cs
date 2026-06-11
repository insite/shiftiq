using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class AssessmentQuestionOrderVerified : Change
    {
        public Guid Form { get; set; }
        public Guid[] Questions { get; set; }

        public AssessmentQuestionOrderVerified(Guid form, Guid[] questions)
        {
            Form = form;
            Questions = questions;
        }
    }
}