using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class AttachmentUpgraded : Change
    {
        public Guid CurrentAttachment { get; set; }
        public Guid UpgradedAttachment { get; set; }

        public AttachmentUpgraded(Guid current, Guid upgraded)
        {
            CurrentAttachment = current;
            UpgradedAttachment = upgraded;
        }
    }
}
