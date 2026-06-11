using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountGroupFields : Query<int>, IGroupFieldCriteria
    {
        public Guid? OrganizationId { get; set; }
    }
}
