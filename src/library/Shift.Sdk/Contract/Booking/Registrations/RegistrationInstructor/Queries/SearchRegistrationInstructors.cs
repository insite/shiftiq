using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchRegistrationInstructors : Query<IEnumerable<RegistrationInstructorMatch>>, IRegistrationInstructorCriteria
    {
        public Guid? OrganizationIdentifier { get; set; }
    }
}