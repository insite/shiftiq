using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class QuestionOrderingSolutionDeleted : Change
    {
        public Guid Question { get; set; }
        public Guid Solution { get; set; }

        public QuestionOrderingSolutionDeleted(Guid question, Guid solution)
        {
            Question = question;
            Solution = solution;
        }
    }
}
