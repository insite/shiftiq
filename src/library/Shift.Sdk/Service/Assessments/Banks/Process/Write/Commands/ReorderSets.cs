using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class ReorderSets : Command
    {
        public Dictionary<int, int> Sequences { get; }

        public ReorderSets(Guid bank, Dictionary<int, int> sequences)
        {
            AggregateIdentifier = bank;
            Sequences = sequences;
        }
    }
}
