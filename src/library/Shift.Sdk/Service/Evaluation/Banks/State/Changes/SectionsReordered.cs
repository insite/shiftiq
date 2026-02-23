using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class SectionsReordered : Change
    {
        public Guid Form { get; set; }
        public Dictionary<int, int> Sequences { get; }

        public SectionsReordered(Guid form, Dictionary<int, int> sequences)
        {
            Form = form;
            Sequences = sequences;
        }
    }
}
