using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Banks;

namespace InSite.Application.Banks.Write
{
    public class ChangeSetRandomization : Command
    {
        public Guid Set { get; set; }
        public Randomization Randomization { get; set; }

        public ChangeSetRandomization(Guid bank, Guid set, Randomization randomization)
        {
            AggregateIdentifier = bank;
            Set = set;
            Randomization = randomization;
        }
    }
}
