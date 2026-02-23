using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class ChangeSetStandard : Command
    {
        public Guid Set { get; set; }
        public Guid Standard { get; set; }

        public ChangeSetStandard(Guid bank, Guid set, Guid standard)
        {
            AggregateIdentifier = bank;
            Set = set;
            Standard = standard;
        }
    }
}
