using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountGroupConnections : Query<int>, IGroupConnectionCriteria
    {
        public Guid? OrganizationIdentifier { get; set; }
    }
}