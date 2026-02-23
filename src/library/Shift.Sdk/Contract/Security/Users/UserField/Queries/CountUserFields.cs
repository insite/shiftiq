using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountUserFields : Query<int>, IUserFieldCriteria
    {
        public Guid? OrganizationId { get; set; }
        public Guid? UserId { get; set; }

        public string Name { get; set; }
        public string ValueJson { get; set; }
        public string ValueType { get; set; }
    }
}