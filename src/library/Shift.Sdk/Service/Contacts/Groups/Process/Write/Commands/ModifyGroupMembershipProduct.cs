using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Groups.Write
{
    public class ModifyGroupMembershipProduct : Command
    {
        public Guid? MembershipProduct { get; }

        public ModifyGroupMembershipProduct(Guid group, Guid? membershipProduct)
        {
            AggregateIdentifier = group;
            MembershipProduct = membershipProduct;
        }
    }
}
