using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectRegistrationInstructors : Query<IEnumerable<RegistrationInstructorModel>>, IRegistrationInstructorCriteria
    {
        public Guid? OrganizationIdentifier { get; set; }
    }
}