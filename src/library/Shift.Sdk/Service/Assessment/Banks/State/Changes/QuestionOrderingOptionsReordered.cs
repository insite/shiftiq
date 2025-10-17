﻿using System;
using System.Collections.Generic;

using Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class QuestionOrderingOptionsReordered : Change
    {
        public Guid Question { get; set; }
        public Dictionary<Guid, int> Order { get; set; }

        public QuestionOrderingOptionsReordered(Guid question, Dictionary<Guid, int> order)
        {
            Question = question;
            Order = order;
        }
    }
}
