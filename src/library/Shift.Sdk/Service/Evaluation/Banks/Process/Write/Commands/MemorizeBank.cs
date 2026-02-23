using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class MemorizeBank : Command
    {
        public MemorizeBank(Guid bank)
        {
            AggregateIdentifier = bank;
        }
    }
}