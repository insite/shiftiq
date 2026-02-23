using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectPeople : Query<IEnumerable<PersonModel>>, IPersonCriteria
    {
        public Guid? OrganizationId { get; set; }
        public Guid? UserId { get; set; }

        public string EmailExact { get; set; }
        public string EventRole { get; set; }
        public string FullName { get; set; }
    }
}