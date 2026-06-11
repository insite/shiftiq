using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class QuestionSetChanged : Change
    {
        public Guid Question { get; set; }
        public Guid Set { get; set; }

        public QuestionSetChanged(Guid question, Guid set)
        {
            Question = question;
            Set = set;
        }
    }
}
