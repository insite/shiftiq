using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class QuestionOrderingOptionDeleted : Change
    {
        public Guid Question { get; set; }
        public Guid Option { get; set; }

        public QuestionOrderingOptionDeleted(Guid question, Guid option)
        {
            Question = question;
            Option = option;
        }
    }
}
