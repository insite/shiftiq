using System.Collections.Generic;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class SetsReordered : Change
    {
        public Dictionary<int, int> Sequences { get; }

        public SetsReordered(Dictionary<int, int> sequences)
        {
            Sequences = sequences;
        }
    }
}
