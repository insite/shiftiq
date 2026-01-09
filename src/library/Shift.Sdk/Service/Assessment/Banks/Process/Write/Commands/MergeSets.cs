using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class MergeSets : Command
    {
        public Guid Set { get; set; }

        public MergeSets(Guid bank, Guid set)
        {
            AggregateIdentifier = bank;
            Set = set;
        }
    }
}