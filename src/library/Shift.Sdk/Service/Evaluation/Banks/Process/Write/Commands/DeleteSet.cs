using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class DeleteSet : Command
    {
        public Guid Set { get; set; }

        public DeleteSet(Guid bank, Guid set)
        {
            AggregateIdentifier = bank;
            Set = set;
        }
    }
}