using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class QuestionRubricConnected : Change
    {
        public Guid Question { get; set; }
        public Guid Rubric { get; set; }

        public QuestionRubricConnected(Guid question, Guid rubric)
        {
            Question = question;
            Rubric = rubric;
        }
    }
}
