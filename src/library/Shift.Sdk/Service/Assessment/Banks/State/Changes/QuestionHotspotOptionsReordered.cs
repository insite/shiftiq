using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class QuestionHotspotOptionsReordered : Change
    {
        public Guid QuestionIdentifier { get; set; }
        public Dictionary<int, int> Order { get; }

        public QuestionHotspotOptionsReordered(Guid questionIdentifier, Dictionary<int, int> order)
        {
            QuestionIdentifier = questionIdentifier;
            Order = order;
        }
    }
}
