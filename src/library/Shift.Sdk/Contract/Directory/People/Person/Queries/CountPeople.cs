using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountPeople : Query<int>, IPersonCriteria
    {
        public string EmailExact { get; set; }
        public string EventRole { get; set; }
        public string FullName { get; set; }
        public Guid? UserIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
    }
}