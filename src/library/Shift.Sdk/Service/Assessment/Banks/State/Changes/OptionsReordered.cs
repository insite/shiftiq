using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class OptionsReordered : Change
    {
        public Guid Question { get; set; }
        public Dictionary<int, int> Sequences { get; set; }

        public OptionsReordered(Guid question, Dictionary<int, int> sequences)
        {
            Question = question;
            Sequences = sequences;
        }
    }
}
