using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class GroupMembershipProductModified : Change
    {
        public Guid? MembershipProduct { get; }

        public GroupMembershipProductModified(Guid? membershipProduct)
        {
            MembershipProduct = membershipProduct;
        }
    }
}
