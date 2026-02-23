using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class RenameBank : Command
    {
        public string Name { get; set; }

        public RenameBank(Guid bank, string name)
        {
            AggregateIdentifier = bank;
            Name = name;
        }
    }
}