using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class UpgradeAttachment : Command
    {
        public Guid CurrentAttachment { get; set; }
        public Guid UpgradedAttachment { get; set; }

        public UpgradeAttachment(Guid bank, Guid currentAttachment, Guid upgradedAttachment)
        {
            AggregateIdentifier = bank;
            CurrentAttachment = currentAttachment;
            UpgradedAttachment = upgradedAttachment;
        }
    }
}
