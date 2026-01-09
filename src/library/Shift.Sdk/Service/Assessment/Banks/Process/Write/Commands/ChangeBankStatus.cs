using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class ChangeBankStatus : Command
    {
        public bool IsActive { get; set; }

        public ChangeBankStatus(Guid bank, bool isActive)
        {
            AggregateIdentifier = bank;
            IsActive = isActive;
        }
    }
}
