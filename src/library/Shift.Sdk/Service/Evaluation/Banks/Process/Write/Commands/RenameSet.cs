using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class RenameSet : Command
    {
        public string Name { get; set; }
        public Guid Set { get; set; }

        public RenameSet(Guid bank, Guid set, string name)
        {
            AggregateIdentifier = bank;
            Set = set;
            Name = name;
        }
    }
}