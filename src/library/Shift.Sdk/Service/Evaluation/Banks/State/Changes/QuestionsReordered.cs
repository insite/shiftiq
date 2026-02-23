using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class QuestionsReordered : Change
    {
        public Guid Set { get; set; }
        public Dictionary<int, int> Sequences { get; set; }

        public QuestionsReordered(Guid set, Dictionary<int, int> sequences)
        {
            Set = set;
            Sequences = sequences;
        }
    }
}
