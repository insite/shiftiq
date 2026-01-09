using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectUserFields : Query<IEnumerable<UserFieldModel>>, IUserFieldCriteria
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }

        public string Name { get; set; }
        public string ValueJson { get; set; }
        public string ValueType { get; set; }
    }
}