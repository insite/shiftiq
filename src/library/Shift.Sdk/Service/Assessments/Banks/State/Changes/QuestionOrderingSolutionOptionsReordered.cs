using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class QuestionOrderingSolutionOptionsReordered : Change
    {
        public Guid Question { get; set; }
        public Guid Solution { get; set; }
        public Dictionary<Guid, int> Order { get; set; }

        public QuestionOrderingSolutionOptionsReordered(Guid question, Guid solution, Dictionary<Guid, int> order)
        {
            Question = question;
            Solution = solution;
            Order = order;
        }
    }
}
