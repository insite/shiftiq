using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountUserFields : Query<int>, IUserFieldCriteria
    {
        public Guid? OrganizationId { get; set; }
    }
}
