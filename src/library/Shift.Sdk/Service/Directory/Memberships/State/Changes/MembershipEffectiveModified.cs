using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class MembershipEffectiveModified : Change
    {
        public MembershipEffectiveModified(DateTimeOffset effective)
        {
            Effective = effective;
        }

        public DateTimeOffset Effective { get; set; }
    }
}