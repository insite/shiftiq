using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveMembershipDeletion : Query<MembershipDeletionModel>
    {
        public Guid DeletionIdentifier { get; set; }
    }
}