using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class QuestionConditionChanged : Change
    {
        public Guid Question { get; set; }
        public string Condition { get; set; }

        public QuestionConditionChanged(Guid question, string condition)
        {
            Question = question;
            Condition = condition;
        }
    }
}
