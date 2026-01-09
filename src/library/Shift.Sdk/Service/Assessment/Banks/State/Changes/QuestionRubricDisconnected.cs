using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class QuestionRubricDisconnected : Change
    {
        public Guid Question { get; set; }

        public QuestionRubricDisconnected(Guid question)
        {
            Question = question;
        }
    }
}
