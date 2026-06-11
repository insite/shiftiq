using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountGroupAddresses : Query<int>, IGroupAddressCriteria
    {
        public Guid? OrganizationId { get; set; }
    }
}
