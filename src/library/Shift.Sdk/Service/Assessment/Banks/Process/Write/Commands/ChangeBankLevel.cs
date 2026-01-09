using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Banks;

namespace InSite.Application.Banks.Write
{
    public class ChangeBankLevel : Command
    {
        public Level Level { get; set; }

        public ChangeBankLevel(Guid bank, Level level)
        {
            AggregateIdentifier = bank;
            Level = level;
        }
    }
}