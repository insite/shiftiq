using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectUserFields : Query<IEnumerable<UserFieldModel>>, IUserFieldCriteria
    {
        public Guid? OrganizationId { get; set; }
        public Guid? UserId { get; set; }

        public string Name { get; set; }
        public string ValueJson { get; set; }
        public string ValueType { get; set; }
    }
}