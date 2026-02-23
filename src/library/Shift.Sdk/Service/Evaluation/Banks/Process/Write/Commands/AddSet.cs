using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class AddSet : Command
    {
        public Guid Set { get; set; }
        public string Name { get; set; }
        public Guid Standard { get; set; }

        public AddSet(Guid bank, Guid set, string name, Guid standard)
        {
            AggregateIdentifier = bank;
            Set = set;
            Name = name;
            Standard = standard;
        }
    }
}