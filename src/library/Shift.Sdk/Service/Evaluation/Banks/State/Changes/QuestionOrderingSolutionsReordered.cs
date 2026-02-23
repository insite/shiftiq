using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class QuestionOrderingSolutionsReordered : Change
    {
        public Guid Question { get; set; }
        public Dictionary<Guid, int> Order { get; set; }

        public QuestionOrderingSolutionsReordered(Guid question, Dictionary<Guid, int> order)
        {
            Question = question;
            Order = order;
        }
    }
}
