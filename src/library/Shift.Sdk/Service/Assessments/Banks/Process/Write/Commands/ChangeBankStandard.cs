using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class ChangeBankStandard : Command
    {
        public Guid Standard { get; set; }

        public ChangeBankStandard(Guid bank, Guid standard)
        {
            AggregateIdentifier = bank;
            Standard = standard;
        }
    }
}