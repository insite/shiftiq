using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertMembershipDeletion : Query<bool>
    {
        public Guid DeletionIdentifier { get; set; }
    }
}